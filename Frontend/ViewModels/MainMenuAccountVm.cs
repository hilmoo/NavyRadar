using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using Shared.Spec;

namespace Frontend.ViewModels
{
    public class MainMenuAccountsVm : ObservableObject
    {
        public ObservableCollection<Account> Accounts { get; } = [];

        public Account CurrentAccount
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        } = null!;
        
        public bool IsAdmin
        {
            get;
            init
            {
                field = value;
                OnPropertyChanged();
            }
        } = false;

        private bool IsLoading
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateAccountCommand { get; }
        public ICommand NewAccountCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainMenuAccountsVm()
        {
            UpdateAccountCommand = new SimpleRelayCommand(async void () =>
                {
                    try
                    {
                        await OnUpdateAccountAsync();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                    }
                },
                () => !IsLoading);
            NewAccountCommand = new SimpleRelayCommand(async void () =>
            {
                try
                {
                    await OnNewAccountAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            }, () => !IsLoading);
            RefreshCommand = new SimpleRelayCommand(async void () =>
            {
                try
                {
                    await LoadAccountsAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            }, () => !IsLoading);

            _ = LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            IsLoading = true;
            Accounts.Clear();

            try
            {
                var accountsFromServer = await ApiService.ApiClient.AccountsAllAsync();
                foreach (var account in accountsFromServer)
                {
                    Accounts.Add(account);
                }
            }
            catch (ApiException apiEx)
            {
                MessageBox.Show($"Could not load accounts. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                    "API Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnUpdateAccountAsync()
        {
            var accountToEdit = new Account
            {
                Id = CurrentAccount.Id,
                Username = CurrentAccount.Username,
                Email = CurrentAccount.Email,
                Password = CurrentAccount.Password,
                Role = CurrentAccount.Role
            };

            var dialog = new MainMenuAccountDialog(MapSpecToModel(accountToEdit));

            if (dialog.ShowDialog() == true)
            {
                IsLoading = true;
                try
                {
                    Debug.Assert(dialog.Account != null);
                    var updatedAccount =
                        await ApiService.ApiClient.AccountsPUTAsync(dialog.Account.Id, MapModelToSpec(dialog.Account));

                    // Find the original account in the collection and replace it
                    var index = Accounts.IndexOf(CurrentAccount);
                    if (index != -1)
                    {
                        Accounts[index] = updatedAccount;
                    }

                    Debug.WriteLine("Update successful.");
                }
                catch (ApiException apiEx)
                {
                    MessageBox.Show(
                        $"Could not update account. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                        "API Error");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task OnNewAccountAsync()
        {
            var dialog = new MainMenuAccountDialog(MapSpecToModel(new Account()));

            if (dialog.ShowDialog() == true)
            {
                IsLoading = true;
                try
                {
                    if (dialog.Account != null)
                    {
                        var newAccount = await ApiService.ApiClient.AccountsPOSTAsync(MapModelToSpec(dialog.Account));

                        Accounts.Add(newAccount);
                    }

                    Debug.WriteLine("Creation successful.");
                }
                catch (ApiException apiEx)
                {
                    MessageBox.Show(
                        $"Could not create account. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                        "API Error");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private static Shared.Models.Account MapSpecToModel(Account spec) =>
            new()
            {
                Id = spec.Id,
                Username = spec.Username,
                Email = spec.Email,
                Password = spec.Password,
                Role = spec.Role
            };

        private static Account MapModelToSpec(Shared.Models.Account model) =>
            new()
            {
                Id = model.Id,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };
    }
}