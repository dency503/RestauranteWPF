using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int IdOrden { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }

        // Esta propiedad se calcula en MySQL como STORED, pero también puede calcularse aquí si se requiere
        public decimal SubTotal => Cantidad * PrecioVenta;

        // Opcionales para mostrar en vistas
        public string NombreProducto { get; set; } // Puede usarse en joins para mostrar
        public string ImagenProducto { get; set; }
    }
}
