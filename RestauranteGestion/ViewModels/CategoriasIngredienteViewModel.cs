using System.Collections.ObjectModel;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using System.Threading.Tasks;
using System;
using System.Windows.Navigation;

namespace RestauranteGestion.ViewModels
{
    public class CategoriasIngredienteViewModel : ViewModelBase
    {
        private readonly CategoriaIngredienteService _categoriaService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<CategoriaIngrediente> _categorias = new();
        private CategoriaIngrediente _selectedCategoria;

        public ObservableCollection<CategoriaIngrediente> Categorias
        {
            get => _categorias;
            set
            {
                _categorias = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _fechaFiltro;
        public DateTime? FechaFiltro
        {
            get => _fechaFiltro;
            set
            {
                _fechaFiltro = value;
                OnPropertyChanged();
            }
        }

        public CategoriaIngrediente SelectedCategoria
        {
            get => _selectedCategoria;
            set
            {
                _selectedCategoria = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand FiltrarPorFechaCommand { get; }

        public CategoriasIngredienteViewModel()
        {
            _categoriaService = new CategoriaIngredienteService();
            _navigationService = ServiceLocator.NavigationService;

            BackCommand = new RelayCommand(_ => _navigationService.GoBack());
            AddCommand = new RelayCommand(_ => ShowAddDialog());
            EditCommand = new RelayCommand(_ => ShowEditDialog(), _ => SelectedCategoria != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteCategoria(), _ => SelectedCategoria != null);
            FiltrarPorFechaCommand = new RelayCommand(async _ => await FiltrarPorFecha(), _ => FechaFiltro != null);

            LoadCategorias();
        }

        private async void LoadCategorias()
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

        private void ShowAddDialog()
        {
            // Aquí va la lógica para mostrar el diálogo de agregar.
        }

        private void ShowEditDialog()
        {
            if (SelectedCategoria == null) return;
            // Aquí va la lógica para mostrar el diálogo de editar.
        }

        private async Task DeleteCategoria()
        {
            if (SelectedCategoria == null) return;

            try
            {
                await _categoriaService.EliminarCategoriaAsync(SelectedCategoria.IdCategoria);
                await RefreshCategorias();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar categoría: {ex.Message}");
            }
        }

        private async Task RefreshCategorias()
        {
            var resultado = await _categoriaService.ObtenerCategoriasAsync();
            Categorias = new ObservableCollection<CategoriaIngrediente>(resultado);
        }

        private async Task FiltrarPorFecha()
        {
            if (FechaFiltro == null) return;

            try
            {
                var resultado = await _categoriaService.ObtenerCategoriasPorFechaAsync(FechaFiltro.Value);
                Categorias = new ObservableCollection<CategoriaIngrediente>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por fecha: {ex.Message}");
            }
        }
    }
}
