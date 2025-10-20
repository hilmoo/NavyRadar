using System.IO;
using System.Text;
using System.Windows;

namespace Frontend.Views.Home
{
    public partial class HomeIndex
    {
        public HomeIndex()
        {
            InitializeComponent();

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }
    }
}