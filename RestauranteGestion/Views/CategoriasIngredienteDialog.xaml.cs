using System.Windows;
using System.Windows.Controls;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels;

namespace RestauranteGestion.Views
{
    public partial class CategoriasIngredienteView : UserControl
    {
        private readonly INavigationService _navigationService;
        public CategoriasIngredienteView()
        {

            _navigationService = ServiceLocator.NavigationService;
            InitializeComponent(); // <-- aquí es donde aparece el error si algo está mal
            DataContext = new CategoriasIngredienteViewModel();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
         
            _navigationService.GoBack();

        }
    }
}

