using System.Diagnostics;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using Shared.Models;

namespace Frontend.ViewModels;

public class MainMenuMyAccountVm : ObservableObject
{
    public Account CurrentAccount
    {
        get;
        init
        {
            field = value;
            OnPropertyChanged();
        }
    } = null!;

    public ICommand UpdateProfileCommand { get; }

    public MainMenuMyAccountVm()
    {
        UpdateProfileCommand = new SimpleRelayCommand(OnUpdateProfile);
    }

    private void OnUpdateProfile()
    {
        var dialog = new MainMenuMyAccountDialog(CurrentAccount.Username, CurrentAccount.Email);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- PROFILE UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"New Username Input: {dialog.UpdatedUsername}");
            Debug.WriteLine($"New Email Input:    {dialog.UpdatedEmail}");
            Debug.WriteLine("------------------------");

            CurrentAccount.Username = dialog.UpdatedUsername;
            CurrentAccount.Email = dialog.UpdatedEmail;
        }
        else
        {
            Debug.WriteLine("--- PROFILE UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }
}