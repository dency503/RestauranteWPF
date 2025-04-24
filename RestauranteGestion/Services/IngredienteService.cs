using RestauranteGestion.Core;
using RestauranteGestion.Core.DataAccess;
using RestauranteGestion.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestauranteGestion.Services
{
    public class IngredienteService
    {
        private readonly DBOperacion _repo;

        public IngredienteService()
        {
            _repo = new DBOperacion();
        }

        public async Task<List<Ingrediente>> GetIngredientesAsync()
        {
            var query = "SELECT * FROM RG_Ingrediente";
            var ingredientes = await _repo.QueryAsync<Ingrediente>(query);
            return ingredientes.ToList(); // Convertir explícitamente a List<Ingrediente>
        }


        public async Task AddIngredienteAsync(Ingrediente ingrediente)
        {
            string query = @"INSERT INTO RG_Ingrediente 
                            (nombreIngrediente, idCategoria, precio, imagen, stock, unidadMedida) 
                            VALUES (@nombre, @categoria, @precio, @imagen, @stock, @unidad)";

            var parametros = new
            {
                nombre = ingrediente.NombreIngrediente,
                categoria = ingrediente.IdCategoria,
                precio = ingrediente.Precio,
                imagen = ingrediente.Imagen,
                stock = ingrediente.Stock,
                unidad = ingrediente.UnidadMedida
            };

            await _repo.EjecutarAsync(query, parametros);
        }
        
        public async Task EditIngredienteAsync(Ingrediente ingrediente)
        {
            string query = @"UPDATE RG_Ingrediente SET 
                            nombreIngrediente = @nombre,
                            idCategoria = @categoria,
                            precio = @precio,
                            imagen = @imagen,
                            stock = @stock,
                            unidadMedida = @unidad
                            WHERE idIngrediente = @id";

            var parametros = new
            {
                nombre = ingrediente.NombreIngrediente,
                categoria = ingrediente.IdCategoria,
                precio = ingrediente.Precio,
                imagen = ingrediente.Imagen,
                stock = ingrediente.Stock,
                unidad = ingrediente.UnidadMedida,
                id = ingrediente.IdIngrediente
            };

            await _repo.EjecutarAsync(query, parametros);
        }

        public async Task DeleteIngredienteAsync(int id)
        {
            string query = "DELETE FROM RG_Ingrediente WHERE idIngrediente = @id";
            var parametros = new { id };

            await _repo.EjecutarAsync(query, parametros);
        }
    }
}
