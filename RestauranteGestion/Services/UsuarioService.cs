using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestauranteGestion.Core.DataAccess;

public class UsuarioService
{
    private readonly DBOperacion _db;

    public UsuarioService()
    {
        _db = new DBOperacion();
    }

    public async Task<string> ObtenerContrasenaActualAsync(int idUsuario)
    {
        var sql = "SELECT contrasenia FROM RG_Usuario WHERE idUsuario = @idUsuario;";
        var parametros = new Dictionary<string, object>
        {
            { "@idUsuario", idUsuario }
        };

        var dt = await _db.Consultar(sql, parametros);

        if (dt.Rows.Count == 0)
            return null;

        return dt.Rows[0]["contrasenia"].ToString();
    }

    public async Task<bool> CambiarContrasenaAsync(int idUsuario, string nuevaContrasena)
    {
        var sql = "UPDATE RG_Usuario SET contrasenia = @nuevaContrasenia WHERE idUsuario = @idUsuario;";
        var parametros = new Dictionary<string, object>
        {
            { "@nuevaContrasenia", nuevaContrasena },
            { "@idUsuario", idUsuario }
        };

        var filasAfectadas = await _db.EjecutarAsync(sql, parametros);

        return filasAfectadas > 0;
    }
}
