using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class Ingrediente
    {
        internal double StockMinimo;

        public int IdIngrediente { get; set; }
        public string NombreIngrediente { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; }
        public decimal Stock { get; set; }
        public string UnidadMedida { get; set; }
        public object Imagen { get; internal set; }
        public object Precio { get; internal set; }
    }


}
