using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class DetalleReceta
    {
        public int IdDetalleReceta { get; set; }

        public int IdReceta { get; set; }

        public int IdIngrediente { get; set; }

        public decimal CantidadNecesaria { get; set; }

        // Navegación
        public Ingrediente Ingrediente { get; set; }

        public Receta Receta { get; set; }
    }
}
