using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RestauranteGestion.Core.DataAccess
{
    /// <summary>
    /// Proporciona operaciones genéricas de base de datos (CRUD, transacciones, consultas) sobre MySQL.
    /// Hereda de DBConexion para gestionar la conexión.
    /// </summary>
    public class DBOperacion : DBConexion, IDisposable
    {
        private MySqlTransaction _transaction;
        private bool _disposed;

        /// <summary>
        /// Crea una nueva instancia de DBOperacion con la cadena de conexión especificada.
        /// </summary>
        public DBOperacion() : base() { }

        /// <summary>
        /// Asegura que la conexión esté abierta, conectando si es necesario.
        /// </summary>
        private async Task EnsureConnectionOpenAsync()
        {
            if (_CONEXION == null || _CONEXION.State != ConnectionState.Open)
            {
                await ConectarAsync();
            }
        }

        /// <summary>
        /// Inicia una transacción en la conexión actual.
        /// </summary>
        public async Task IniciarTransaccionAsync()
        {
            await EnsureConnectionOpenAsync();
            if (_transaction == null)
            {
                _transaction = await _CONEXION.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// Confirma (commit) la transacción activa.
        /// </summary>
        public async Task ConfirmarTransaccionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Revierte (rollback) la transacción activa.
        /// </summary>
        public async Task RevertirTransaccionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Ejecuta una sentencia SQL (INSERT, UPDATE, DELETE) y retorna el número de filas afectadas.
        /// </summary>
        public async Task<int> EjecutarSentencia(string sql, Dictionary<string, object> parametros = null)
        {
            await EnsureConnectionOpenAsync();
            using var cmd = CrearComando(sql, parametros);
            return await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Ejecuta una sentencia INSERT y retorna el ID generado (LAST_INSERT_ID).
        /// </summary>
        public async Task<int> EjecutarSentenciaYObtenerID(string sql, Dictionary<string, object> parametros = null)
        {
            await EnsureConnectionOpenAsync();
            using var cmd = CrearComando(sql, parametros);
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT LAST_INSERT_ID();";
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Ejecuta una consulta y retorna los resultados en un DataTable.
        /// </summary>
        /// 
        public async Task<DataTable> Consultar(string sql, Dictionary<string, object> parametros = null)
        {
            await EnsureConnectionOpenAsync();
            var dt = new DataTable();
            using var cmd = CrearComando(sql, parametros);
            using var adapter = new MySqlDataAdapter(cmd);
            await Task.Run(() => adapter.Fill(dt));
            return dt;
        }
        public async Task<List<T>> Consultar<T>(string sql, Func<DataRow, T> map, Dictionary<string, object>? parametros = null)
        {
            await EnsureConnectionOpenAsync();

            var dt = new DataTable();
            using var cmd = CrearComando(sql, parametros);
            using var adapter = new MySqlDataAdapter(cmd);
            await Task.Run(() => adapter.Fill(dt));

            var lista = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(map(row));
            }

            return lista;
        }


        /// <summary>
        /// Ejecuta una consulta que devuelve un único registro y lo mapea al tipo T.
        /// </summary>
        public async Task<T> QuerySingleAsync<T>(string sql, object parametros = null) where T : class, new()
        {
            await EnsureConnectionOpenAsync();
            using var cmd = CrearComando(sql, ConvertToDictionary(parametros));
            using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapearReaderATipo<T>(reader);
            }
            return null;
        }

        /// <summary>
        /// Ejecuta una consulta que devuelve múltiples registros y los mapea al tipo T.
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parametros = null) where T : class, new()
        {
            await EnsureConnectionOpenAsync();
            var lista = new List<T>();
            using var cmd = CrearComando(sql, ConvertToDictionary(parametros));
            using var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearReaderATipo<T>(reader));
            }
            return lista;
        }

        /// <summary>
        /// Ejecuta una consulta escalar y retorna el valor convertido a T.
        /// </summary>
        public async Task<T> EjecutarEscalarAsync<T>(string sql, object parametros = null)
        {
            await EnsureConnectionOpenAsync();
            using var cmd = CrearComando(sql, ConvertToDictionary(parametros));
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return default;

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            return (T)Convert.ChangeType(result, targetType);
        }


        /// <summary>
        /// Ejecuta una sentencia con opciones avanzadas (tipo de comando, timeout).
        /// </summary>
        public async Task<int> EjecutarAsync(string sql, object parametros = null,
                                            CommandType commandType = CommandType.Text,
                                            int? timeout = null)
        {
            await EnsureConnectionOpenAsync();
            using var cmd = CrearComando(sql, parametros, commandType, timeout);
            return await cmd.ExecuteNonQueryAsync();
        }

        #region Helpers internos

        private MySqlCommand CrearComando(string sql, Dictionary<string, object> parametros)
        {
            var cmd = new MySqlCommand(sql, _CONEXION, _transaction);
            if (parametros != null)
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            return cmd;
        }

        private MySqlCommand CrearComando(string sql, object parametros, CommandType commandType, int? timeout)
        {
            var cmd = new MySqlCommand(sql, _CONEXION, _transaction)
            {
                CommandType = commandType
            };
            if (timeout.HasValue)
            {
                cmd.CommandTimeout = timeout.Value;
            }
            var dict = ConvertToDictionary(parametros);
            if (dict != null)
            {
                foreach (var param in dict)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            return cmd;
        }

        private Dictionary<string, object> ConvertToDictionary(object parametros)
        {
            if (parametros == null) return null;
            if (parametros is Dictionary<string, object> dict) return dict;

            var dictionary = new Dictionary<string, object>();
            foreach (var prop in parametros.GetType().GetProperties())
            {
                var key = prop.Name.StartsWith("@") ? prop.Name : $"@{prop.Name}";
                dictionary.Add(key, prop.GetValue(parametros));
            }
            return dictionary;
        }

        private T MapearReaderATipo<T>(MySqlDataReader reader) where T : class, new()
        {
            var obj = new T();
            var props = typeof(T).GetProperties();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var colName = reader.GetName(i);
                var prop = Array.Find(props, p => p.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));
                if (prop != null && !reader.IsDBNull(i))
                {
                    var val = reader.GetValue(i);
                    prop.SetValue(obj, val is DBNull ? null : val);
                }
            }
            return obj;
        }

        #endregion

        /// <summary>
        /// Libera recursos de transacción y conexión.
        /// </summary>
        public new void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                base.Dispose();
                _disposed = true;
            }
        }
    }
}
