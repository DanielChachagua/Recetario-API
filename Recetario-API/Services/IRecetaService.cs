using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.DTO;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Services
{
    public interface IRecetaService
    {
        Task<RecetaDto?> GetReceta(int id, DataBaseContext _dbContext);
        Task<IEnumerable<RecetaDto>> GetRecetas(DataBaseContext _dbContext);
        Task<int> CreateReceta(RecetaCreate receta, DataBaseContext _dbContext);
        Task<RecetaDto> UpdateReceta(int id, RecetaUpdate receta, DataBaseContext _dbContext);
        Task<bool> DeleteReceta(int id, DataBaseContext _dbContext);
    }
}
