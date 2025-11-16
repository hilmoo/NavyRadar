using System.Windows;
using System.Windows.Controls;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Main;

public partial class MenuMyAccount
{
    public MenuMyAccount()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuMyAccountVm vm) return;
        vm.ErrorOccurred += ShowErrorMessage;
    }

    private static void ShowErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuAccountsVm vm) return;
        vm.ErrorOccurred -= ShowErrorMessage;
    }
}