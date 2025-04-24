using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public class MesaService
{
    private readonly DBOperacion _dbOperacion;

    public MesaService()
    {
        _dbOperacion = new DBOperacion();
    }

    #region Obtener Mesas
    public async Task<List<Mesa>> ObtenerMesasAsync()
    {
        string sql = "SELECT idMesa, numeroMesa, capacidad, estado FROM RG_Mesa";
        return await _dbOperacion.Consultar(sql, MapearMesa);
    }

    public async Task<Mesa> ObtenerMesaPorIdAsync(int idMesa)
    {
        string sql = "SELECT idMesa, numeroMesa, capacidad, estado FROM RG_Mesa WHERE idMesa = @idMesa";
        var parametros = new Dictionary<string, object> { { "@idMesa", idMesa } };

        var mesas = await _dbOperacion.Consultar(sql, MapearMesa, parametros);
        return mesas.Count > 0 ? mesas[0] : null;
    }
    #endregion

    #region Agregar Mesa
    public async Task<int> AgregarMesaAsync(Mesa mesa)
    {
        string sql = "INSERT INTO RG_Mesa (numeroMesa, capacidad, estado) VALUES (@numeroMesa, @capacidad, @estado);";

        var parametros = new Dictionary<string, object>
        {
            { "@numeroMesa", mesa.NumeroMesa },
            { "@capacidad", mesa.Capacidad },
            { "@estado", mesa.Estado }
        };

        return await _dbOperacion.EjecutarSentenciaYObtenerID(sql, parametros);
    }
    #endregion

    #region Editar Mesa
    public async Task EditarMesaAsync(Mesa mesa)
    {
        string sql = @"UPDATE RG_Mesa 
                       SET numeroMesa = @numeroMesa, 
                           capacidad = @capacidad, 
                           estado = @estado 
                       WHERE idMesa = @idMesa;";

        var parametros = new Dictionary<string, object>
        {
            { "@idMesa", mesa.IdMesa },
            { "@numeroMesa", mesa.NumeroMesa },
            { "@capacidad", mesa.Capacidad },
            { "@estado", mesa.Estado }
        };

        await _dbOperacion.EjecutarAsync(sql, parametros);
    }
    #endregion

    #region Eliminar Mesa
    public async Task<bool> EliminarMesaAsync(int idMesa)
    {
        string sql = "DELETE FROM RG_Mesa WHERE idMesa = @idMesa;";
        var parametros = new Dictionary<string, object> { { "@idMesa", idMesa } };

        int filasAfectadas = await _dbOperacion.EjecutarAsync(sql, parametros);
        return filasAfectadas > 0;
    }
    #endregion

    #region Mapear Mesa
    private Mesa MapearMesa(DataRow row)
    {
        var estado = row["estado"].ToString();
        System.Diagnostics.Debug.WriteLine($"Estado: {estado}");
        return new Mesa
        {
            IdMesa = Convert.ToInt32(row["idMesa"]),
            NumeroMesa = Convert.ToInt32(row["numeroMesa"]),
            Capacidad = Convert.ToInt32(row["capacidad"]),
            Estado = row["estado"].ToString().Trim().ToUpper()

        };
    }
    #endregion
}
