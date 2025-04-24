
using RestauranteGestion.Models;

namespace RestauranteGestion.ViewModels
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdEmpleado { get; set; }
        public int IdOrden { get; set; }
        public Empleado Empleado { get; set; } = new();
        public Orden Orden { get; set; } = new();
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public DateTime FechaHora { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}