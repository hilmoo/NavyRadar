using System.Windows.Input;
using Frontend.Util;

namespace Frontend.ViewModels;

public class MainVm : ViewModelBase
{
    private object _currentView = null!;

    public object CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsShipsViewSelected));
            OnPropertyChanged(nameof(IsUsersViewSelected));
            OnPropertyChanged(nameof(IsSailsViewSelected));
            OnPropertyChanged(nameof(IsMyAccountViewSelected));
        }
    }

    public ICommand ShipsCommand { get; }
    public ICommand UsersCommand { get; }
    public ICommand SailsCommand { get; }
    public ICommand MyAccountCommand { get; }

    private void Ships(object? obj) => CurrentView = new MainMenuShipsVm();
    private void Users(object? obj) => CurrentView = new MainMenuUsersVm();
    private void Sails(object? obj) => CurrentView = new MainMenuPortsVm();
    private void MyAccount(object? obj) => CurrentView = new MainMenuMyAccountVm();

    public bool IsShipsViewSelected => CurrentView is MainMenuShipsVm;
    public bool IsUsersViewSelected => CurrentView is MainMenuUsersVm;
    public bool IsSailsViewSelected => CurrentView is MainMenuPortsVm;
    public bool IsMyAccountViewSelected => CurrentView is MainMenuMyAccountVm;

    public MainVm()
    {
        ShipsCommand = new RelayCommand(Ships);
        UsersCommand = new RelayCommand(Users);
        SailsCommand = new RelayCommand(Sails);
        MyAccountCommand = new RelayCommand(MyAccount);

        CurrentView = new MainMenuShipsVm();
    }
}