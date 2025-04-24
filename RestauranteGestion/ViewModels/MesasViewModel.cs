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
    public class MesasViewModel : ViewModelBase
    {
        private readonly MesaService _mesaService;

        private ObservableCollection<Mesa> _mesas = new();
        private ObservableCollection<Mesa> _filtradasMesas = new();
        private ObservableCollection<string> _estadosMesa = new();
        private Mesa _selectedMesa;
        private string _selectedEstado;
        private string _filter;

        public ObservableCollection<Mesa> Mesas
        {
            get => _mesas;
            set
            {
                _mesas = value;
                OnPropertyChanged();
                FiltrarMesas();
            }
        }

        public ObservableCollection<Mesa> FiltradasMesas
        {
            get => _filtradasMesas;
            set
            {
                _filtradasMesas = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> EstadosMesa
        {
            get => _estadosMesa;
            set
            {
                _estadosMesa = value;
                OnPropertyChanged();
            }
        }

        public string SelectedEstado
        {
            get => _selectedEstado;
            set
            {
                _selectedEstado = value;
                OnPropertyChanged();
                FiltrarMesas();
            }
        }

        public Mesa SelectedMesa
        {
            get => _selectedMesa;
            set
            {
                if (_selectedMesa != value)
                {
                    _selectedMesa = value;
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
                FiltrarMesas();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public MesasViewModel()
        {
            _mesaService = new MesaService();

            AddCommand = new RelayCommand(_ => ShowAddDialog());
            EditCommand = new RelayCommand(_ => ShowEditDialog(), _ => SelectedMesa != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteMesa(), _ => SelectedMesa != null);
            SearchCommand = new RelayCommand(_ => FiltrarMesas());
            ClearFiltersCommand = new RelayCommand(_ => ExecuteClearFilters());

            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            await LoadMesas();
            EstadosMesa = new ObservableCollection<string> { "Disponible", "Ocupada", "Reservada" }; // Los 3 estados posibles
        }

        private async Task LoadMesas()
        {
            try
            {
                var resultado = await _mesaService.ObtenerMesasAsync();
                Mesas = new ObservableCollection<Mesa>(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar mesas: {ex.Message}");
                // Aquí podrías mostrar un mensaje al usuario
            }
        }

        private void ExecuteClearFilters()
        {
            Filter = string.Empty;
            SelectedEstado = null;
        }

        private void FiltrarMesas()
        {
            var query = Mesas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filtro = Filter.ToLower();
                query = query.Where(m => m.NumeroMesa.ToString().Contains(filtro));
            }

            if (!string.IsNullOrWhiteSpace(SelectedEstado))
            {
                query = query.Where(m => m.Estado?.ToLower() == SelectedEstado.ToLower());
            }

            FiltradasMesas = new ObservableCollection<Mesa>(query);
        }

        private async void ShowAddDialog()
        {
            var dialog = new MesaDialog();
            dialog.DataContext = new MesaDialogViewModel();
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadMesas();
            }
        }

        private async void ShowEditDialog()
        {
            if (SelectedMesa == null) return;

            var dialog = new MesaDialog();
            dialog.DataContext = new MesaDialogViewModel(SelectedMesa);
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                await LoadMesas();
            }
        }

        private async Task DeleteMesa()
        {
            if (SelectedMesa == null) return;

            try
            {
                await _mesaService.EliminarMesaAsync(SelectedMesa.IdMesa);
                await LoadMesas();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar mesa: {ex.Message}");
                // Mostrar mensaje de error al usuario
            }
        }
    }
}
