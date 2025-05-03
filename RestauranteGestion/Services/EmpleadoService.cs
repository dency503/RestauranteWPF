// Services/EmpleadoService.cs
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

namespace RestauranteGestion.Services
{
    public class EmpleadoService
    {
        private readonly DBOperacion _db;

        public EmpleadoService()
        {
            _db = new DBOperacion();
        }

        public async Task<IEnumerable<Empleado>> ObtenerEmpleadosAsync()
        {
            string query = @"SELECT e.*, c.nombreCargo 
                             FROM rg_empleado e 
                             INNER JOIN rg_cargo c ON e.idCargo = c.idCargo";

            return await _db.Consultar(query, row => new Empleado
            {
                IdEmpleado = row.Field<int>("idEmpleado"),
                Nombre = row.Field<string>("nombre"),
                Apellido = row.Field<string>("apellido"),
                Telefono = row.Field<string>("telefono"),
                Direccion = row.Field<string>("direccion"),
                Email = row.Field<string>("email"),
                FechaNacimiento = row.Field<DateTime?>("fechaNacimiento"),
                IdCargo = row.Field<int>("idCargo"),
                NombreCargo = row.Field<string>("nombreCargo")
            });
        }

        public async Task<int> AgregarEmpleadoAsync(Empleado empleado)
        {
            string query = @"INSERT INTO rg_empleado 
                (nombre, apellido, telefono, direccion, email, fechaNacimiento, idCargo) 
                VALUES (@nombre, @apellido, @telefono, @direccion, @email, @fechaNacimiento, @idCargo)";

            var parametros = new Dictionary<string, object>
            {
                { "@nombre", empleado.Nombre },
                { "@apellido", empleado.Apellido },
                { "@telefono", empleado.Telefono },
                { "@direccion", empleado.Direccion },
                { "@email", empleado.Email },
                { "@fechaNacimiento", empleado.FechaNacimiento },
                { "@idCargo", empleado.IdCargo }
            };

            return await _db.EjecutarSentenciaYObtenerID(query, parametros);
        }

        public async Task<bool> ActualizarEmpleadoAsync(Empleado empleado)
        {
            string query = @"UPDATE rg_empleado SET 
                nombre = @nombre, apellido = @apellido, telefono = @telefono, direccion = @direccion,
                email = @email, fechaNacimiento = @fechaNacimiento, idCargo = @idCargo
                WHERE idEmpleado = @idEmpleado";

            var parametros = new Dictionary<string, object>
            {
                { "@nombre", empleado.Nombre },
                { "@apellido", empleado.Apellido },
                { "@telefono", empleado.Telefono },
                { "@direccion", empleado.Direccion },
                { "@email", empleado.Email },
                { "@fechaNacimiento", empleado.FechaNacimiento },
                { "@idCargo", empleado.IdCargo },
                { "@idEmpleado", empleado.IdEmpleado }
            };

            return await _db.EjecutarSentencia(query, parametros) > 0;
        }

        public async Task<bool> EliminarEmpleadoAsync(int idEmpleado)
        {
            string query = "DELETE FROM rg_empleado WHERE idEmpleado = @idEmpleado";

            var parametros = new Dictionary<string, object>
            {
                { "@idEmpleado", idEmpleado }
            };

            return await _db.EjecutarSentencia(query, parametros) > 0;
        }
    }
}
