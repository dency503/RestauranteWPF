using System.Configuration;
using System.Data;
using System.Windows;
using RestauranteGestion.Services;
using RestauranteGestion.Views;

namespace RestauranteGestion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)

        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            var navigationService = new NavigationService(mainWindow.MainContentControl); // <- esto debe ser un ContentControl con x:Name
            ServiceLocator.NavigationService = navigationService;
            navigationService.NavigateTo<DashboardView>();
            mainWindow.DataContext = new MainViewModel(); // <- ya podrá usar el NavigationService
            mainWindow.Show();
        }



    }

}
