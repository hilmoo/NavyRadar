using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using NavyRadar.Frontend.ViewModels;

namespace NavyRadar.Frontend.Views.Main;

public partial class MenuSailing
{
    private WebView2? _webView;
    private bool _isMapInitialized;

    public MenuSailing()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuSailingVm vm) return;

        vm.ErrorOccurred += ShowErrorMessage;
        vm.ConfirmationRequested += ShowConfirmationDialog;
        vm.PropertyChanged += OnViewModelPropertyChanged;

        if (vm.CurrentSail != null && _webView != null)
        {
            _ = InitializeMapAsync();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainMenuSailingVm vm) return;

        vm.ErrorOccurred -= ShowErrorMessage;
        vm.ConfirmationRequested -= ShowConfirmationDialog;
        vm.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainMenuSailingVm.CurrentSail))
        {
            Dispatcher.Invoke(() => _ = InitializeMapAsync());
        }
    }

    private void WebView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not WebView2 webView) return;
        _webView = webView;
        _ = InitializeMapAsync();
    }

    private async Task InitializeMapAsync()
    {
        try
        {
            if (DataContext is not MainMenuSailingVm vm) return;
            if (vm.CurrentSail == null) return;
            if (_webView == null) return;

            if (_isMapInitialized) return;

            try
            {
                await _webView.EnsureCoreWebView2Async();
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                string html;
                var uri = new Uri("pack://application:,,,/Assets/position.html");

                await using (var s = Application.GetResourceStream(uri)!.Stream)
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    html = await r.ReadToEndAsync();
                }

                // Hook up the navigation completed event BEFORE navigating
                // We use a local function or lambda to capture the specific Sail ID
                int currentSailId = vm.CurrentSail.Id;
                _webView.NavigationCompleted -= OnNavigationCompleted; // Remove old handlers if any
                _webView.NavigationCompleted += OnNavigationCompleted;

                _webView.NavigateToString(html);

                _isMapInitialized = true;

                // Local handler to avoid closure issues with changing IDs
                void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs args)
                {
                    WebView_NavigationCompleted(sender, args, currentSailId);
                    // Optional: Unsubscribe if you only ever render once
                    // ((WebView2)sender).NavigationCompleted -= OnNavigationCompleted; 
                }
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
}