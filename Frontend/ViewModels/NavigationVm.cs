using Frontend.Util;
using System.Windows.Input;

namespace Frontend.ViewModels;

public class NavigationVm : ViewModelBase
{
    public object CurrentView
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public ICommand HomeCommand { get; set; }
    public ICommand SignInCommand { get; set; }
    public ICommand RegisterCommand { get; set; }
    public ICommand MainCommand { get; set; }


    private void Home(object? obj) => CurrentView = new HomeVm();
    private void SignIn(object? obj) => CurrentView = new SignInVm();
    private void Register(object? obj) => CurrentView = new RegisterVm();
    private void Main(object? obj) => CurrentView = new MainVm();


    public NavigationVm()
    {
        HomeCommand = new RelayCommand(Home);
        SignInCommand = new RelayCommand(SignIn);
        RegisterCommand = new RelayCommand(Register);
        MainCommand = new RelayCommand(Main);

        // Startup Page
        CurrentView = new MainVm();
    }

    public void NavigateTo(object viewModel)
    {
        CurrentView = viewModel;
    }
}