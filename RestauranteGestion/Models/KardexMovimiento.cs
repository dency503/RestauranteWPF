using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class KardexMovimiento
    {
        public int IdKardex { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public string DocumentoReferencia { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreProducto { get; set; }
        public string NombreIngrediente { get; set; }
    }

}
