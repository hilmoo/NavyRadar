using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Shared.Models;

namespace NavyRadar.Frontend.ViewModels;

public class NavigationVm : ViewModelBase
{
    private readonly HomeVm _homeVm;
    private readonly SignInVm _signInVm;
    private readonly RegisterVm _registerVm;

    public object CurrentView
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public Account? CurrentAccount { get; set; }

    public ICommand HomeCommand { get; set; }
    public ICommand SignInCommand { get; set; }
    public ICommand RegisterCommand { get; set; }
    public ICommand MainCommand { get; set; }


    private void Home(object? obj) => CurrentView = _homeVm;
    private void SignIn(object? obj) => CurrentView = _signInVm;
    private void Register(object? obj) => CurrentView = _registerVm;

    private void Main(object? obj)
    {
        if (CurrentAccount != null)
        {
            NavigateToMain(CurrentAccount);
        }
    }


    public NavigationVm()
    {
        _homeVm = new HomeVm();
        _signInVm = new SignInVm(this);
        _registerVm = new RegisterVm(this);

        HomeCommand = new RelayCommand(Home);
        SignInCommand = new RelayCommand(SignIn);
        RegisterCommand = new RelayCommand(Register);
        MainCommand = new RelayCommand(Main);

        CurrentView = new HomeVm();
    }

    public void NavigateToHome()
    {
        CurrentView = _homeVm;
    }

    public void SignOut()
    {
        CurrentAccount = null;
        CurrentView = _homeVm;
    }

    public void NavigateToMain(Account userAccount)
    {
        CurrentAccount = userAccount;
        var mainVm = new MainVm(this);
        CurrentView = mainVm;
    }
}