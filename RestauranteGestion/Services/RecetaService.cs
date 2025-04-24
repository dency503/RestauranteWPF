using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using Mysqlx;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using RestauranteGestion.ViewModels;
using static RestauranteGestion.ViewModels.EdicionRecetaViewModel;

namespace RestauranteGestion.Services
{
    public class RecetaService
    {
        private readonly DBOperacion _db;

        public RecetaService()
        {
            _db = new DBOperacion();
        }
        public async Task<IEnumerable<IngredienteReceta>> ObtenerRecetaPorProductoAsync(int idProducto)
        {
            const string query = @"
               SELECT r.idIngrediente, r.cantidadNecesaria,  i.nombreIngrediente, i.unidadMedida as  unidad
FROM rg_detallereceta r
Inner Join rg_receta re on re.idReceta = r.idReceta
INNER JOIN rg_ingrediente i ON r.idIngrediente = i.idIngrediente
         WHERE re.idProducto = @idProducto";

            using var db = new DBOperacion();

            var parametros = new Dictionary<string, object>
    {
        { "@idProducto", idProducto }
    };

            return await _db.Consultar(query, row => new IngredienteReceta
            {
                Ingrediente = new Ingrediente
                {
                    IdIngrediente = Convert.ToInt32(row["idIngrediente"]),
                    NombreIngrediente = row["nombreIngrediente"].ToString() ?? ""
                },
                Cantidad = Convert.ToDecimal(row["cantidadNecesaria"]),
                Unidad = new Unidad
                {
                    Nombre = row["unidad"].ToString() ?? ""
                }
            }, parametros);
        }


        public async Task GuardarRecetaAsync(int idProducto, ObservableCollection<IngredienteReceta> ingredientesReceta)
        {
    

            try
            {
                await _db.IniciarTransaccionAsync();

                // Verificar si ya existe receta
                const string selectRecetaSql = "SELECT idReceta FROM RG_Receta WHERE idProducto = @idProducto";
                var recetaExistente = await _db.EjecutarEscalarAsync<int?>(
                    selectRecetaSql,
                    new Dictionary<string, object> { { "@idProducto", idProducto } });

                int idReceta;
                if (recetaExistente.HasValue)
                {
                    idReceta = recetaExistente.Value;
                }
                else
                {
                    const string insertRecetaSql = "INSERT INTO RG_Receta (idProducto) VALUES (@idProducto)";
                    idReceta = await _db.EjecutarSentenciaYObtenerID(
                        insertRecetaSql,
                        new Dictionary<string, object> { { "@idProducto", idProducto } });
                }

                // Eliminar detalle anterior
                const string deleteSql = "DELETE FROM RG_DetalleReceta WHERE idReceta = @idReceta";
                await _db.EjecutarSentencia(deleteSql, new Dictionary<string, object>
        {
            { "@idReceta", idReceta }
        });

                // Insertar nuevos ingredientes
                const string insertSql = @"
            INSERT INTO RG_DetalleReceta (idReceta, idIngrediente, cantidadNecesaria)
            VALUES (@idReceta, @idIngrediente, @cantidad)";

                foreach (var ingrediente in ingredientesReceta)
                {
                    var parametros = new Dictionary<string, object>
            {
                { "@idReceta", idReceta },
                { "@idIngrediente", ingrediente.Ingrediente.IdIngrediente },
                { "@cantidad", ingrediente.Cantidad }
            };

                    await _db.EjecutarSentencia(insertSql, parametros);
                }

                await _db.ConfirmarTransaccionAsync();
            }
            catch (Exception e)
            {
                await _db.RevertirTransaccionAsync();
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }


        public async Task<List<Receta>> ObtenerRecetasAsync()
        {
            string sql = @"
                SELECT 
                    r.idReceta,
                    r.idProducto,
                    p.nombreProducto AS NombreProducto
                FROM rg_receta r
                INNER JOIN rg_producto p ON r.idProducto = p.idProducto;
            ";

            return await _db.Consultar(sql, row => new Receta
            {
                IdReceta = row.Field<int>("idReceta"),
                IdProducto = row.Field<int>("idProducto"),
                NombreReceta = row.Field<string>("NombreProducto")
            });
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            string sql = "SELECT * FROM rg_producto";
            return await _db.Consultar(sql, row => new Producto
            {
                IdProducto = row.Field<int>("idProducto"),
               NombreProducto = row.Field<string>("nombreProducto"),
                PrecioVenta = row.Field<decimal>("precioVenta"),
                // Agrega más campos si es necesario
            });
        }


        public async Task<int> AgregarRecetaAsync(int idProducto)
        {
            string sql = "INSERT INTO rg_receta (idProducto) VALUES (@idProducto)";
            var parametros = new Dictionary<string, object>
            {
                { "@idProducto", idProducto }
            };
            return await _db.EjecutarSentenciaYObtenerID(sql, parametros);
        }

        public async Task<bool> ActualizarRecetaAsync(int idReceta, int idProducto)
        {
            string sql = "UPDATE rg_receta SET idProducto = @idProducto WHERE idReceta = @idReceta";
            var parametros = new Dictionary<string, object>
            {
                { "@idReceta", idReceta },
                { "@idProducto", idProducto }
            };
            return await _db.EjecutarSentencia(sql, parametros) > 0;
        }

        public async Task<bool> EliminarRecetaAsync(int idReceta)
        {
            string sql = "DELETE FROM rg_receta WHERE idReceta = @idReceta";
            var parametros = new Dictionary<string, object>
            {
                { "@idReceta", idReceta }
            };
            return await _db.EjecutarSentencia(sql, parametros) > 0;
        }
    }
}
