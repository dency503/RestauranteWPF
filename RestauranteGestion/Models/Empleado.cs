using System;

namespace RestauranteGestion.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int IdCargo { get; set; }

        // Propiedad opcional si quieres mostrar el nombre del cargo sin hacer otra consulta en la vista
        public string CargoNombre { get; set; }
        public string? NombreCargo { get; internal set; }
    }
}
