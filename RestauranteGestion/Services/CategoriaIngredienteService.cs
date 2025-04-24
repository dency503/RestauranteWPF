using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;

namespace RestauranteGestion.Services
{
    public class CategoriaIngredienteService
    {
        private readonly DBOperacion _repo;

        public CategoriaIngredienteService()
        {
            _repo = new DBOperacion();
        }

        public async Task<List<CategoriaIngrediente>> ObtenerCategoriasAsync()
        {
            var query = "SELECT * FROM RG_CategoriaIngrediente";
            var categorias = await _repo.QueryAsync<CategoriaIngrediente>(query);
            return categorias.ToList();
        }

        public async Task<int> AgregarCategoriaAsync(CategoriaIngrediente categoria)
        {
            var query = @"INSERT INTO RG_CategoriaIngrediente(nombreCategoria)
                          VALUES(@nombreCategoria)";

            var parametros = new Dictionary<string, object>
            {
                { "@nombreCategoria", categoria.NombreCategoria }
            };

            return await _repo.EjecutarSentenciaYObtenerID(query, parametros);
        }

        public async Task<int> ActualizarCategoriaAsync(CategoriaIngrediente categoria)
        {
            var query = @"UPDATE RG_CategoriaIngrediente 
                          SET nombreCategoria = @nombreCategoria 
                          WHERE idCategoria = @idCategoria";

            var parametros = new Dictionary<string, object>
            {
                { "@nombreCategoria", categoria.NombreCategoria },
                { "@idCategoria", categoria.IdCategoria }
            };

            return await _repo.EjecutarSentencia(query, parametros);
        }
        public async Task<List<CategoriaIngrediente>> ObtenerCategoriasPorFechaAsync(DateTime fecha)
        {
            var query = "SELECT * FROM RG_CategoriaIngrediente WHERE DATE(fechaCreacion) = @fecha";
            var parametros = new Dictionary<string, object>
    {
        { "@fecha", fecha.ToString("yyyy-MM-dd") }
    };

            return await _repo.Consultar(query, row => new CategoriaIngrediente
            {
                IdCategoria = Convert.ToInt32(row["idCategoria"]),
                NombreCategoria = row["nombreCategoria"].ToString()
                // Agrega otras propiedades si las tienes
            }, parametros);
        }



        public async Task<int> EliminarCategoriaAsync(int idCategoria)
        {
            var query = "DELETE FROM RG_CategoriaIngrediente WHERE idCategoria = @idCategoria";
            var parametros = new Dictionary<string, object>
            {
                { "@idCategoria", idCategoria }
            };

            return await _repo.EjecutarSentencia(query, parametros);
        }
    }
}
