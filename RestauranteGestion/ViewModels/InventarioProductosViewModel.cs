using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;
using RestauranteGestion.Services;
using System.Threading.Tasks;
using System;
using RestauranteGestion.Core.DataAccess;

namespace RestauranteGestion.ViewModels
{
    public class ProductosViewModel : ViewModelBase
    {
        private readonly ProductoService _productoService;
     

        private ObservableCollection<Producto> _productos = new();
        private ObservableCollection<Producto> _filtradosProductos = new();
        private ObservableCollection<CategoriaProducto> _categorias = new();
        private Producto _selectedProducto;
        private CategoriaProducto _selectedCategoria;
        private string _filter;

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
                FiltrarProductos();
            }
        }

        public ObservableCollection<Producto> FiltradosProductos
        {
            get => _filtradosProductos;
            set
            {
                _filtradosProductos = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoriaProducto> Categorias
        {
            get => _categorias;
            set
            {
                _categorias = value;
                OnPropertyChanged();
            }
        }

        public CategoriaProducto SelectedCategoria
        {
            get => _selectedCategoria;
            set
            {
                _selectedCategoria = value;
                OnPropertyChanged();
                FiltrarProductos();
            }
        }

        public Producto SelectedProducto
        {
            get => _selectedProducto;
            set
            {
                if (_selectedProducto != value)
                {
                    _selectedProducto = value;
                    OnPropertyChanged();

                    // Notifica que deben reevaluarse los comandos
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }


        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged();
                FiltrarProductos();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ManageCategoriesCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public ProductosViewModel()
        {
            _productoService = new ProductoService();


            AddCommand = new RelayCommand(_ => ShowAddDialog());
            EditCommand = new RelayCommand(_ => ShowEditDialog(), _ => SelectedProducto != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteProducto(), _ => SelectedProducto != null);
            SearchCommand = new RelayCommand(_ => FiltrarProductos());
            ManageCategoriesCommand = new RelayCommand(_ => ExecuteManageCategories());
            ClearFiltersCommand = new RelayCommand(_ => ExecuteClearFilters());

            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            await LoadProductos();
            await LoadCategorias();
        }

        private async Task LoadProductos()
        {
            try
            {
                var resultado = await _productoService.ObtenerProductosAsync();
                Productos = new ObservableCollection<Producto>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar productos: {ex.Message}");
                // Aquí podrías mostrar un mensaje al usuario
            }
        }

        private async Task LoadCategorias()
        {
            try
            {
                var query = "SELECT * FROM rg_categoriaproducto";
                var categorias = await new DBOperacion()
                                        .QueryAsync<CategoriaProducto>(query);

                Categorias = new ObservableCollection<CategoriaProducto>(categorias);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar categorías: {ex.Message}");
            }
        }

        private void ExecuteManageCategories()
        {
          /*  var dialog = new CategoriasDialog();
            dialog.DataContext = new CategoriasDialogViewModel();
            DialogHost.Show(dialog, "MyDialogHost");*/
        }

        private void ExecuteClearFilters()
        {
            Filter = string.Empty;
            SelectedCategoria = null;
        }

        private void FiltrarProductos()
        {
            var query = Productos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filtro = Filter.ToLower();
                query = query.Where(p => p.NombreProducto.ToLower().Contains(filtro));
            }

            if (SelectedCategoria != null)
            {
                query = query.Where(p => p.IdCategoria == SelectedCategoria.IdCategoria);
            }

            FiltradosProductos = new ObservableCollection<Producto>(query);
        }

        private async void ShowAddDialog()
        {
            var dialog = new ProductoDialog();
            dialog.DataContext = new ProductoDialogViewModel();
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadProductos();
            }
        }

        private async void ShowEditDialog()
        {
            if (SelectedProducto == null) return;

            var dialog = new ProductoDialog();
            dialog.DataContext = new ProductoDialogViewModel(SelectedProducto);
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadProductos();
            }
        }

        private async Task DeleteProducto()
        {
            if (SelectedProducto == null) return;

            try
            {
                await _productoService.EliminarProductoAsync(SelectedProducto.IdProducto);
                await LoadProductos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                // Mostrar mensaje de error al usuario
            }
        }
    }
}