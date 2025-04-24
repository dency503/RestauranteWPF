using MaterialDesignThemes.Wpf;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using RestauranteGestion.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using RestauranteGestion.Services;
using System.Linq;
using System.Threading.Tasks;

public class IngredienteDialogViewModel : INotifyPropertyChanged
{
    public Ingrediente Ingrediente { get; set; }
    public string Titulo { get; set; }

    private CategoriaIngrediente _categoriaSeleccionada;
    private ObservableCollection<CategoriaIngrediente> _categorias;
    private readonly IngredienteService _ingredienteService;

    public ObservableCollection<CategoriaIngrediente> Categorias
    {
        get => _categorias;
        set
        {
            _categorias = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> UnidadesMedida { get; set; }
    public ICommand GuardarCommand { get; }
    public ICommand CancelarCommand { get; }

    public IngredienteDialogViewModel(Ingrediente ingrediente = null)
    {
        Ingrediente = ingrediente ?? new Ingrediente();
        _ingredienteService = new IngredienteService();

        Titulo = ingrediente?.IdIngrediente > 0 ? "Editar Ingrediente" : "Agregar Ingrediente";

        GuardarCommand = new RelayCommand(async _ => await GuardarAsync());
        CancelarCommand = new RelayCommand(_ => Cancelar());

        UnidadesMedida = new ObservableCollection<string>
        {
            "Unidad", "Gramo", "Litro", "Kilogramo"
        };

        _ = LoadCategoriasAsync();
    }

    private async Task LoadCategoriasAsync()
    {
        var query = "SELECT * FROM rg_categoriaingrediente";
        var categorias = await new DBOperacion()
                                .QueryAsync<CategoriaIngrediente>(query);

        Categorias = new ObservableCollection<CategoriaIngrediente>(categorias);
        if (Ingrediente.IdCategoria > 0)
        {
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.IdCategoria == Ingrediente.IdCategoria);
        }
    }

    public CategoriaIngrediente CategoriaSeleccionada
    {
        get => _categoriaSeleccionada;
        set
        {
            _categoriaSeleccionada = value;
            Ingrediente.IdCategoria = value?.IdCategoria ?? 0;
            OnPropertyChanged();
        }
    }

    private async Task GuardarAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Ingrediente.NombreIngrediente) || Ingrediente.IdCategoria == 0)
            {
                MessageBox.Show("Por favor, completa todos los campos requeridos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Ingrediente.IdIngrediente > 0)
            {
                await _ingredienteService.EditIngredienteAsync(Ingrediente);
            }
            else
            {
                await _ingredienteService.AddIngredienteAsync(Ingrediente);
            }

            DialogHost.CloseDialogCommand.Execute(true, null); // Devuelve el objeto al padre
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Ocurrió un error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancelar()
    {
        DialogHost.CloseDialogCommand.Execute(null, null); // Cierra sin guardar
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
