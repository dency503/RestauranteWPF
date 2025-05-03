// ViewModels/EmpleadosViewModel.cs
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestauranteGestion.ViewModels
{
    public class EmpleadosViewModel : ViewModelBase
    {
        private readonly EmpleadoService _empleadoService = new();

        private ObservableCollection<Empleado> _empleados;
        private Empleado _selectedEmpleado;
        private string _filter;

        public ObservableCollection<Empleado> Empleados
        {
            get => _empleados;
            set { _empleados = value; OnPropertyChanged(); }
        }

        public Empleado SelectedEmpleado
        {
            get => _selectedEmpleado;
            set { _selectedEmpleado = value; OnPropertyChanged(); }
        }

        public string Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(); FiltrarEmpleados(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }

        public EmpleadosViewModel()
        {
            AddCommand = new RelayCommand(async _ => await AgregarEmpleadoAsync());
            EditCommand = new RelayCommand(async _ => await EditarEmpleadoAsync(), _ => SelectedEmpleado != null);
            DeleteCommand = new RelayCommand(async _ => await EliminarEmpleadoAsync(), _ => SelectedEmpleado != null);
            SearchCommand = new RelayCommand(_ => FiltrarEmpleados());

            CargarEmpleados();
        }

        private async void CargarEmpleados()
        {
            var empleados = await _empleadoService.ObtenerEmpleadosAsync();
            Empleados = new ObservableCollection<Empleado>(empleados);
        }

        private async Task AgregarEmpleadoAsync()
        {
           /* var dialog = new EmpleadoDialog();
            dialog.DataContext = new EmpleadoDialogViewModel();
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                CargarEmpleados();
            }*/
        }

        private async Task EditarEmpleadoAsync()
        {
         /*   if (SelectedEmpleado == null) return;

            var dialog = new EmpleadoDialog();
            dialog.DataContext = new EmpleadoDialogViewModel(SelectedEmpleado);
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                CargarEmpleados();
            }*/
        }

        private async Task EliminarEmpleadoAsync()
        {
            if (SelectedEmpleado == null) return;
            await _empleadoService.EliminarEmpleadoAsync(SelectedEmpleado.IdEmpleado);
            CargarEmpleados();
        }

        private void FiltrarEmpleados()
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                CargarEmpleados();
            }
            else
            {
                var filtrados = Empleados.Where(e =>
                    (e.Nombre + " " + e.Apellido).ToLower().Contains(Filter.ToLower()) ||
                    (e.Email?.ToLower().Contains(Filter.ToLower()) ?? false)
                ).ToList();

                Empleados = new ObservableCollection<Empleado>(filtrados);
            }
        }
    }
}
