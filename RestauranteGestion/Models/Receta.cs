using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class Receta
    {
        public int IdReceta { get; set; }

        public int IdProducto { get; set; }

        public string NombreReceta { get; set; } // Esta propiedad puede ser generada para mostrar en la UI (por ejemplo: "Torta de Pollo #1")

        // Navegación
        public Producto Producto { get; set; }

        public ObservableCollection<DetalleReceta> Ingredientes { get; set; } = new();
    }
}
