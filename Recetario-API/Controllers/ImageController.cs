using Microsoft.AspNetCore.Mvc;
using Recetario_API.Models.DTO;

namespace Recetario_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet("string:ruta", Name = "GetImagen")]
        public async Task<ActionResult<RecetaDto>> GetImagen(string ruta)
        {

            // Determina el tipo de contenido basado en la extensión del archivo
            string contentType;
            if (Path.GetExtension(ruta).Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/png";
            }
            else if (Path.GetExtension(ruta).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(ruta).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }
            else if (Path.GetExtension(ruta).Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/gif";
            }
            else
            {
                // Si la extensión del archivo no es reconocida, devuelve un tipo de contenido genérico
                contentType = "application/octet-stream";
            }

            // Lee la imagen como un arreglo de bytes
            byte[] imageBytes = System.IO.File.ReadAllBytes(ruta);

            // Devuelve la imagen con el tipo de contenido adecuado
            return File(imageBytes, contentType);

        }
    }
}
