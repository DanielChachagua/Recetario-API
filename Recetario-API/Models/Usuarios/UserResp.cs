namespace Recetario_API.Models.Usuarios
{
    public class UserResp
    {
       public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Imagen { get; set; }
    }
}
