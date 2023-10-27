using System.Security.Claims;
using Recetario_API.Data;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Models
{
    public class JWT
    {
        private readonly DataBaseContext _context;
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        public JWT(DataBaseContext context)
        {
            _context = context;
        }

        public dynamic ValidarToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        message = "Verificar si el token es válido",
                        result = ""
                    };
                }

                var id = identity.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

                if (id == null)
                {
                    return new
                    {
                        success = false,
                        message = "El token no contiene un ID de usuario válido",
                        result = ""
                    };
                }

                var usuario = _context.Users.Find(id);
                Console.WriteLine(usuario.Id);

                if (usuario == null)
                {
                    return new
                    {
                        success = false,
                        message = "Usuario no encontrado en la base de datos",
                        result = ""
                    };
                }

                return new
                {
                    success = true,
                    message = "Éxito",
                    result = usuario
                };

            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Error: " + ex.Message,
                    result = ""
                };
            }
        }
    }

}
