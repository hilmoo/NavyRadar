using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Spec;
using AccountEntity = NavyRadar.Shared.Entities.Account;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuMyAccountVm : ViewModelBase
{
    public AccountEntity CurrentAccount
    {
        get;
        init
        {
            field = value;
            OnPropertyChanged();
        }
    } = null!;

    public NavigationVm NavigationVm { get; init; } = null!;

    public ICommand UpdateProfileCommand { get; }
    public ICommand SignOutCommand { get; }
    public Action OnSignOut { get; init; } = null!;


    public MainMenuMyAccountVm()
    {
        UpdateProfileCommand = new SimpleRelayCommand(
            void () => _ = ExecuteLoadingTask(OnUpdateProfileAsync),
            () => !IsLoading
        );
        SignOutCommand = new RelayCommand(SignOut);
    }

    private void SignOut(object? obj)
    {
        OnSignOut.Invoke();
    }

    private async Task OnUpdateProfileAsync()
    {
        var dialog = new MainMenuMyAccountDialog(CurrentAccount.Username, CurrentAccount.Email);

        if (dialog.ShowDialog() != true)
            return;

        var updated = new UpdateAccount
        {
            Id = CurrentAccount.Id,
            Username = dialog.UpdatedUsername,
            Email = dialog.UpdatedEmail,
            Password = string.IsNullOrWhiteSpace(dialog.UpdatedPassword)
                ? ""
                : dialog.UpdatedPassword,
            Role = ""
        };

        await ApiService.ApiClient.ProfileAsync(updated);

        CurrentAccount.Username = updated.Username;
        CurrentAccount.Email = updated.Email;
        CurrentAccount.Password = updated.Password;

        OnPropertyChanged(nameof(CurrentAccount));
    }
}