using System.Diagnostics;
using System.Windows;
using Frontend.ViewModels;

namespace Frontend.Views.SignIn;

public partial class SignInIndex
{
    public SignInIndex()
    {
        InitializeComponent();
    }

    private void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameInput.Text;
        var password = PasswordInput.Password;

        Debug.WriteLine($"Username: {username}");
        Debug.WriteLine($"Password: {password}");

        var vm = (NavigationVm)DataContext;
        vm.NavigateTo(new MainVm());
    }
}