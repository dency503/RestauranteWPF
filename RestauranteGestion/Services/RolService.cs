using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

namespace RestauranteGestion.Services
{
    public class RolService
    {
        private readonly DBOperacion _db;

        public RolService()
        {
            _db = new DBOperacion();
        }

        public async Task<IEnumerable<Rol>> ObtenerRolesAsync()
        {
            string query = "SELECT idRol, nombreRol FROM rg_rol";
            return await _db.Consultar(query, row => new Rol
            {
                IdRol = row.Field<int>("idRol"),
                NombreRol = row.Field<string>("nombreRol")
            });
        }

        public async Task<int> AgregarRolAsync(string nombreRol)
        {
            string query = "INSERT INTO rg_rol (nombreRol) VALUES (@nombreRol)";
            var parametros = new Dictionary<string, object>
            {
                { "@nombreRol", nombreRol }
            };
            return await _db.EjecutarSentenciaYObtenerID(query, parametros);
        }

        public async Task<bool> ActualizarRolAsync(int idRol, string nombreRol)
        {
            string query = "UPDATE rg_rol SET nombreRol = @nombreRol WHERE idRol = @idRol";
            var parametros = new Dictionary<string, object>
            {
                { "@nombreRol", nombreRol },
                { "@idRol", idRol }
            };
            return await _db.EjecutarSentencia(query, parametros) > 0;
        }

        public async Task<bool> EliminarRolAsync(int idRol)
        {
            string query = "DELETE FROM rg_rol WHERE idRol = @idRol";
            var parametros = new Dictionary<string, object>
            {
                { "@idRol", idRol }
            };
            return await _db.EjecutarSentencia(query, parametros) > 0;
        }
    }
}
