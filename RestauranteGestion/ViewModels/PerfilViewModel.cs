using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MySqlX.XDevAPI;
using RestauranteGestion.Models;
using RestauranteGestion.SesionManager;
using RestauranteGestion.ViewModels.Base;

namespace RestauranteGestion.ViewModels
{
    public class PerfilViewModel : INotifyPropertyChanged
    {
        private bool esEditable;
        public bool EsEditable
        {
            get => esEditable;
            set
            {
                esEditable = value;
                OnPropertyChanged(nameof(EsEditable));
            }
        }

        // Propiedades del perfil
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Cargo { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string NombreUsuario { get; set; }
        public ICommand CambiarContrasenaCommand { get; }

        public ObservableCollection<Actividad> ActividadReciente { get; set; }

        public ICommand EditarPerfilCommand { get; }
        public ICommand GuardarCambiosCommand { get; }
        private string contrasenaActual;
        public string ContrasenaActual
        {
            get => contrasenaActual;
            set
            {
                contrasenaActual = value;
                OnPropertyChanged(nameof(ContrasenaActual));
            }
        }

        private string nuevaContrasena;
        public string NuevaContrasena
        {
            get => nuevaContrasena;
            set
            {
                nuevaContrasena = value;
                OnPropertyChanged(nameof(NuevaContrasena));
            }
        }

        private string confirmarNuevaContrasena;
        public string ConfirmarNuevaContrasena
        {
            get => confirmarNuevaContrasena;
            set
            {
                confirmarNuevaContrasena = value;
                OnPropertyChanged(nameof(ConfirmarNuevaContrasena));
            }
        }
        private async void CambiarContrasena()
        {
            if (string.IsNullOrWhiteSpace(ContrasenaActual) ||
                string.IsNullOrWhiteSpace(NuevaContrasena) ||
                string.IsNullOrWhiteSpace(ConfirmarNuevaContrasena))
            {
                Debug.WriteLine("Por favor complete todos los campos de contraseña."+contrasenaActual+NuevaContrasena+ConfirmarNuevaContrasena);
                MessageBox.Show("Por favor complete todos los campos de contraseña.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NuevaContrasena != ConfirmarNuevaContrasena)
            {
                MessageBox.Show("La nueva contraseña y la confirmación no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var usuarioService = new UsuarioService();
            Sesion session = Sesion.ObtenerInstancia();

            // 1. Obtener la contraseña real de la base
            string contrasenaReal = await usuarioService.ObtenerContrasenaActualAsync(session.IdUsuario);

            if (contrasenaReal == null)
            {
                MessageBox.Show("No se pudo verificar la contraseña actual.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Comparar con la contraseña ingresada
            if (contrasenaReal != ContrasenaActual)
            {
                MessageBox.Show("La contraseña actual es incorrecta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Actualizar en la base
            bool cambioExitoso = await usuarioService.CambiarContrasenaAsync(session.IdUsuario, NuevaContrasena);

            if (!cambioExitoso)
            {
                MessageBox.Show("Hubo un problema al cambiar la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Limpiar campos
            ContrasenaActual = string.Empty;
            NuevaContrasena = string.Empty;
            ConfirmarNuevaContrasena = string.Empty;

            MessageBox.Show("Contraseña cambiada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public PerfilViewModel()
        {
            CambiarContrasenaCommand = new RelayCommand(_ => CambiarContrasena());

            EsEditable = false;
            Sesion Session = Sesion.ObtenerInstancia();
            // Aquí inicializas los datos del usuario de sesión
            if (Session.empleado != null)
            {
                Nombre = Session.empleado.Nombre;
                Apellido = Session.empleado.Apellido;
                Telefono = Session.empleado.Telefono;
                Direccion = Session.empleado.Direccion;
                Email = Session.empleado.Email;
                FechaNacimiento = Session.empleado.FechaNacimiento;
                Cargo = Session.empleado.CargoNombre;
                NombreUsuario = Session.Usuario;
            }

            ActividadReciente = new ObservableCollection<Actividad>();

            EditarPerfilCommand = new RelayCommand(_ => EditarPerfil());
            GuardarCambiosCommand = new RelayCommand(_ => GuardarCambios());
        }

        private void EditarPerfil()
        {
            EsEditable = true;
        }

        private void GuardarCambios()
        {
            // TODO: guardar cambios en base de datos aquí
            EsEditable = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class Actividad
    {
        public DateTime Fecha { get; set; }
        public string Accion { get; set; }
        public string DireccionIP { get; set; }
    }
}
