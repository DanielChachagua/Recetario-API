using System.Data.Entity;
using Recetario_API.Data;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Services
{
    public class UserService : IUserService
    {
        

        //public UserService(DataBaseContext context)
        //{
            
        //}
        public async Task<UserDto?> GetUser(int id, DataBaseContext _dbContext)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Imagen = user.Imagen,
            };
        }

        public async Task<UserDto?> Login(Authentication authentication, DataBaseContext _dbContext)
        {
            var user = await _dbContext.Users.FindAsync(authentication.Email);

            if (user == null) return null;

            if (PasswordHash.VerifyPassword(authentication.Password, user.Password))
            {
                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Imagen = user.Imagen,
                };
            }
            return null;
        }

        //public Task<UserDto> Logout(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<UserDto?> RegisterUSer(UserRegister usuario, DataBaseContext _dbContext)
        {
            var newUsuario = new User
            {
                Username = usuario.Username,
                FirstName = usuario.FirstName,
                LastName = usuario.LastName,
                Email = usuario.Email,
                Imagen = "default.png",
                Password = PasswordHash.HashPassword(usuario.Password),
                Actualizado = DateTime.UtcNow,
            };
            _dbContext.Users.Add(newUsuario);
            _dbContext.SaveChanges();
            Console.WriteLine("hola");
            var ultimoUsuario = _dbContext.Users
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();
            return new UserDto
            {
                Id = ultimoUsuario.Id,
                Username = ultimoUsuario.Username,
                FirstName = ultimoUsuario.FirstName,
                LastName = ultimoUsuario.LastName,
                Imagen = ultimoUsuario.Imagen
            };
        }

        public async Task<UserDto> UpdateUser(int id, UserUpdate usuario, DataBaseContext _dbContext)
        {
            //var user = await _context.User.Where(u => u.Id == id).FirstOrDefaultAsync();

            var user = _dbContext.Users.Find(id);
            if (user == null) return null;
            if (usuario == null) return null;
            // Verificar si la imagen no es null y tiene datos
            if (usuario.Imagen != null && usuario.Imagen.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(usuario.Imagen.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "UserPhotos", user.Username);

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await usuario.Imagen.CopyToAsync(stream);
                }
                var imagenUrl = Path.Combine("Imagenes", "UserPhotos", user.Username, fileName).Replace("\\", "/");
                user.Imagen = imagenUrl;
            }

            user.FirstName = usuario.FirstName;
            user.LastName = usuario.LastName;
            if (usuario.Password != null && usuario.Password == usuario.ConfirmarPassword)
            {
                string passwordHash = PasswordHash.HashPassword(usuario.Password);
                user.Password = passwordHash;
            }
            user.Actualizado = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Imagen = user.Imagen,
            };

        }


    }
}
