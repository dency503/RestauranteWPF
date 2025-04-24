using System;

namespace RestauranteGestion.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }

        public int IdCargo { get; set; }
       // public Cargo Cargo { get; set; } = new(); // Relación con RG_Cargo

        // Propiedad combinada para mostrar en UI
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
