using System;
using System.Data;
using System.Text;

using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

public class LoginService
{
    private readonly DBOperacion _db;

    public LoginService()
    {
        _db = new DBOperacion();
    }

    public async Task<UsuarioLogin> AutenticarUsuarioAsync(string usuario, string contrasenia)
    {
        try
        {
            var sql = new StringBuilder(@"
            SELECT u.idUsuario, u.nombreUsuario, u.contrasenia, 
                   u.idRol, u.idEmpleado,
                   e.nombre AS nombreEmpleado, r.nombreRol AS rol
            FROM RG_Usuario u
            INNER JOIN RG_Empleado e ON u.idEmpleado = e.idEmpleado
            INNER JOIN RG_Rol r ON u.idRol = r.idRol
            WHERE u.nombreUsuario = @usuario AND u.contrasenia = @contrasenia;
        ");

            var parametros = new Dictionary<string, object>
        {
            { "@usuario", usuario },
            { "@contrasenia", contrasenia }
        };

            var dt = await _db.Consultar(sql.ToString(), parametros);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];

            return new UsuarioLogin
            {
                IdUsuario = Convert.ToInt32(row["idUsuario"]),
                NombreUsuario = row["nombreUsuario"].ToString(),
                Contraseña = row["contrasenia"].ToString(),
                IdRol = Convert.ToInt32(row["idRol"]),
                IdEmpleado = Convert.ToInt32(row["idEmpleado"]),
                NombreEmpleado = row["nombreEmpleado"].ToString(),
                Rol = row["rol"].ToString()
            };
        }
        catch (Exception ex)
        {
            // Manejo/log de errores
            return null;
        }
    }



}
