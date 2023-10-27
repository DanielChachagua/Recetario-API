using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recetario_API.Models.Usuarios;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Recetario_API.Models
{
    [Table("Receta")]
    public partial class Receta
    {
        public Receta ()
        {
            Ingredientes = new HashSet<IngredienteReceta>();
            Preparaciones = new HashSet<PasosPreparacion>();
        }
 
        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        //public List<IngredienteReceta> ListaIngredientes { get; set; }
        [InverseProperty("Receta")]
        public virtual ICollection<IngredienteReceta> Ingredientes { get; set; }
        //public List<PasosPreparacion> Preparacion { get; set; }
        [InverseProperty("Receta")]
        public virtual ICollection<PasosPreparacion> Preparaciones { get; set; }
        public string? Imagen { get; set; }
        public int TiempoPreparacion { get; set; }
        public int TiempoCoccion { get; set;  }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        //[ForeignKey("User")]
        //public int UserId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User? UsuarioCreador { get; set; }
    }
}
