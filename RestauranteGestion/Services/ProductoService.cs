using System.Data;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

public class ProductoService
{
    private readonly DBOperacion _dbOperacion;

    public ProductoService()
    {
        _dbOperacion = new DBOperacion();
    }
    public async Task<List<Producto>> ObtenerProductosSinReceta()
    {


        string sql = @"
            SELECT p.idProducto, p.nombreProducto
            FROM rg_producto p
            LEFT JOIN rg_receta r ON p.idProducto = r.idProducto
            WHERE r.idProducto IS NULL";
        return await _dbOperacion.Consultar(sql, row => new Producto
        {
            IdProducto = row.Field<int>("idProducto"),
            NombreProducto = row.Field<string>("nombreProducto"),
           
            // Agrega más campos si es necesario
        });



    }
    public async Task<int> CrearProductoAsync(Producto producto)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@NombreProducto", producto.NombreProducto },
            { "@CostoUnitario", producto.CostoUnitario },
            { "@Stock", producto.Stock },
            { "@PrecioVenta", producto.PrecioVenta },
            { "@IdCategoria", producto.IdCategoria },
            { "@Imagen", producto.Imagen }
        };

        string sql = "INSERT INTO RG_Producto (nombreProducto, costoUnitario, stock, precioVenta, idCategoria, imagen) " +
                     "VALUES (@NombreProducto, @CostoUnitario, @Stock, @PrecioVenta, @IdCategoria, @Imagen);";

        return await _dbOperacion.EjecutarSentenciaYObtenerID(sql, parametros);
    }

    public async Task EditarProductoAsync(Producto producto)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdProducto", producto.IdProducto },
            { "@NombreProducto", producto.NombreProducto },
            { "@CostoUnitario", producto.CostoUnitario },
            { "@Stock", producto.Stock },
            { "@PrecioVenta", producto.PrecioVenta },
            { "@IdCategoria", producto.IdCategoria },
            { "@Imagen", producto.Imagen }
        };

        string sql = "UPDATE RG_Producto SET nombreProducto = @NombreProducto, costoUnitario = @CostoUnitario, " +
                     "stock = @Stock, precioVenta = @PrecioVenta, idCategoria = @IdCategoria, imagen = @Imagen " +
                     "WHERE idProducto = @IdProducto;";

        await _dbOperacion.EjecutarAsync(sql, parametros);
    }

    public async Task EliminarProductoAsync(int idProducto)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@IdProducto", idProducto }
        };

        string sql = "DELETE FROM RG_Producto WHERE idProducto = @IdProducto;";
        await _dbOperacion.EjecutarAsync(sql, parametros);
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        string sql = "SELECT * FROM RG_Producto";
        return await _dbOperacion.Consultar<Producto>(sql, MapearProducto);
    }

    private Producto MapearProducto(DataRow row)
    {
        return new Producto
        {
            IdProducto = Convert.ToInt32(row["idProducto"]),
            NombreProducto = row["nombreProducto"].ToString(),
            CostoUnitario = Convert.ToDecimal(row["costoUnitario"]),
            Stock = Convert.ToInt32(row["stock"]),
            PrecioVenta = Convert.ToDecimal(row["precioVenta"]),
            IdCategoria = Convert.ToInt32(row["idCategoria"]),
            Imagen = row["imagen"].ToString()
        };
    }
}
