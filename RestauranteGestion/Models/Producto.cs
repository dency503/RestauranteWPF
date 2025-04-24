namespace RestauranteGestion.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal CostoUnitario { get; set; }
        public int Stock { get; set; }
        public decimal PrecioVenta { get; set; }
        public int IdCategoria { get; set; }
        public string Imagen { get; set; }  // Puede ser una ruta o URL
    }
}
