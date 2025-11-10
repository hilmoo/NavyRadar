using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Spec;

namespace NavyRadar.Frontend.ViewModels;

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
        OnSignOut.Invoke();
    }

    private async Task OnUpdateProfileAsync()
    {
        var dialog = new MainMenuMyAccountDialog(CurrentAccount.Username, CurrentAccount.Email);

        if (dialog.ShowDialog() != true)
            return;

        var updated = new UpdateAcc
        {
            Id = CurrentAccount.Id,
            Username = dialog.UpdatedUsername,
            Email = dialog.UpdatedEmail,
            Password = string.IsNullOrWhiteSpace(dialog.UpdatedPassword)
                ? ""
                : dialog.UpdatedPassword,
            Role = ""
        };

        IsLoading = true;
        try
        {
            await ApiService.ApiClient.ProfileAsync(updated);

            CurrentAccount.Username = updated.Username;
            CurrentAccount.Email = updated.Email;
            CurrentAccount.Password = updated.Password;

            OnPropertyChanged(nameof(CurrentAccount));
        }
        catch (ApiException apiEx)
        {
            Debug.WriteLine($"API Error: {apiEx.StatusCode} - {apiEx.Message}");
            MessageBox.Show(
                $"Could not update profile. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                "API Error");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected Error: {ex.Message}");
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
        }
        finally
        {
            IsLoading = false;
        }
    }
}