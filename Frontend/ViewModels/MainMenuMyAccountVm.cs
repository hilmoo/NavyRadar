using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using Shared.Spec;

namespace Frontend.ViewModels
{
    public class MainMenuMyAccountVm : ObservableObject
    {
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

        public Account CurrentAccount
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
        public ICommand BackToHomeCommand { get; }
        public ICommand SignOutCommand { get; }
        public Action OnSignOut { get; init; } = null!;
        

        public MainMenuMyAccountVm()
        {
            UpdateProfileCommand = new SimpleRelayCommand(
                async void () =>
                {
                    try
                    {
                        await OnUpdateProfileAsync();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                    }
                },
                () => !IsLoading
            );
            BackToHomeCommand = new RelayCommand(BackToHome);
            SignOutCommand = new RelayCommand(SignOut);
        }

        private void BackToHome(object? obj)
        {
            NavigationVm.NavigateToHome();
        }
        
        private void SignOut(object? obj)
        {
            OnSignOut?.Invoke();
        }

        private async Task OnUpdateProfileAsync()
        {
            var dialog = new MainMenuMyAccountDialog(CurrentAccount.Username, CurrentAccount.Email);

            if (dialog.ShowDialog() == true)
            {
                var oldUsername = CurrentAccount.Username;
                var oldEmail = CurrentAccount.Email;

                CurrentAccount.Username = dialog.UpdatedUsername;
                CurrentAccount.Email = dialog.UpdatedEmail;

                IsLoading = true;
                try
                {
                    await ApiService.ApiClient.AccountsPUTAsync(CurrentAccount.Id, CurrentAccount);
                }
                catch (ApiException apiEx)
                {
                    Debug.WriteLine($"API Error: {apiEx.StatusCode} - {apiEx.Message}");
                    MessageBox.Show(
                        $"Could not update profile. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                        "API Error");

                    CurrentAccount.Username = oldUsername;
                    CurrentAccount.Email = oldEmail;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected Error: {ex.Message}");
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");

                    CurrentAccount.Username = oldUsername;
                    CurrentAccount.Email = oldEmail;
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}