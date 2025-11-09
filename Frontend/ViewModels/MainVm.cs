using System.Windows.Input;
using Frontend.Util;
using Shared.Models;

namespace Frontend.ViewModels;

public class MainVm : ViewModelBase
{
    private readonly NavigationVm _navigationVm;

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

    private void Ships(object? obj) =>
        CurrentView = new MainMenuShipsVm { IsAdmin = _navigationVm.CurrentAccount!.Role == "Admin" };

    private void Accounts(object? obj) => CurrentView = new MainMenuAccountsVm
        { IsAdmin = _navigationVm.CurrentAccount!.Role == "Admin" };

    private void Ports(object? obj) =>
        CurrentView = new MainMenuPortsVm { IsAdmin = _navigationVm.CurrentAccount!.Role == "Admin" };

    private void MyAccount(object? obj) =>
        CurrentView = new MainMenuMyAccountVm
        {
            CurrentAccount = MapModelToSpec(_navigationVm.CurrentAccount!),
            NavigationVm = _navigationVm,
            OnSignOut = HandleSignOut
        };

    public bool IsShipsViewSelected => CurrentView is MainMenuShipsVm;
    public bool IsAccountsViewSelected => CurrentView is MainMenuAccountsVm;
    public bool IsPortsViewSelected => CurrentView is MainMenuPortsVm;
    public bool IsMyAccountViewSelected => CurrentView is MainMenuMyAccountVm;
    public bool IsAdmin => _navigationVm.CurrentAccount?.Role == "Admin";

    public MainVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;
        ShipsCommand = new RelayCommand(Ships);
        AccountsCommand = new RelayCommand(Accounts);
        PortsCommand = new RelayCommand(Ports);
        MyAccountCommand = new RelayCommand(MyAccount);

        CurrentView = new MainMenuShipsVm
        {
            IsAdmin = _navigationVm.CurrentAccount!.Role == "Admin"
        };
    }

    private void HandleSignOut()
    {
        ApiService.ApiClient.ClearJwtToken();
        _navigationVm.SignOut();
    }

    private static Shared.Spec.Account MapModelToSpec(Account model) =>
        new()
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            Role = model.Role
        };
}