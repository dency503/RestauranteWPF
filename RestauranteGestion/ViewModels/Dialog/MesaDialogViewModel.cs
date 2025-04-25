using MaterialDesignThemes.Wpf;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using RestauranteGestion.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RestauranteGestion.Services;

public class MesaDialogViewModel : INotifyPropertyChanged
{
    public Mesa Mesa { get; set; }
    public string Titulo { get; set; }

   
    private string _estadoSeleccionado;
    private readonly MesaService _mesaService;

    // Lista local de tres estados como strings
    public List<string> Estados { get; } = new()
{
    "Disponible",
    "Ocupada",
    "Mantenimiento" // ← asegúrate de que coincida exactamente con lo definido en la tabla
};



    public ICommand GuardarMesaCommand { get; }
    public ICommand CancelarCommand { get; }

    public MesaDialogViewModel(Mesa mesa = null)
    {
        Mesa = mesa ?? new Mesa();
        _mesaService = new MesaService();

        Titulo = mesa?.IdMesa > 0 ? "Editar Mesa" : "Agregar Mesa";

        GuardarMesaCommand = new RelayCommand(async _ => await GuardarAsync());
        CancelarCommand = new RelayCommand(_ => Cancelar());

        
    }

    

    public string EstadoSeleccionado
    {
        get => _estadoSeleccionado;
        set
        {
            _estadoSeleccionado = value;
            Mesa.Estado = value;  // Asignar el valor del estado seleccionado a la propiedad de la mesa
            OnPropertyChanged();
        }
    }

    private async Task GuardarAsync()
    {
        try
        {
            if (Mesa.NumeroMesa <= 0 || string.IsNullOrWhiteSpace(Mesa.Estado))
            {
                MessageBox.Show("Por favor, completa todos los campos requeridos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Mesa.IdMesa > 0)
            {
                await _mesaService.EditarMesaAsync(Mesa);
            }
            else
            {
                await _mesaService.AgregarMesaAsync(Mesa);
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
        DialogHost.CloseDialogCommand.Execute(null, null);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
