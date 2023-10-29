using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.Usuarios;
using Microsoft.AspNetCore.Http;

namespace Recetario_API.Services
{
    public class UserService : IUserService
    {
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

        public async Task<dynamic> Login(Authentication authentication, DataBaseContext _dbContext, IConfiguration _configuration)
        {
            var user = _dbContext.Users.Where(u => u.Email == authentication.Email).FirstOrDefault();

            if (user == null) return new
            {
                success = false,
                message = "Credenciales incorrectas",
                result = ""
            };

            if (!PasswordHash.VerifyPassword(authentication.Password, user.Password))
            {
                return new
                {
                    success = false,
                    message = "Credenciales incorrectas",
                    result = ""
                };
            }

            var jwt = _configuration.GetSection("Jwt").Get<JWT>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id",user.Id.ToString()),
                new Claim("username",user.Username),
                new Claim("firstName",user.FirstName),
                new Claim("lastName",user.LastName),
                new Claim("email",user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signIn
                );

            return new
            {
                success = true,
                message = "Exito",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

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

        public async Task<dynamic> UpdateUser(int id, UserUpdate usuario, DataBaseContext _dbContext)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null) return new
            {
                success = false,
                message = "Usuario no encontrado",
                result = "404"
            };

            if (usuario.Imagen != null && usuario.Imagen.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(usuario.Imagen.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "UserPhotos", user.Username);

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await usuario.Imagen.CopyToAsync(stream);
                }
                var imagenUrl = Path.Combine("Images", "UserPhotos", user.Username, fileName).Replace("\\", "/");
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

            return new 
            {
                success = true,
                message = "Usuario actualizado",
                result = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Imagen = user.Imagen,
                }
            };

        }


    }
}
