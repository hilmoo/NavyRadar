using System.Windows;
using System.Windows.Controls;
using Frontend.ViewModels;

namespace Frontend.Views.SignIn;

public partial class SignInIndex
{
    public SignInIndex()
    {
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is SignInVm vm && sender is PasswordBox passwordBox)
        {
            vm.Password = passwordBox.Password;
        }
    }
}