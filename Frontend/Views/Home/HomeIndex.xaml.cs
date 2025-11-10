using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Frontend.ViewModels;

namespace Frontend.Views.Home
{
    public partial class HomeIndex
    {
        private readonly HomeVm _viewModel;

        public HomeIndex()
        {
            InitializeComponent();

            _viewModel = new HomeVm();
            DataContext = _viewModel;
            _viewModel.SendMessageToWebViewRequest += OnSendMessageToWebView;

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await WebView.EnsureCoreWebView2Async();

                WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

                string html;
                var uri = new Uri("pack://application:,,,/Assets/index.html");
                await using (var s = Application.GetResourceStream(uri)!.Stream)
                using (var r = new StreamReader(s, Encoding.UTF8))
                    html = await r.ReadToEndAsync();
                WebView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private void OnWebMessageReceived(object? sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            _viewModel.ProcessWebMessage(e.WebMessageAsJson);
        }

        private async void OnSendMessageToWebView(string jsonMessage)
        {
            try
            {
                if (WebView?.CoreWebView2 == null)
                {
                    Debug.WriteLine("WebView not ready to receive messages.");
                    return;
                }

                try
                {
                    await WebView.CoreWebView2.ExecuteScriptAsync($"filterShip({jsonMessage})");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send message to WebView: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred while sending message to WebView: {e.Message}");
            }
        }
    }
}