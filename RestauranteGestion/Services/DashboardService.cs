using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

public class DashboardService
{
    private readonly DBOperacion _dbOperacion;

    public DashboardService()
    {
        _dbOperacion = new DBOperacion();
    }

    /// <summary>
    /// Obtiene las estadísticas de ventas por fecha.
    /// </summary>
    public async Task<List<VentasPorFecha>> ObtenerVentasPorFechaAsync()
    {
        string sql = @"
            SELECT DATE(v.fechaHora) AS Fecha, SUM(v.total) AS Total
            FROM RG_Venta v
            GROUP BY DATE(v.fechaHora)
            ORDER BY Fecha DESC;";

        return await _dbOperacion.Consultar<VentasPorFecha>(sql, MapearVentasPorFecha);
    }

    /// <summary>
    /// Obtiene los productos más vendidos.
    /// </summary>
    public async Task<List<ProductoMasVendido>> ObtenerProductosMasVendidosAsync()
    {
        string sql = @"
            SELECT p.idProducto, p.nombreProducto, SUM(dv.cantidad) AS TotalVendidos
            FROM RG_Producto p
            JOIN RG_DetalleVenta dv ON p.idProducto = dv.idProducto
            GROUP BY p.idProducto
            ORDER BY TotalVendidos DESC
            LIMIT 10;";

        return await _dbOperacion.Consultar<ProductoMasVendido>(sql, MapearProductoMasVendido);
    }

    /// <summary>
    /// Obtiene el total de ventas del mes.
    /// </summary>
    public async Task<decimal> ObtenerTotalVentasMesAsync()
    {
        string sql = @"
            SELECT SUM(v.total) 
            FROM RG_Venta v 
            WHERE MONTH(v.fechaHora) = MONTH(CURRENT_DATE) AND YEAR(v.fechaHora) = YEAR(CURRENT_DATE);";

        return await _dbOperacion.EjecutarEscalarAsync<decimal>(sql);
    }

    /// <summary>
    /// Obtiene el total de productos vendidos en el mes.
    /// </summary>
    public async Task<int> ObtenerTotalProductosVendidosMesAsync()
    {
        string sql = @"
            SELECT SUM(dv.cantidad) 
            FROM RG_DetalleVenta dv
            JOIN RG_Venta v ON dv.idOrden = v.idOrden
            WHERE MONTH(v.fechaHora) = MONTH(CURRENT_DATE) AND YEAR(v.fechaHora) = YEAR(CURRENT_DATE);";

        return await _dbOperacion.EjecutarEscalarAsync<int>(sql);
    }

    /// <summary>
    /// Obtiene los movimientos en el Kardex.
    /// </summary>
    public async Task<List<KardexMovimiento>> ObtenerMovimientosKardexAsync()
    {
        string sql = @"
            SELECT k.idKardex, k.tipoMovimiento, k.cantidad, k.costoUnitario, k.documentoReferencia, 
                   k.fecha, p.nombreProducto, i.nombreIngrediente
            FROM RG_Kardex k
            LEFT JOIN RG_Producto p ON k.idProducto = p.idProducto
            LEFT JOIN RG_Ingrediente i ON k.idIngrediente = i.idIngrediente
            ORDER BY k.fecha DESC
            LIMIT 10;";

        return await _dbOperacion.Consultar<KardexMovimiento>(sql, MapearKardexMovimiento);
    }

    private VentasPorFecha MapearVentasPorFecha(DataRow row)
    {
        return new VentasPorFecha
        {
            Fecha = Convert.ToDateTime(row["Fecha"]),
            Total = Convert.ToDecimal(row["Total"])
        };
    }

    private ProductoMasVendido MapearProductoMasVendido(DataRow row)
    {
        return new ProductoMasVendido
        {
            
            Nombre = row["nombreProducto"].ToString(),
            Cantidad = Convert.ToInt32(row["TotalVendidos"])
        };
    }
    /// <summary>
    /// Obtiene el total de ventas del día.
    /// </summary>
    public async Task<decimal> ObtenerVentasHoyAsync()
    {
        string sql = @"
        SELECT SUM(total) 
        FROM RG_Venta 
        WHERE DATE(fechaHora) = CURRENT_DATE;";

        return await _dbOperacion.EjecutarEscalarAsync<decimal>(sql);
    }

    /// <summary>
    /// Obtiene la cantidad de mesas actualmente ocupadas.
    /// </summary>
    public async Task<int> ObtenerMesasOcupadasAsync()
    {
        string sql = @"
        SELECT COUNT(*) 
        FROM RG_Mesa 
        WHERE estado = 'Ocupada';";

        return await _dbOperacion.EjecutarEscalarAsync<int>(sql);
    }

    /// <summary>
    /// Obtiene la cantidad de alertas por inventario bajo.
    /// </summary>
    public async Task<int> ObtenerCantidadAlertasInventarioAsync()
    {
        string sql = @"
        SELECT COUNT(*) 
        FROM RG_Ingrediente 
        WHERE stock;";

        return await _dbOperacion.EjecutarEscalarAsync<int>(sql);
    }

    /// <summary>
    /// Obtiene los últimos pedidos registrados.
    /// </summary>
    public async Task<List<Pedido>> ObtenerUltimosPedidosAsync()
    {
        string sql = @"
    SELECT o.idOrden, o.nombreCliente, v.total, o.estado, m.numeroMesa
    FROM RG_Orden o
    JOIN RG_Mesa m ON o.idMesa = m.idMesa
    JOIN RG_Venta v ON o.idOrden = v.idOrden
    ORDER BY o.fechaHora DESC
    LIMIT 10;";

        return await _dbOperacion.Consultar<Pedido>(sql, MapearPedido);
    }


    /// <summary>
    /// Obtiene el inventario crítico.
    /// </summary>
    public async Task<List<Ingrediente>> ObtenerInventarioCriticoAsync()
    {
        string sql = @"
        SELECT nombreIngrediente, stock
        FROM RG_Ingrediente
       ";

        return await _dbOperacion.Consultar<Ingrediente>(sql, MapearIngredienteCritico);
    }

    /// <summary>
    /// Obtiene el estado de todas las mesas.
    /// </summary>
    public async Task<List<Mesa>> ObtenerEstadoMesasAsync()
    {
        string sql = @"
        SELECT idMesa, numeroMesa, estado
        FROM RG_Mesa;";

        return await _dbOperacion.Consultar<Mesa>(sql, MapearMesa);
    }
    private Pedido MapearPedido(DataRow row)
    {
        return new Pedido
        {
            
            NombreCliente = row["nombreCliente"].ToString(),
            Total = Convert.ToDecimal(row["total"]),
            Estado = row["estado"].ToString(),
            Mesa = new Mesa
            {
                NumeroMesa = Convert.ToInt32(row["numeroMesa"])
            }
        };
    }

    private Ingrediente MapearIngredienteCritico(DataRow row)
    {
        return new Ingrediente
        {
            NombreIngrediente = row["nombreIngrediente"].ToString(),
            Stock = Convert.ToDecimal(row["stock"]),
            StockMinimo = 100.0

        };
    }

    private Mesa MapearMesa(DataRow row)
    {
        return new Mesa
        {
            IdMesa = Convert.ToInt32(row["idMesa"]),
            NumeroMesa = Convert.ToInt32(row["numeroMesa"]),
            Estado = row["estado"].ToString()
        };
    }

    private KardexMovimiento MapearKardexMovimiento(DataRow row)
    {
        return new KardexMovimiento
        {
            IdKardex = Convert.ToInt32(row["idKardex"]),
            TipoMovimiento = row["tipoMovimiento"].ToString(),
            Cantidad = Convert.ToInt32(row["cantidad"]),
            CostoUnitario = Convert.ToDecimal(row["costoUnitario"]),
            DocumentoReferencia = row["documentoReferencia"].ToString(),
            Fecha = Convert.ToDateTime(row["fecha"]),
            NombreProducto = row["nombreProducto"] as string,
            NombreIngrediente = row["nombreIngrediente"] as string
        };
    }
}
