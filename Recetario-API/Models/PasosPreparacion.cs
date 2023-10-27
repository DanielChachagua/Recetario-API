using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Recetario_API.Models
{
    [Table("PasosPreparacion")]
    public class PasosPreparacion
    {
        [Key]
        public int Id { get; set; }
        public string PasoPreparacion { get; set; }
        //[ForeignKey("Receta")]
        //public int RecetaId { get; set; }
        [ForeignKey("RecetaId")]
        public int RecetaId { get; set; }
        [JsonIgnore]
        public virtual Receta? Receta { get; set; }
    }
}
