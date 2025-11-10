using System.Windows;
using System.Windows.Controls;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Register;

public partial class RegisterIndex
{
    public RegisterIndex()
    {
        InitializeComponent();
    }
    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterVm vm && sender is PasswordBox passwordBox)
        {
            vm.Password = passwordBox.Password;
        }
    }
}