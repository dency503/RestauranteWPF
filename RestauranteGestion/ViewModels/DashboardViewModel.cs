using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using RestauranteGestion.Models;
using RestauranteGestion.Services;
using RestauranteGestion.ViewModels.Base;

namespace RestauranteGestion.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DashboardService _dashboardService;

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel()
        {
            _dashboardService = new DashboardService();
            CargarDashboardCommand = new RelayCommand(async _ => await CargarDashboardAsync());

            ProductosMasVendidos = new ObservableCollection<ProductoMasVendido>();
            MovimientosKardex = new ObservableCollection<KardexMovimiento>();
            UltimosPedidos = new ObservableCollection<Pedido>();
            InventarioCritico = new ObservableCollection<Ingrediente>();
            Mesas = new ObservableCollection<Mesa>();

            VentasSeries = new SeriesCollection();
            TopProductosSeries = new SeriesCollection();
            Fechas = new ObservableCollection<string>();
        }

        private decimal _ventasHoy;
        public decimal VentasHoy
        {
            get => _ventasHoy;
            set { _ventasHoy = value; OnPropertyChanged(nameof(VentasHoy)); }
        }

        private int _mesasOcupadas;
        public int MesasOcupadas
        {
            get => _mesasOcupadas;
            set { _mesasOcupadas = value; OnPropertyChanged(nameof(MesasOcupadas)); }
        }

        private int _alertasInventario;
        public int AlertasInventario
        {
            get => _alertasInventario;
            set { _alertasInventario = value; OnPropertyChanged(nameof(AlertasInventario)); }
        }

        public SeriesCollection VentasSeries { get; set; }
        public ObservableCollection<string> Fechas { get; set; }

        public SeriesCollection TopProductosSeries { get; set; }

        public ObservableCollection<Mesa> Mesas { get; set; }
        public ObservableCollection<Pedido> UltimosPedidos { get; set; }
        public ObservableCollection<Ingrediente> InventarioCritico { get; set; }

        public ObservableCollection<ProductoMasVendido> ProductosMasVendidos { get; set; }
        public ObservableCollection<KardexMovimiento> MovimientosKardex { get; set; }

        public ICommand CargarDashboardCommand { get; }

        private async Task CargarDashboardAsync()
        {
            VentasHoy = await _dashboardService.ObtenerVentasHoyAsync();
            MesasOcupadas = await _dashboardService.ObtenerMesasOcupadasAsync();
            AlertasInventario = await _dashboardService.ObtenerCantidadAlertasInventarioAsync();

            var ventasPorFecha = await _dashboardService.ObtenerVentasPorFechaAsync();
            VentasSeries.Clear();
            Fechas.Clear();
            VentasSeries.Add(new ColumnSeries
            {
                Title = "Ventas",
                Values = new ChartValues<decimal>(ventasPorFecha.Select(v => v.Total))
            });
            foreach (var item in ventasPorFecha)
                Fechas.Add(item.Fecha.ToString("dd/MM"));

            var topProductos = await _dashboardService.ObtenerProductosMasVendidosAsync();
            TopProductosSeries.Clear();
            TopProductosSeries.AddRange(topProductos.Select(p => new PieSeries
            {
                Title = p.Nombre,
                Values = new ChartValues<int> { p.Cantidad },
                DataLabels = true
            }));

            Mesas = new ObservableCollection<Mesa>(await _dashboardService.ObtenerEstadoMesasAsync());
            OnPropertyChanged(nameof(Mesas));

            UltimosPedidos = new ObservableCollection<Pedido>(await _dashboardService.ObtenerUltimosPedidosAsync());
            OnPropertyChanged(nameof(UltimosPedidos));

            InventarioCritico = new ObservableCollection<Ingrediente>(await _dashboardService.ObtenerInventarioCriticoAsync());
            OnPropertyChanged(nameof(InventarioCritico));
        }

        protected void OnPropertyChanged(string propiedad) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
    }
}
