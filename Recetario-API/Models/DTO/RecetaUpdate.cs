namespace Recetario_API.Models.DTO
{
    public class RecetaUpdate
    {
        public RecetaUpdate()
        {
            ListaIngredientes = new HashSet<IngredientesDto>();
            Preparacion = new HashSet<PreparacionDto>();
        }
        public string Nombre { get; set; }
        public ICollection<IngredientesDto> ListaIngredientes { get; set; }
        public ICollection<PreparacionDto> Preparacion { get; set; }
        public IFormFile? Imagen { get; set; }
        public int TiempoPreparacion { get; set; }
        public int TiempoCoccion { get; set; }
    }
}
