using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Recetario_API.Models.Usuarios
{
    public class UserUpdate
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas deben de ser iguales")]
        public string ConfirmarPassword { get; set; }

        public IFormFile Imagen { get; set; }

    }
}
