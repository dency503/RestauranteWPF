using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;

#nullable enable

namespace RestauranteGestion.ViewModels
{
    public class IngredientesViewModel : ViewModelBase
    {
        private readonly IngredienteService _ingredienteService;
        private readonly CategoriaIngredienteService _categoriaService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<Ingrediente> _ingredientes = new();
        private ObservableCollection<Ingrediente> _filtradosIngredientes = new();
        private ObservableCollection<CategoriaIngrediente> _categorias = new();

        private Ingrediente? _selectedIngrediente;
        private CategoriaIngrediente? _selectedCategoria;
        private string _filter = string.Empty;

        public string DialogTitle { get; set; } = "Gestión de Ingredientes";

        public ObservableCollection<Ingrediente> Ingredientes
        {
            get => _ingredientes;
            set
            {
                _ingredientes = value;
                OnPropertyChanged();
                FiltrarIngredientes();
            }
        }

        public ObservableCollection<Ingrediente> FiltradosIngredientes
        {
            get => _filtradosIngredientes;
            set
            {
                _filtradosIngredientes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoriaIngrediente> Categorias
        {
            get => _categorias;
            set
            {
                _categorias = value;
                OnPropertyChanged();
            }
        }

        public CategoriaIngrediente? SelectedCategoria
        {
            get => _selectedCategoria;
            set
            {
                if (_selectedCategoria != value)
                {
                    _selectedCategoria = value;
                    OnPropertyChanged();
                    FiltrarIngredientes();
                    // Notifica que deben reevaluarse los comandos
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public Ingrediente? SelectedIngrediente
        {
            get => _selectedIngrediente;
            set
            {
                _selectedIngrediente = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged();
                FiltrarIngredientes();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ManageCategoriesCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public IngredientesViewModel()
        {
            Console.WriteLine("Iniciando carga de datos.");
            _ingredienteService = new IngredienteService();
            _categoriaService = new CategoriaIngredienteService();
            _navigationService = ServiceLocator.NavigationService;

            AddCommand = new RelayCommand(async _ => await ShowAddDialogAsync());
            EditCommand = new RelayCommand(async _ => await ShowEditDialogAsync(), _ => SelectedIngrediente != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteIngredienteAsync(), _ => SelectedIngrediente != null);
            SearchCommand = new RelayCommand(_ => FiltrarIngredientes());
            ManageCategoriesCommand = new RelayCommand(_ => ExecuteManageCategories());
            ClearFiltersCommand = new RelayCommand(_ => ExecuteClearFilters());

            _ = LoadDatosInicialesAsync();
        }


        private async Task LoadDatosInicialesAsync()
        {
            await LoadIngredientesAsync();
            await LoadCategoriasAsync();
        }

        private async Task LoadIngredientesAsync()
        {
            try
            {
                var resultado = await _ingredienteService.GetIngredientesAsync();
                if (resultado != null)
                {
                    Ingredientes = new ObservableCollection<Ingrediente>(resultado);
                    Console.WriteLine($"Se cargaron {Ingredientes.Count} ingredientes.");
                }
                else
                {
                    Console.WriteLine("No se encontraron ingredientes.");
                }
            }
            catch (Exception ex)
            {
                // Muestra el error en un MessageBox
                MessageBox.Show($"Error al cargar ingredientes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task LoadCategoriasAsync()
        {
            try
            {
                var resultado = await _categoriaService.ObtenerCategoriasAsync();
                Categorias = new ObservableCollection<CategoriaIngrediente>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar categorías: {ex.Message}");
            }
        }

        private void ExecuteManageCategories()
        {
            _navigationService.NavigateTo<CategoriasIngredienteView>();
        }

        private void ExecuteClearFilters()
        {
            Filter = string.Empty;
            SelectedCategoria = null;
        }
        private void FiltrarIngredientes()
        {
            var query = Ingredientes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filtro = Filter.ToLower();
                query = query.Where(i => i.NombreIngrediente.ToLower().Contains(filtro));
            }

            if (SelectedCategoria != null)
            {
                query = query.Where(i => i.IdCategoria == SelectedCategoria.IdCategoria);
            }

            FiltradosIngredientes = new ObservableCollection<Ingrediente>(query);
        }


        private async Task ShowAddDialogAsync()
        {
            var dialog = new IngredienteDialog
            {
                DataContext = new IngredienteDialogViewModel()
            };
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadIngredientesAsync();
            }
        }

        private async Task ShowEditDialogAsync()
        {
            if (SelectedIngrediente == null) return;

            var dialog = new IngredienteDialog
            {
                DataContext = new IngredienteDialogViewModel(SelectedIngrediente)
            };
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadIngredientesAsync();
            }
        }

        private async Task DeleteIngredienteAsync()
        {
            if (SelectedIngrediente == null) return;

            try
            {
                await _ingredienteService.DeleteIngredienteAsync(SelectedIngrediente.IdIngrediente);
                await LoadIngredientesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar ingrediente: {ex.Message}");
            }
        }
    }
}
