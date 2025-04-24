using System.Collections.ObjectModel;
using System.Data;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using RestauranteGestion.ViewModels;

public class VentaService
{
    private readonly DBOperacion _dbOperacion;

    public VentaService()
    {
        _dbOperacion = new DBOperacion();
    }

    #region Obtener Ventas
    public async Task<int> ContarVentasAsync()
{
    string sql = "SELECT COUNT(*) FROM RG_Venta";
        return await _dbOperacion.EjecutarEscalarAsync<int>(sql);
    }

    public async Task<List<Venta>> ObtenerVentasAsync(int offset, int limit)
    {
        string sql = $"SELECT idVenta, idEmpleado, idOrden, total, metodoPago, fechaHora FROM RG_Venta ORDER BY fechaHora DESC LIMIT {limit} OFFSET {offset}";
        var ventas = await _dbOperacion.Consultar(sql, MapearVenta);

        foreach (var venta in ventas)
        {
            var detalles = await ObtenerDetallesPorVentaIdAsync(venta.IdVenta);
            venta.Detalles = detalles;
        }

        return ventas;
    }

    public async Task<List<Venta>> ObtenerVentasAsync()
    {
        string sql = "SELECT idVenta, idEmpleado, idOrden, total, metodoPago, fechaHora FROM RG_Venta";
        var ventas = await _dbOperacion.Consultar(sql, MapearVenta);

        // Agregar detalles a cada venta
        // Versión recomendada (paralelismo controlado)
        const int batchSize = 10; // Procesar 10 ventas a la vez
        for (int i = 0; i < ventas.Count; i += batchSize)
        {
            var batch = ventas.Skip(i).Take(batchSize);
            var batchTasks = batch.Select(v => ObtenerDetallesPorVentaIdAsync(v.IdVenta));
            var detallesBatch = await Task.WhenAll(batchTasks);

            for (int j = 0; j < batch.Count(); j++)
            {
                batch.ElementAt(j).Detalles = detallesBatch[j];
            }
        }

        return ventas;
    }
    #endregion

    public async Task<List<DetalleVenta>> ObtenerDetallesPorVentaIdAsync(int idVenta)
    {
        const string query = @"
        SELECT dv.Cantidad, dv.PrecioVenta, 
               p.IdProducto, p.NombreProducto, dv.IdDetalleVenta
        FROM RG_DetalleVenta dv
        INNER JOIN RG_Producto p ON dv.IdProducto = p.IdProducto
        WHERE dv.IdVenta = @IdVenta";

       

        return await _dbOperacion.Consultar(query, row => new DetalleVenta
        {
            Cantidad = Convert.ToInt32(row["Cantidad"]),
            PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),
            IdDetalleVenta = Convert.ToInt32(row["IdDetalleVenta"]),
         
            IdProducto = Convert.ToInt32(row["IdProducto"]),
            NombreProducto = row["NombreProducto"].ToString(),
           
        }, new Dictionary<string, object>
    {
        { "@IdVenta", idVenta }
    });
    }

    #region Obtener Detalles de Venta
    public async Task<List<DetalleVenta>> GetDetallesVentaAsync(int idOrden)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@idOrden", idOrden }
        };

        string sql = "SELECT idProducto, cantidad, precioVenta, subTotal FROM RG_DetalleVenta WHERE idOrden = @idOrden;";

        return await _dbOperacion.Consultar<DetalleVenta>(sql, MapearDetalleVenta, parametros);
    }

    private DetalleVenta MapearDetalleVenta(DataRow row)
    {
        return new DetalleVenta
        {
            IdProducto = Convert.ToInt32(row["idProducto"]),
            Cantidad = Convert.ToInt32(row["cantidad"]),
            PrecioVenta = Convert.ToDecimal(row["precioVenta"]),
            
        };
    }
    #endregion

    #region Agregar Venta
    public async Task<int> AgregarVentaAsync(Venta venta)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@idEmpleado", venta.IdEmpleado },
            { "@idOrden", venta.IdOrden },
            { "@total", venta.Total },
            { "@metodoPago", venta.MetodoPago }
        };

        string sql = "INSERT INTO RG_Venta (idEmpleado, idOrden, total, metodoPago) " +
                     "VALUES (@idEmpleado, @idOrden, @total, @metodoPago);";

        return await _dbOperacion.EjecutarSentenciaYObtenerID(sql, parametros);
    }
    #endregion

    #region Editar Venta
    public async Task EditarVentaAsync(Venta venta)
    {
        var parametros = new Dictionary<string, object>
        {
            { "@idVenta", venta.IdVenta },
            { "@idEmpleado", venta.IdEmpleado },
            { "@idOrden", venta.IdOrden },
            { "@total", venta.Total },
            { "@metodoPago", venta.MetodoPago }
        };

        string sql = "UPDATE RG_Venta SET idEmpleado = @idEmpleado, idOrden = @idOrden, " +
                     "total = @total, metodoPago = @metodoPago WHERE idVenta = @idVenta;";

        await _dbOperacion.EjecutarAsync(sql, parametros);
    }
    #endregion

    #region Eliminar Venta
    public async Task<bool> EliminarVentaAsync(int idVenta)
    {
        var parametros = new Dictionary<string, object>
    {
        { "@idVenta", idVenta }
    };

        string sql = "DELETE FROM RG_Venta WHERE idVenta = @idVenta;";
        int filasAfectadas = await _dbOperacion.EjecutarAsync(sql, parametros);

        return filasAfectadas > 0;
    }

    #endregion

    #region Mapear Venta
    private Venta MapearVenta(DataRow row)
    {
        return new Venta
        {
            IdVenta = Convert.ToInt32(row["idVenta"]),
            IdEmpleado = Convert.ToInt32(row["idEmpleado"]),
            IdOrden = Convert.ToInt32(row["idOrden"]),
            Total = Convert.ToDecimal(row["total"]),
            MetodoPago = row["metodoPago"].ToString(),
            FechaHora = Convert.ToDateTime(row["fechaHora"])
        };
    }
    #endregion
}
