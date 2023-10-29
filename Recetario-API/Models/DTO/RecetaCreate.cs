using Newtonsoft.Json;
using Recetario_API.Models.Usuarios;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.Swagger;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recetario_API.Models.DTO
{
    public class RecetaCreate
    {
        //public string Nombre { get; set; }
        ////public IEnumerable<IngredientesDto> ListaIngredientes { get; set; } = new List<IngredientesDto> { new IngredientesDto() };
        ////public IEnumerable<PreparacionDto> Preparacion { get; set; } = new List<PreparacionDto> { new PreparacionDto() };
        //public List<List<string>> ListaIngredientes { get; set; }
        //public List<string> Preparacion { get; set; }
        public RecetaCreate()
        {
            ListaIngredientes = new HashSet<IngredientesDto>();
            Preparacion = new HashSet<PreparacionDto>();
        }

        public string Nombre { get; set; }
        public ICollection<IngredientesDto> ListaIngredientes { get; set; }
        public ICollection<PreparacionDto> Preparacion { get; set; }
        public IFormFile? Imagen { get; set; }
        public int TiempoPreparacion { get; set; }
        public int TiempoCoccion { get; set; }
        
    }
}
