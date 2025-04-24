using MySql.Data.MySqlClient;

using System;
using System.Data;
using System.Threading.Tasks;

namespace RestauranteGestion.Core.DataAccess
{
    public class DBConexion : IDisposable
    {
        protected MySqlConnection _CONEXION;
        private readonly string _connectionString;

        public DBConexion() : this("server=localhost;port=3306;database=RestauranteGestion;user=root;password=root;CharSet=utf8mb4") { }

        public DBConexion(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> ConectarAsync()
        {
            try
            {
                _CONEXION = new MySqlConnection(_connectionString);
                await _CONEXION.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar: {ex.Message}");
                return false;
            }
        }

        public async Task DesconectarAsync()
        {
            try
            {
                if (_CONEXION?.State == ConnectionState.Open)
                {
                    await _CONEXION.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desconectar: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _CONEXION?.Dispose();
        }
    }
}