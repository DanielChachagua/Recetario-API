namespace Recetario_API.Models.DTO
{
    public class RecetaUpdate
    {
        public string Nombre { get; set; }
        public List<IngredienteReceta> ListaIngredientes { get; set; }
        public List<PasosPreparacion> Preparacion { get; set; }
        public IFormFile? Imagen { get; set; }
        public int TiempoPreparacion { get; set; }
        public int TiempoCoccion { get; set; }
    }
}
