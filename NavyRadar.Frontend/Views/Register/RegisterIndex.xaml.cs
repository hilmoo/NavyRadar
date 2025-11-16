using System.Windows;
using System.Windows.Controls;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Register;

public partial class RegisterIndex
{
    public RegisterIndex()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterVm vm && sender is PasswordBox passwordBox)
        {
            vm.Password = passwordBox.Password;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not RegisterVm vm) return;
        vm.ErrorOccurred += ShowErrorMessage;
    }

    private static void ShowErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }


    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not RegisterVm vm) return;
        vm.ErrorOccurred -= ShowErrorMessage;
    }
}