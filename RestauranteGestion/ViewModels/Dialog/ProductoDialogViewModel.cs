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

public class ProductoDialogViewModel : INotifyPropertyChanged
{
    public Producto Producto { get; set; }
    public string Titulo { get; set; }

    private CategoriaProducto _categoriaSeleccionada;
    private ObservableCollection<CategoriaProducto> _categorias;
    private readonly ProductoService _productoService;

    public ObservableCollection<CategoriaProducto> Categorias
    {
        get => _categorias;
        set
        {
            _categorias = value;
            OnPropertyChanged();
        }
    }

    public ICommand GuardarCommand { get; }
    public ICommand CancelarCommand { get; }

    public ProductoDialogViewModel(Producto producto = null)
    {
        Producto = producto ?? new Producto();
        _productoService = new ProductoService();

        Titulo = producto?.IdProducto > 0 ? "Editar Producto" : "Agregar Producto";

        GuardarCommand = new RelayCommand(async _ => await GuardarAsync());
        CancelarCommand = new RelayCommand(_ => Cancelar());

        _ = LoadCategoriasAsync();
    }

    private async Task LoadCategoriasAsync()
    {
        var query = "SELECT * FROM rg_categoriaproducto";
        var categorias = await new DBOperacion()
                                .QueryAsync<CategoriaProducto>(query);

        Categorias = new ObservableCollection<CategoriaProducto>(categorias);
        if (Producto.IdCategoria > 0)
        {
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.IdCategoria == Producto.IdCategoria);
        }
    }

    public CategoriaProducto CategoriaSeleccionada
    {
        get => _categoriaSeleccionada;
        set
        {
            _categoriaSeleccionada = value;
            Producto.IdCategoria = value?.IdCategoria ?? 0;
            OnPropertyChanged();
        }
    }

    private async Task GuardarAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Producto.NombreProducto) || Producto.IdCategoria == 0)
            {
                MessageBox.Show("Por favor, completa todos los campos requeridos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Producto.IdProducto > 0)
            {
                await _productoService.EditarProductoAsync(Producto);
            }
            else
            {
                await _productoService.CrearProductoAsync(Producto);
            }

            DialogHost.CloseDialogCommand.Execute(Producto, null);
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
