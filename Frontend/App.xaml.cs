using System.Windows;
using Frontend.Service;
using Frontend.Service.Api;
using Frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend
{
    public partial class App
    {
        private static IServiceProvider ServiceProvider { get; set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IShipApiService, ShipApiService>();
            services.AddSingleton<ShipViewModel>();
            
            services.AddTransient<JsBridge>();
            
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
