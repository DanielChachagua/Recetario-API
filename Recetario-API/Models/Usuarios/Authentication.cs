using System.ComponentModel.DataAnnotations;

namespace Recetario_API.Models.Usuarios
{
    public class Authentication
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
