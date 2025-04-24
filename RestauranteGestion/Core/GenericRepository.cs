using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RestauranteGestion.Core.DataAccess
{
    public class GenericRepository
    {
        private readonly string _connectionString;

        public GenericRepository()
        {
            _connectionString = "server=localhost;port=3306;database=RestauranteGestion;user=root;password=root;CharSet=utf8mb4";
        }

        private MySqlConnection CrearConexion()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<List<T>> ConsultarAsync<T>(string query, Func<MySqlDataReader, T> map, Dictionary<string, object>? parametros = null)
        {
            var resultados = new List<T>();

            using var conn = CrearConexion();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query, conn);

            if (parametros != null)
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultados.Add(map((MySqlDataReader)reader));
            }

            return resultados;
        }

        public async Task<int> EjecutarAsync(string query, Dictionary<string, object>? parametros = null)
        {
            using var conn = CrearConexion();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(query, conn);

            if (parametros != null)
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
