using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RestauranteGestion.Core
{
    public class DatabaseConnection
    {
        private string connectionString = "Server=localhost;Database=RestauranteGestion;Uid=root;Pwd=root;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
