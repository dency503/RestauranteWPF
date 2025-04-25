using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;

#nullable enable

namespace RestauranteGestion.ViewModels
{
    public class EdicionRecetaViewModel : ViewModelBase
    {
        private RelayCommand _addIngredienteCommand;
        private readonly INavigationService _navigationService;
        public ICommand AddIngredienteCommand => _addIngredienteCommand;
        private readonly ProductoService _productoService;
        private readonly IngredienteService _ingredienteService;
        private readonly RecetaService _recetaService;

        private ObservableCollection<Producto> _productos = new();
        private ObservableCollection<Ingrediente> _ingredientesDisponibles = new();
        private ObservableCollection<IngredienteReceta> _ingredientesReceta = new();
        public ObservableCollection<Unidad> Unidades { get; set; }


        private Producto? _selectedProducto;
        private Ingrediente? _nuevoIngrediente;
        private string _nuevaCantidad = string.Empty;
        private Unidad? _nuevaUnidad;
        private string _mensajeEstado = string.Empty;

        public string DialogTitle { get; set; } = "Editor de Recetas";

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Ingrediente> IngredientesDisponibles
        {
            get => _ingredientesDisponibles;
            set
            {
                _ingredientesDisponibles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IngredienteReceta> IngredientesReceta
        {
            get => _ingredientesReceta;
            set
            {
                _ingredientesReceta = value;
                OnPropertyChanged();
            }
        }

        public Producto? SelectedProducto
        {
            get => _selectedProducto;
            set
            {
                if (_selectedProducto != value)
                {
                    _selectedProducto = value;
                    OnPropertyChanged();
                   _ = CargarRecetaAsync();
                }
            }
        }

        public Ingrediente? NuevoIngrediente
        {
            get => _nuevoIngrediente;
            set
            {
                _nuevoIngrediente = value;
                OnPropertyChanged();
                _addIngredienteCommand.RaiseCanExecuteChanged();
            }
        }

        public string NuevaCantidad
        {
            get => _nuevaCantidad;
            set
            {
                _nuevaCantidad = value;
                OnPropertyChanged();
                _addIngredienteCommand.RaiseCanExecuteChanged();
            }
        }

        public Unidad? NuevaUnidad
        {
            get => _nuevaUnidad;
            set
            {
                _nuevaUnidad = value;
                OnPropertyChanged();
                _addIngredienteCommand.RaiseCanExecuteChanged();
            }
        }
        public string MensajeEstado
        {
            get => _mensajeEstado;
            set
            {
                _mensajeEstado = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadRecetaCommand { get; }

        public ICommand ClearFormCommand { get; }
        public ICommand SaveRecetaCommand { get; }
        public ICommand CancelCommand { get; }

        public EdicionRecetaViewModel()
        {
            Unidades = new ObservableCollection<Unidad>
    {
        new Unidad { Nombre = "Unidad" },
        new Unidad { Nombre = "Gramo" },
        new Unidad { Nombre = "Litro" },
        new Unidad { Nombre = "Kilogramo" }
    };
            _navigationService = ServiceLocator.NavigationService;
            _productoService = new ProductoService();
            _ingredienteService = new IngredienteService();
            _recetaService = new RecetaService();

            LoadRecetaCommand = new RelayCommand(async _ => await CargarRecetaAsync());
            _addIngredienteCommand = new RelayCommand(
                async _ => await AgregarIngredienteAsync(),
                _ => NuevoIngrediente != null && !string.IsNullOrEmpty(NuevaCantidad) && NuevaUnidad != null
            );
            ClearFormCommand = new RelayCommand(_ => LimpiarFormulario());
            SaveRecetaCommand = new RelayCommand(async _ => await GuardarRecetaAsync());
            CancelCommand = new RelayCommand(_ => CancelarEdicion());

            _ = CargarProductosAsync();
            _ = CargarIngredientesDisponiblesAsync();
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                var productos = await _productoService.ObtenerProductosAsync();
                Productos = new ObservableCollection<Producto>(productos);
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar productos: {ex.Message}";
            }
        }

        private async Task CargarIngredientesDisponiblesAsync()
        {
            try
            {
                var ingredientes = await _ingredienteService.GetIngredientesAsync();
                IngredientesDisponibles = new ObservableCollection<Ingrediente>(ingredientes);
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar ingredientes: {ex.Message}";
            }
        }

        private async Task CargarRecetaAsync()
        {
            if (SelectedProducto == null)
            {
                return;
            }

            try
            {
                var receta = await _recetaService.ObtenerRecetaPorProductoAsync(SelectedProducto.IdProducto);
                IngredientesReceta = new ObservableCollection<IngredienteReceta>(receta);
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar receta: {ex.Message}";
            }
        }

        private async Task AgregarIngredienteAsync()
        {
            if (NuevoIngrediente == null || string.IsNullOrEmpty(NuevaCantidad) || NuevaUnidad == null)
            {
                MensajeEstado = "Faltan datos para agregar ingrediente.";
                return;
            }

            try
            {
                var ingredienteReceta = new IngredienteReceta
                {
                    Ingrediente = NuevoIngrediente,
                    Cantidad = Convert.ToDecimal(NuevaCantidad),
                    Unidad = NuevaUnidad
                };
                IngredientesReceta.Add(ingredienteReceta);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al agregar ingrediente: {ex.Message}";
            }
        }

        private void LimpiarFormulario()
        {
            NuevoIngrediente = null;
            NuevaCantidad = string.Empty;
            NuevaUnidad = null;
        }

        private async Task GuardarRecetaAsync()
        {
            if (SelectedProducto == null || !IngredientesReceta.Any())
            {
                MensajeEstado = "Debe seleccionar un producto y agregar ingredientes.";
                return;
            }

            try
            {
                await _recetaService.GuardarRecetaAsync(SelectedProducto.IdProducto, IngredientesReceta);
                MensajeEstado = "Receta guardada con éxito.";
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al guardar receta: {ex.Message}";
            }
        }

        private void CancelarEdicion()
        {
           _navigationService.CloseCurrentView(); 
        }
    }
    public class IngredienteReceta
    {
        public Ingrediente Ingrediente { get; set; }
        public decimal Cantidad { get; set; }
        public Unidad Unidad { get; set; }
    }

    public class Unidad
    {
        public string Nombre { get; set; }
    }
}
