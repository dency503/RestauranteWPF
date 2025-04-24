using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace RestauranteGestion
{
    public static class AppConfig
    {
        private static IConfiguration _configuration;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }

        public static string ConnectionString => Configuration.GetConnectionString("MySQLConnection");

        public static int TimeoutConexion =>
            int.TryParse(Configuration["ConfiguracionApp:TimeoutConexion"], out var timeout) ? timeout : 30;

        public static bool HabilitarLogging =>
            bool.TryParse(Configuration["ConfiguracionApp:HabilitarLogging"], out var habilitar) && habilitar;
    }
}