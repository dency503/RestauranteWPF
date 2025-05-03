using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;

namespace RestauranteGestion.ViewModels.Dialog
{
    public class RolDialogViewModel : INotifyPropertyChanged
    {
        private readonly RolService _rolService;
        public Rol Rol { get; set; }
        public string Titulo { get; set; }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public RolDialogViewModel(Rol rol = null)
        {
            _rolService = new RolService();
            Rol = rol != null ? new Rol
            {
                IdRol = rol.IdRol,
                NombreRol = rol.NombreRol
            } : new Rol();

            Titulo = rol != null ? "Editar Rol" : "Agregar Rol";

            GuardarCommand = new RelayCommand(async _ => await GuardarAsync());
            CancelarCommand = new RelayCommand(_ => Cancelar());
        }

        private async Task GuardarAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Rol.NombreRol))
                {
                    MessageBox.Show("Por favor, ingresa el nombre del rol.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Rol.IdRol > 0)
                {
                    await _rolService.ActualizarRolAsync(Rol.IdRol, Rol.NombreRol);
                }
                else
                {
                    await _rolService.AgregarRolAsync(Rol.NombreRol);
                }

                DialogHost.CloseDialogCommand.Execute(true, null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
}