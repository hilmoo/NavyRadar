using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await WebView.EnsureCoreWebView2Async();

            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            string html;
            var uri = new Uri("pack://application:,,,/Assets/index.html");
            await using (var s = Application.GetResourceStream(uri)!.Stream)
            using (var r = new StreamReader(s, Encoding.UTF8))
                html = await r.ReadToEndAsync();
            WebView.NavigateToString(html);
        }
    }
}