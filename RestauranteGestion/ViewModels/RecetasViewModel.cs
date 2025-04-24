using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;

namespace RestauranteGestion.ViewModels
{
    public class RecetasViewModel : ViewModelBase
    {
        private readonly RecetaService _recetaService;

        private readonly INavigationService _navigationService;
        private ObservableCollection<Receta> _recetas = new();
        private ObservableCollection<Receta> _filtradasRecetas = new();
        private ObservableCollection<Producto> _productos = new();
        private Receta _selectedReceta;
        private Producto _selectedProducto;
        private string _filter = "";
        public ICommand ManageIngredientesCommand { get; }

        public ObservableCollection<Receta> Recetas
        {
            get => _recetas;
            set
            {
                _recetas = value;
                OnPropertyChanged();
                FiltrarRecetas();
            }
        }

        public ObservableCollection<Receta> FiltradasRecetas
        {
            get => _filtradasRecetas;
            set
            {
                _filtradasRecetas = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
            }
        }

        public Receta SelectedReceta
        {
            get => _selectedReceta;
            set
            {
                _selectedReceta = value;
                OnPropertyChanged();
                // Notifica que deben reevaluarse los comandos
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public Producto SelectedProducto
        {
            get => _selectedProducto;
            set
            {
                _selectedProducto = value;
                OnPropertyChanged();
                FiltrarRecetas();
            }
        }

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged();
                FiltrarRecetas();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public RecetasViewModel()
        {
            _recetaService = new RecetaService();
            _navigationService = ServiceLocator.NavigationService;
            AddCommand = new RelayCommand(_ => ShowAddDialog());
            EditCommand = new RelayCommand(_ => ShowEditDialog(), _ => SelectedReceta != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteReceta(), _ => SelectedReceta != null);
            SearchCommand = new RelayCommand(_ => FiltrarRecetas());
            ClearFiltersCommand = new RelayCommand(_ => ExecuteClearFilters());

            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            await LoadRecetas();
            await LoadProductos();
        }

        private async Task LoadRecetas()
        {
            try
            {
                var resultado = await _recetaService.ObtenerRecetasAsync();
                Recetas = new ObservableCollection<Receta>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar recetas: {ex.Message}");
            }
        }

        private async Task LoadProductos()
        {
            try
            {
                var productos = await _recetaService.ObtenerProductosAsync();
                Productos = new ObservableCollection<Producto>(productos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar productos: {ex.Message}");
            }
        }

        private void ExecuteClearFilters()
        {
            Filter = string.Empty;
            SelectedProducto = null;
        }

        private void FiltrarRecetas()
        {
            var query = Recetas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filtro = Filter.ToLower();
                query = query.Where(r => r.Producto?.NombreProducto.ToLower().Contains(filtro) == true);
            }

            if (SelectedProducto != null)
            {
                query = query.Where(r => r.IdProducto == SelectedProducto.IdProducto);
            }

            FiltradasRecetas = new ObservableCollection<Receta>(query);
        }

        private async void ShowAddDialog()
        {
            _navigationService.NavigateTo<EdicionRecetaView>();
            /*var dialog = new RecetaDialog();
            dialog.DataContext = new RecetaDialogViewModel();
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadRecetas();
            }*/
        }

        private async void ShowEditDialog()
        {
            if (SelectedReceta == null) return;

           /* var dialog = new RecetaDialog();
            dialog.DataContext = new RecetaDialogViewModel(SelectedReceta);
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadRecetas();
            }*/
        }

        private async Task DeleteReceta()
        {
            if (SelectedReceta == null) return;

            try
            {
                await _recetaService.EliminarRecetaAsync(SelectedReceta.IdReceta);
                await LoadRecetas();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar receta: {ex.Message}");
            }
        }
    }
}
