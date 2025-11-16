using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Entities;
using NavyRadar.Shared.Spec;
using AccountEntity = NavyRadar.Shared.Entities.Account;
using CaptainEntity = NavyRadar.Shared.Entities.Captain;
using CaptainSpec = NavyRadar.Shared.Spec.Captain;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuAccountsVm : ViewModelBase
{
    public ObservableCollection<AccountEntity> Accounts { get; } = [];

    public AccountEntity? CurrentAccount
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            CurrentCaptain = _captainsByAccountId.GetValueOrDefault(field?.Id ?? 0);
        }
    }

    private Dictionary<int, CaptainEntity> _captainsByAccountId = new();

    public CaptainEntity? CurrentCaptain
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public event Func<string, string, bool>? ConfirmationRequested;

    public ICommand UpdateAccountCommand { get; }
    public ICommand NewAccountCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AssignCaptainCommand { get; }
    public ICommand RemoveCaptainCommand { get; }
    public ICommand RemoveAccountCommand { get; }

    public MainMenuAccountsVm()
    {
        UpdateAccountCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnUpdateAccountAsync),
            () => !IsLoading && CurrentAccount != null);

        NewAccountCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnNewAccountAsync),
            () => !IsLoading);

        RefreshCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(LoadAccountsAsync),
            () => !IsLoading);

        AssignCaptainCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnAssignCaptainAsync),
            () => !IsLoading && CurrentAccount != null &&
                  CurrentCaptain == null);

        RemoveCaptainCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRemoveCaptainAsync),
            () => !IsLoading && CurrentCaptain != null);

        RemoveAccountCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRemoveAccountAsync),
            () => !IsLoading && CurrentAccount != null);

        _ = ExecuteLoadingTask(LoadAccountsAsync);
    }

    private async Task LoadAccountsAsync()
    {
        Accounts.Clear();
        _captainsByAccountId.Clear();
        CurrentCaptain = null;

        var captainsFromServer = await ApiService.ApiClient.CaptainsAllAsync();

        _captainsByAccountId = captainsFromServer.ToDictionary(
            c => c.AccountId,
            c => c.ToEntity());

        var accountsFromServer = await ApiService.ApiClient.AccountsAllAsync();
        foreach (var accountSpec in accountsFromServer)
        {
            Accounts.Add(accountSpec.ToEntity());
        }

        CurrentAccount = Accounts.FirstOrDefault();
    }

    private async Task OnUpdateAccountAsync()
    {
        if (CurrentAccount == null) return;

        var accountToEdit = new AccountEntity
        {
            Id = CurrentAccount.Id,
            Username = CurrentAccount.Username,
            Email = CurrentAccount.Email,
            Password = "",
            Role = CurrentAccount.Role
        };

        var dialog = new MainMenuAccountDialog(accountToEdit);

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Account == null) return;

            var updatedEntity = dialog.Account;
            var passwordToSend = updatedEntity.Password;

            if (passwordToSend == CurrentAccount.Password)
            {
                passwordToSend = "";
            }

            var updatedAccount = await ApiService.ApiClient.AccountsPUTAsync(updatedEntity.Id, new UpdateAccount
            {
                Id = updatedEntity.Id,
                Username = updatedEntity.Username,
                Email = updatedEntity.Email,
                Password = passwordToSend,
                Role = updatedEntity.Role.ToString()
            });


            var originalAccountInList = Accounts.FirstOrDefault(s => s.Id == updatedAccount.Id);
            var updatedAccountEntity = updatedAccount.ToEntity();

            if (originalAccountInList != null)
            {
                var index = Accounts.IndexOf(originalAccountInList);

                if (index != -1)
                {
                    Accounts[index] = updatedAccountEntity;
                    CurrentAccount = updatedAccountEntity;
                }
            }
        }
    }

    private async Task OnNewAccountAsync()
    {
        var dialog = new MainMenuAccountDialog(new AccountEntity
        {
            Username = "",
            Password = "",
            Email = "",
            Role = AccountRole.User
        });

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Account != null)
            {
                var newAccountSpec = await ApiService.ApiClient.AccountsPOSTAsync(dialog.Account.ToDto());

                var newEntity = newAccountSpec.ToEntity();
                Accounts.Add(newEntity);
                CurrentAccount = newEntity;
            }
        }
    }

    private async Task OnAssignCaptainAsync()
    {
        if (CurrentAccount == null) return;

        var dialog = new MainMenuCaptainDialog();
        if (dialog.ShowDialog() == true)
        {
            var captainSpec = new CaptainSpec
            {
                AccountId = CurrentAccount.Id,
                FirstName = dialog.FirstName,
                LastName = dialog.LastName,
                LicenseNumber = dialog.LicenseNumber
            };

            var newCaptainSpec = await ApiService.ApiClient.CaptainsPOSTAsync(captainSpec);
            var newCaptainEntity = newCaptainSpec.ToEntity();

            var accountSpec = new UpdateAccount
            {
                Id = CurrentAccount.Id,
                Username = CurrentAccount.Username,
                Email = CurrentAccount.Email,
                Password = "",
                Role = CurrentAccount.Role.ToString()
            };
            var updatedAccountSpec = await ApiService.ApiClient.AccountsPUTAsync(CurrentAccount.Id, accountSpec);
            var updatedAccountEntity = updatedAccountSpec.ToEntity();

            _captainsByAccountId[newCaptainEntity.AccountId] = newCaptainEntity;

            var index = Accounts.IndexOf(CurrentAccount);
            if (index != -1)
            {
                Accounts[index] = updatedAccountEntity;
            }

            CurrentAccount = updatedAccountEntity;

            Debug.WriteLine("Captain assignment successful.");
        }
    }

    private async Task OnRemoveCaptainAsync()
    {
        if (CurrentCaptain == null || CurrentAccount == null) return;

        var confirmed = ConfirmationRequested?.Invoke("Confirm Removal",
            $"Are you sure you want to remove {CurrentAccount.Username} as a captain? " +
            "This will revert their role to 'User'.") ?? false;
        if (!confirmed) return;

        await ApiService.ApiClient.CaptainsDELETEAsync(CurrentCaptain.Id);

        var accountSpec = new UpdateAccount
        {
            Id = CurrentAccount.Id,
            Username = CurrentAccount.Username,
            Email = CurrentAccount.Email,
            Password = "",
            Role = CurrentAccount.Role.ToString()
        };
        var updatedAccountSpec = await ApiService.ApiClient.AccountsPUTAsync(CurrentAccount.Id, accountSpec);

        _captainsByAccountId.Remove(CurrentAccount.Id);

        var index = Accounts.IndexOf(CurrentAccount);
        if (index != -1)
        {
            Accounts[index] = updatedAccountSpec.ToEntity();
        }

        CurrentAccount = updatedAccountSpec.ToEntity();

        Debug.WriteLine("Captain removal successful.");
    }

    private async Task OnRemoveAccountAsync()
    {
        if (CurrentAccount == null) return;

        var confirmed = ConfirmationRequested?.Invoke("Confirm Removal",
                            $"Are you sure you want to remove the account '{CurrentAccount.Username}'? This action cannot be undone.") ??
                        false;
        if (!confirmed) return;

        var accountToRemove = CurrentAccount;

        var index = Accounts.IndexOf(accountToRemove);

        await ApiService.ApiClient.AccountsDELETEAsync(accountToRemove.Id);

        Accounts.Remove(accountToRemove);

        CurrentAccount = Accounts.Any() ? Accounts[Math.Max(0, index - 1)] : null;

        Debug.WriteLine("Account removal successful.");
    }
}