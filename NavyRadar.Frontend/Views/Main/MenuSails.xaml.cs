using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Main;

public partial class MenuSails
{
    public MenuSails()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuSailsVm vm) return;
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
        if (DataContext is not MainMenuSailsVm vm) return;
        vm.ErrorOccurred -= ShowErrorMessage;
        vm.ConfirmationRequested -= ShowConfirmationDialog;
    }

    private async void WebView_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not WebView2 webView) return;

            if (DataContext is not MainMenuSailsVm vm) return;

            try
            {
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                string html;
                var uri = new Uri("pack://application:,,,/Assets/position.html");

                await using (var s = Application.GetResourceStream(uri)!.Stream)
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    html = await r.ReadToEndAsync();
                }

                webView.NavigateToString(html);

                webView.NavigationCompleted += (o, args) => WebView_NavigationCompleted(o, args, vm.CurrentSail.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing map: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private static async void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e,
        int sailId)
    {
        try
        {
            if (!e.IsSuccess) return;
            if (sender is not WebView2 webView) return;

            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync($"renderShipHistory('{sailId}')");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Script execution failed: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while rendering ship history: {ex.Message}");
        }
    }
}