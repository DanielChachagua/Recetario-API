using System.ComponentModel.DataAnnotations;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Models.DTO
{
    public class RecetaDto
    {
        public int Id { get; set; }
        [Required] [MaxLength(200)] 
        public string Nombre { get; set; }
        [Required]
        public IEnumerable<IngredientesDto> ListaIngredientes { get; set;}

        [Required][MaxLength(10, ErrorMessage = "La lista de preparación no puede tener más de 10 elementos.")] 
        public IEnumerable<PreparacionDto> Preparacion { get; set; }
        public string? Imagen { get; set; }
        [Required][Range(1, 600)] 
        public int TiempoPreparacion { get; set; }
        [Required][Range(1, 600)] 
        public int TiempoCoccion { get; set; }
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; } = DateTime.Now;
        [Required] 
        public UserDto UserCreador { get; set; }
    }
}
