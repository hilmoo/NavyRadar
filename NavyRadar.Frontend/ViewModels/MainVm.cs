using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Frontend.ViewModels;

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
            OnPropertyChanged(nameof(IsSailsViewSelected));
            OnPropertyChanged(nameof(IsCaptainViewSelected));
        }
    }

    public ICommand BackToHomeCommand { get; }
    public ICommand ShipsCommand { get; }
    public ICommand AccountsCommand { get; }
    public ICommand PortsCommand { get; }
    public ICommand MyAccountCommand { get; }
    public ICommand SailsCommand { get; }
    public ICommand CaptainCommand { get; }

    private void Ships(object? obj) =>
        CurrentView = new MainMenuShipsVm { IsAdmin = IsAdmin };

    private void Accounts(object? obj) => CurrentView = new MainMenuAccountsVm();

    private void Ports(object? obj) =>
        CurrentView = new MainMenuPortsVm { IsAdmin = IsAdmin };

    private void MyAccount(object? obj) =>
        CurrentView = new MainMenuMyAccountVm
        {
            CurrentAccount = _navigationVm.CurrentAccount!,
            NavigationVm = _navigationVm,
            OnSignOut = HandleSignOut
        };

    private void Sails(object? obj) =>
        CurrentView = new MainMenuSailsVm
        {
            IsAdmin = IsAdmin
        };

    private void Captain(object? obj) =>
        CurrentView = new MainMenuSailingVm
        {
            CurrentAccount = _navigationVm.CurrentAccount!
        };

    public bool IsShipsViewSelected => CurrentView is MainMenuShipsVm;
    public bool IsAccountsViewSelected => CurrentView is MainMenuAccountsVm;
    public bool IsPortsViewSelected => CurrentView is MainMenuPortsVm;
    public bool IsMyAccountViewSelected => CurrentView is MainMenuMyAccountVm;
    public bool IsSailsViewSelected => CurrentView is MainMenuSailsVm;
    public bool IsCaptainViewSelected => CurrentView is MainMenuSailingVm;
    public bool IsAdmin => _navigationVm.CurrentAccount!.Role == AccountRole.Admin;
    public bool IsCaptain => _navigationVm.CurrentAccount!.Role == AccountRole.Captain;
    public bool IsNotCaptain => !IsCaptain;

    public MainVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;
        ShipsCommand = new RelayCommand(Ships);
        AccountsCommand = new RelayCommand(Accounts);
        PortsCommand = new RelayCommand(Ports);
        MyAccountCommand = new RelayCommand(MyAccount);
        SailsCommand = new RelayCommand(Sails);
        BackToHomeCommand = new RelayCommand(BackToHome);
        CaptainCommand = new RelayCommand(Captain);

        if (IsNotCaptain)
        {
            CurrentView = new MainMenuShipsVm
            {
                IsAdmin = IsAdmin
            };
        }
        else
        {
            CurrentView = new MainMenuSailingVm();
        }
    }

    private void HandleSignOut()
    {
        ApiService.ApiClient.ClearJwtToken();
        _navigationVm.SignOut();
    }

    private void BackToHome(object? obj)
    {
        _navigationVm.NavigateToHome();
    }
}