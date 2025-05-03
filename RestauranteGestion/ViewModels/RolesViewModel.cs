using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.ViewModels.Dialog;
using RestauranteGestion.Views;
using RestauranteGestion.Views.Dialog;

namespace RestauranteGestion.ViewModels
{
    public class RolesViewModel : ViewModelBase
    {
        private readonly RolService _rolService = new();

        private ObservableCollection<Rol> _roles;
        private Rol _selectedRol;
        private string _filter;

        public ObservableCollection<Rol> Roles
        {
            get => _roles;
            set { _roles = value; OnPropertyChanged(); }
        }

        public Rol SelectedRol
        {
            get => _selectedRol;
            set { _selectedRol = value; OnPropertyChanged(); }
        }

        public string Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(); FiltrarRoles(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }

        public RolesViewModel()
        {
            AddCommand = new RelayCommand(async _ => await AgregarRolAsync());
            EditCommand = new RelayCommand(async _ => await EditarRolAsync(), _ => SelectedRol != null);
            DeleteCommand = new RelayCommand(async _ => await EliminarRolAsync(), _ => SelectedRol != null);
            SearchCommand = new RelayCommand(_ => FiltrarRoles());

            CargarRoles();
        }

        private async void CargarRoles()
        {
            var roles = await _rolService.ObtenerRolesAsync();
            Roles = new ObservableCollection<Rol>(roles);
        }

        private async Task AgregarRolAsync()
        {
            var dialog = new RolDialog();
            dialog.DataContext = new RolDialogViewModel();
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                CargarRoles();
            }
        }

        private async Task EditarRolAsync()
        {
            if (SelectedRol == null) return;

            var dialog = new RolDialog();
            dialog.DataContext = new RolDialogViewModel(SelectedRol);
            var result = await DialogHost.Show(dialog, "MyDialogHost");

            if (result is bool b && b)
            {
                CargarRoles();
            }
        }

        private async Task EliminarRolAsync()
        {
            if (SelectedRol == null) return;
            await _rolService.EliminarRolAsync(SelectedRol.IdRol);
            CargarRoles();
        }

        private void FiltrarRoles()
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                CargarRoles();
            }
            else
            {
                var filtrados = Roles.Where(r => r.NombreRol.ToLower().Contains(Filter.ToLower())).ToList();
                Roles = new ObservableCollection<Rol>(filtrados);
            }
        }
    }
}
