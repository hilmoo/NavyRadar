using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Frontend.Service;
using Frontend.ViewModels;

namespace Frontend
{
    public partial class MainWindow
    {
        private readonly JsBridge _jsBridge;
        
        public MainWindow(ShipViewModel viewModel, JsBridge jsBridge)
        {
            InitializeComponent();
            
            DataContext = viewModel;
            _jsBridge = jsBridge;
            
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await WebView.EnsureCoreWebView2Async();

                WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                WebView.CoreWebView2.AddHostObjectToScript("bridge", _jsBridge);

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
    }
}