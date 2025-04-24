using MySql.Data.MySqlClient;
using RestauranteGestion.Core;
using RestauranteGestion.Models;
using System.Collections.Generic;
using System.Data;

namespace RestauranteGestion.Data
{
    public class OrdenRepository
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

        public List<Orden> ObtenerOrdenesActivas()
        {
            var ordenes = new List<Orden>();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT idOrden, nombreCliente, estado, fechaHora 
                                 FROM RG_Orden 
                                 WHERE estado IN ('Pendiente', 'En preparacion');";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ordenes.Add(new Orden
                        {
                            IdOrden = reader.GetInt32("idOrden"),
                            NombreCliente = reader.IsDBNull("nombreCliente") ? "Sin nombre" : reader.GetString("nombreCliente"),
                            Estado = reader.GetString("estado"),
                            FechaHora = reader.GetDateTime("fechaHora")
                        });
                    }
                }
            }

            return ordenes;
        }
    }
}
