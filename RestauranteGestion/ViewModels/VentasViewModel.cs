using RestauranteGestion.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestauranteGestion.ViewModels
{
    public class VentasViewModel : ViewModelBase
    {
        private readonly VentaService _ventaService;

        private List<Venta> _todasLasVentas = new();
        private List<Venta> _ventasFiltradas = new();
        private Venta _selectedVenta;
        public Venta SelectedVenta
        {
            get => _selectedVenta;
            set
            {
                _selectedVenta = value;
                OnPropertyChanged();
               
            }
        }

        public ObservableCollection<Venta> VentasPaginadas { get; set; } = new();

        private DateTime? _fechaFiltro;
        public DateTime? FechaFiltro
        {
            get => _fechaFiltro;
            set
            {
                _fechaFiltro = value;
                OnPropertyChanged();
                FiltrarVentas();
            }
        }

        public int PageSize { get; set; } = 10;
        private int currentPageIndex = 0;

        public int CurrentPage => currentPageIndex;

        public string PaginationInfo
        {
            get
            {
                // Usar el total de ventas filtradas para el cálculo de páginas
                int total = _ventasFiltradas.Count;
                int totalPages = (int)Math.Ceiling((double)total / PageSize);
                if (totalPages == 0) totalPages = 1;
                return $"Página {Math.Min(CurrentPage + 1, totalPages)} de {totalPages}";
            }
        }

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public VentasViewModel()
        {
            _ventaService = new VentaService();
            NextPageCommand = new RelayCommand(_ => NextPage());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage());
            CargarVentas();
        }

        private async void CargarVentas()
        {
            _todasLasVentas = await _ventaService.ObtenerVentasAsync(0, 1000); // Cargar todas las ventas para hacer el filtrado
            _ventasFiltradas = _todasLasVentas; // Inicialmente, no hay filtro
            await CargarPaginaActualAsync();
        }

        private async Task CargarPaginaActualAsync()
        {
            int offset = currentPageIndex * PageSize;
            var lista = await _ventaService.ObtenerVentasAsync(offset, PageSize);

            VentasPaginadas.Clear();
            foreach (var venta in lista)
            {
                VentasPaginadas.Add(venta);
            }

            OnPropertyChanged(nameof(PaginationInfo)); // Actualizar la información de paginación
        }

        private void FiltrarVentas()
        {
            if (FechaFiltro == null)
            {
                _ventasFiltradas = _todasLasVentas;
            }
            else
            {
                _ventasFiltradas = _todasLasVentas
                    .Where(v => v.FechaHora.Date == FechaFiltro.Value.Date)
                    .ToList();
            }

            currentPageIndex = 0; // Reiniciar a la primera página después de aplicar un filtro
            ActualizarVentasPaginadas();
        }

        private void ActualizarVentasPaginadas()
        {
            VentasPaginadas.Clear();

            var pagina = _ventasFiltradas
                .Skip(CurrentPage * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (var venta in pagina)
            {
                VentasPaginadas.Add(venta);
            }

            // Asegurarse de que la propiedad PaginationInfo se actualice después de cargar las ventas filtradas
            OnPropertyChanged(nameof(PaginationInfo));
        }

        private async void NextPage()
        {
            int totalPages = (int)Math.Ceiling((double)_ventasFiltradas.Count / PageSize); // Usar el total filtrado para el cálculo
            if (currentPageIndex < totalPages - 1)
            {
                currentPageIndex++;
                await CargarPaginaActualAsync();
            }
        }

        private async void PreviousPage()
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                await CargarPaginaActualAsync();
            }
        }
    }
}
