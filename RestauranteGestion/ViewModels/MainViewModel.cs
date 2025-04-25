using System.Windows;
using System.Windows.Input;
using RestauranteGestion;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private bool _isSidebarVisible;

    public bool IsSidebarVisible
    {
        get => _isSidebarVisible;
        set { _isSidebarVisible = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        IsSidebarVisible = false;
        _navigationService = ServiceLocator.NavigationService;
        ServiceLocator.MainViewModel = this;
        // Registrar el MainViewModel en el ServiceLocator
        EventBus.LoginSucceeded += () => IsSidebarVisible = true;
    }

    private ICommand _navigateCommand;
    public ICommand NavigateCommand => _navigateCommand ??= new RelayCommand<string>(NavigateTo);

    private void NavigateTo(string viewName)
    {
        switch (viewName)
        {
            case "DashboardView":
                _navigationService.NavigateTo<DashboardView>();
                break;
            case "InventarioProductosView":
                _navigationService.NavigateTo<InventarioProductosView>();
                break;
            case "InventarioIngredientesView":
                _navigationService.NavigateTo<InventarioIngredientesView>();
                break;
            case "VentasView":
                _navigationService.NavigateTo<VentaView>();
                break;
            case "MesasView":
                _navigationService.NavigateTo<MesasView>();
                break;
            case "RecetasView":
                _navigationService.NavigateTo<InventarioRecetasView>();
                break;
            case "PerfilView":
                _navigationService.NavigateTo<PerfilView>();
                break;
            default:
                MessageBox.Show($"Vista '{viewName}' no implementada");
                break;
        }
    }

    public void SetSidebarVisibility(bool isVisible)
    {
        IsSidebarVisible = isVisible;
    }
}
