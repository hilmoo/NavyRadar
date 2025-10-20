using System.Diagnostics;
using System.Windows;
using Frontend.ViewModels;

namespace Frontend.Views.Register;

public partial class RegisterIndex
{
    public RegisterIndex()
    {
        InitializeComponent();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameInput.Text;
        var email = EmailInput.Text;
        var password = PasswordInput.Password;


        Debug.WriteLine($"Username: {username}");
        Debug.WriteLine($"Username: {email}");
        Debug.WriteLine($"Password: {password}");

        var vm = (NavigationVm)DataContext;
        vm.NavigateTo(new MainVm());
    }
}