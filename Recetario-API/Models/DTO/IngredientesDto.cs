using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Recetario_API.Models.DTO
{
    public class IngredientesDto
    {
        public double Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Ingrediente { get; set; }
    }
}
