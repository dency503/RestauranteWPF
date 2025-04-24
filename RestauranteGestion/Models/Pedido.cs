using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class Pedido
    {
        public Mesa Mesa { get; set; }
        public string NombreCliente { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }

}
