using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;
using RestauranteGestion.Views;

namespace RestauranteGestion.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly MainViewModel _mainViewModel;
        private string _usuario;

        public string Usuario
        {
            get => _usuario;
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged();
                    _loginCommand.RaiseCanExecuteChanged(); // Notifica cambio en la capacidad de iniciar sesión
                }
            }
        }

        private string _contrasenia;
        public string Contrasenia
        {
            get => _contrasenia;
            set
            {
                if (_contrasenia != value)
                {
                    _contrasenia = value;
                    OnPropertyChanged();
                    _loginCommand.RaiseCanExecuteChanged(); // Notifica cambio en la capacidad de iniciar sesión
                }
            }
        }


        private bool _estaCargando;
        public bool EstaCargando
        {
            get => _estaCargando;
            set => SetProperty(ref _estaCargando, value);
        }

        private string _mensajeError;
        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }
        private readonly RelayCommand _loginCommand;
        public ICommand LoginCommand => _loginCommand;

        private readonly LoginService _loginService;

        public LoginViewModel()
        {
            _loginService = new LoginService();
            _loginCommand = new RelayCommand(async _ => await IniciarSesionAsync(), _ => PuedeIniciarSesion());
            
            _navigationService = ServiceLocator.NavigationService;
            _mainViewModel = ServiceLocator.MainViewModel;
        }
        private bool PuedeIniciarSesion()
        {
            //Debug.WriteLine($"Evaluando CanExecute: Usuario='{Usuario}', Contraseña='{Contrasenia}'");
            return !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Contrasenia);
        }


        private async Task IniciarSesionAsync()
        {
            EstaCargando = true;

            var resultado = await _loginService.AutenticarUsuarioAsync(Usuario, Contrasenia);

            EstaCargando = false;

            if (resultado != null)
            {
                // Aquí podrías navegar a otra vista, guardar sesión, etc.
                _navigationService.NavigateTo<DashboardView>();
                _mainViewModel.SetSidebarVisibility(true);
                EventBus.RaiseLoginSucceeded();
            }
            else
            {
                MensajeError = "Credenciales incorrectas. Intente de nuevo.";
            }
        }

        // --- INotifyPropertyChanged Boilerplate ---
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string propiedad = "")
        {
            if (Equals(campo, valor)) return false;
            campo = valor;
            OnPropertyChanged(propiedad);
            return true;
        }
    }
}
