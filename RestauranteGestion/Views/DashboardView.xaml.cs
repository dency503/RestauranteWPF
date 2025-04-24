using System.Windows;
using System.Windows.Controls;
using RestauranteGestion.ViewModels;

namespace RestauranteGestion.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            this.DataContext = new DashboardViewModel(); // ← conecta el ViewModel
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DashboardViewModel vm && vm.CargarDashboardCommand.CanExecute(null))
            {
                vm.CargarDashboardCommand.Execute(null);
            }
        }
    }
}
