namespace RestauranteGestion.Models
{
    public class Orden
    {
        public int IdOrden { get; set; }
        public string NombreCliente { get; set; }
        public string Estado { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
