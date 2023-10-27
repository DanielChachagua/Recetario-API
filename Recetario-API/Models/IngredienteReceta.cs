using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Recetario_API.Models
{
    [Table("Ingrediente")]
    public class IngredienteReceta
    {
        [Key]
        public int Id { get; set; }
        public double Cantidad { get; set; }
        public string Unidad { get; set; }
        public string Ingrediente { get; set; }
        //[ForeignKey("Receta")]
        //public int RecetaId { get; set; }
        [ForeignKey("RecetaId")]
        public int RecetaId { get; set; }
        [JsonIgnore]
        public virtual Receta? Receta { get; set; }
    }
}
