using System.Windows;
using System.Windows.Controls;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Main;

public partial class MenuShips
{
    public MenuShips()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuShipsVm vm) return;
        vm.ErrorOccurred += ShowErrorMessage;
        vm.ConfirmationRequested += ShowConfirmationDialog;
    }

    private static void ShowErrorMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static bool ShowConfirmationDialog(string title, string message)
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        return result == MessageBoxResult.Yes;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuShipsVm vm) return;
        vm.ErrorOccurred -= ShowErrorMessage;
        vm.ConfirmationRequested -= ShowConfirmationDialog;
    }
}