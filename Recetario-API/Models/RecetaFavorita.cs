using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Models
{
    [Table("Favorita")]
    public class RecetaFavorita
    {
        [Key]
        public int Id { get; init; }
        public bool Favorita { get; init; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        [ForeignKey("Receta")]
        public int RecetaId { get; set; }
    }
}
