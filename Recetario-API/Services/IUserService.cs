using Recetario_API.Data;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUser(int id, DataBaseContext _dbContext);
        Task<UserDto> UpdateUser(int id,UserUpdate usuario, DataBaseContext _dbContext);
        Task<UserDto?> RegisterUSer(UserRegister usuario, DataBaseContext _dbContext);
        Task<dynamic> Login(Authentication authentication, DataBaseContext _dbContext, IConfiguration _configuration);
        //Task<UserDto> Logout(int id);

    }
}
