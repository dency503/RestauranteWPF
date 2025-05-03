using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestauranteGestion.Models
{
    public class UsuarioLogin
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string Rol { get; set; }
        public Empleado Empleado { get; internal set; }
    }

}
