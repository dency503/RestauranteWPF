using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;

namespace RestauranteGestion.ViewModels
{
    public class EmpleadoDialogViewModel : INotifyPropertyChanged
    {
        private readonly EmpleadoService _empleadoService;
        private readonly DBOperacion _dbOperacion;

        public Empleado Empleado { get; set; }
        public string Titulo { get; set; }

        private ObservableCollection<Cargo> _cargos;
        public ObservableCollection<Cargo> Cargos
        {
            get => _cargos;
            set { _cargos = value; OnPropertyChanged(); }
        }

        private Cargo _cargoSeleccionado;
        public Cargo CargoSeleccionado
        {
            get => _cargoSeleccionado;
            set
            {
                _cargoSeleccionado = value;
                Empleado.IdCargo = value?.IdCargo ?? 0;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public EmpleadoDialogViewModel(Empleado empleado = null)
        {
            _empleadoService = new EmpleadoService();
            _dbOperacion = new DBOperacion();
            Empleado = empleado ?? new Empleado();

            Titulo = empleado?.IdEmpleado > 0 ? "Editar Empleado" : "Agregar Empleado";

            GuardarCommand = new RelayCommand(async _ => await GuardarAsync());
            CancelarCommand = new RelayCommand(_ => Cancelar());

            _ = LoadCargosAsync();
        }

        private async Task LoadCargosAsync()
        {
            var query = "SELECT * FROM rg_cargo";
            var cargos = await _dbOperacion.QueryAsync<Cargo>(query);
            Cargos = new ObservableCollection<Cargo>(cargos);

            if (Empleado.IdCargo > 0)
            {
                CargoSeleccionado = Cargos.FirstOrDefault(c => c.IdCargo == Empleado.IdCargo);
            }
        }

        private async Task GuardarAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Empleado.Nombre) ||
                    string.IsNullOrWhiteSpace(Empleado.Apellido) ||
                    Empleado.IdCargo == 0)
                {
                    MessageBox.Show("Por favor, completa todos los campos requeridos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Empleado.IdEmpleado > 0)
                {
                    await _empleadoService.ActualizarEmpleadoAsync(Empleado);
                }
                else
                {
                    await _empleadoService.AgregarEmpleadoAsync(Empleado);
                }

                DialogHost.CloseDialogCommand.Execute(true, null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar()
        {
            DialogHost.CloseDialogCommand.Execute(false, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Modelo para el Cargo
    public class Cargo
    {
        public int IdCargo { get; set; }
        public string NombreCargo { get; set; }
    }
}
