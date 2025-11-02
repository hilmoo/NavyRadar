using System.Diagnostics;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using Shared.Models;

namespace Frontend.ViewModels;

public class MainMenuAccountsVm : ObservableObject
{
    public List<Account>? Accounts { get; private set; }

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

    public ICommand UpdateAccountCommand { get; }
    public ICommand NewAccountCommand { get; }

    public MainMenuAccountsVm()
    {
        LoadDummyData();
        UpdateAccountCommand = new SimpleRelayCommand(OnUpdateAccount);
        NewAccountCommand = new SimpleRelayCommand(OnNewAccount);
    }

    private void OnUpdateAccount()
    {
        Debug.WriteLine(CurrentAccount);
        var dialog = new MainMenuAccountDialog(CurrentAccount);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:       {dialog.Account!.Id}");
            Debug.WriteLine($"Username: {dialog.Account.Username}");
            Debug.WriteLine($"Email:    {dialog.Account.Email}");
            Debug.WriteLine($"Password: {dialog.Account.Password}");
            Debug.WriteLine($"Role:     {dialog.Account.Role}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }

    private static void OnNewAccount()
    {
        var dialog = new MainMenuAccountDialog(null);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:       {dialog.Account!.Id}");
            Debug.WriteLine($"Username: {dialog.Account.Username}");
            Debug.WriteLine($"Email:    {dialog.Account.Email}");
            Debug.WriteLine($"Password: {dialog.Account.Password}");
            Debug.WriteLine($"Role:     {dialog.Account.Role}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }

    private void LoadDummyData()
    {
        Accounts =
        [
            new Account
            {
                Id = 1,
                Username = "captain_jack",
                Password = "blackpearl",
                Email = "email@mail.com",
                Role = "Admin"
            },
            new Account
            {
                Id = 2,
                Username = "first_mate",
                Password = "seadog",
                Email = "email1@mail.com",
                Role = "User"
            },
            new Account
            {
                Id = 3,
                Username = "deckhand",
                Password = "sailor",
                Email = "email2@mail.com",
                Role = "User"
            }
        ];
    }
}