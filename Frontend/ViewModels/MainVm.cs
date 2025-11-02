using System.Windows.Input;
using Frontend.Util;
using Shared.Models;
using Shared.Util;

namespace Frontend.ViewModels;

public class MainVm : ViewModelBase
{
    public object CurrentView
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsShipsViewSelected));
            OnPropertyChanged(nameof(IsAccountsViewSelected));
            OnPropertyChanged(nameof(IsPortsViewSelected));
            OnPropertyChanged(nameof(IsMyAccountViewSelected));
        }
    }

    public ICommand ShipsCommand { get; }
    public ICommand AccountsCommand { get; }
    public ICommand PortsCommand { get; }
    public ICommand MyAccountCommand { get; }

    private void Ships(object? obj) => CurrentView = new MainMenuShipsVm();
    private void Accounts(object? obj) => CurrentView = new MainMenuAccountsVm();
    private void Ports(object? obj) => CurrentView = new MainMenuPortsVm();

    private void MyAccount(object? obj) =>
        CurrentView = new MainMenuMyAccountVm { CurrentAccount = CurrentAccount };

    public bool IsShipsViewSelected => CurrentView is MainMenuShipsVm;
    public bool IsAccountsViewSelected => CurrentView is MainMenuAccountsVm;
    public bool IsPortsViewSelected => CurrentView is MainMenuPortsVm;
    public bool IsMyAccountViewSelected => CurrentView is MainMenuMyAccountVm;

    private Account CurrentAccount { get; set; }

    public MainVm()
    {
        ShipsCommand = new RelayCommand(Ships);
        AccountsCommand = new RelayCommand(Accounts);
        PortsCommand = new RelayCommand(Ports);
        MyAccountCommand = new RelayCommand(MyAccount);

        CurrentView = new MainMenuShipsVm();

        CurrentAccount =
            new Account
            {
                Id = 3,
                Username = "deckhand",
                Password = "sailor",
                Email = "email2@mail.com",
                Role = nameof(RoleType.Admin)
            };
    }
}