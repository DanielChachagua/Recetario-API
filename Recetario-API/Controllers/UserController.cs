using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.DTO;
using Recetario_API.Models.Usuarios;
using Recetario_API.Services;

namespace Recetario_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DataBaseContext _dbContext;
        private readonly ILogger<RecetaController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<RecetaController> logger, IUserService userService, DataBaseContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> login([FromBody] Authentication authentication)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (authentication == null) return BadRequest(authentication);

            var login = _userService.Login(authentication, _dbContext, _configuration);
            if (login.Result.success == false) return BadRequest(login.Result);
            return Ok(login.Result);
        }

        [HttpGet("id:int", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (id == 0) return BadRequest();
            var user = await _userService.GetUser(id, _dbContext);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> CrearUser([FromBody] UserRegister user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (user == null) return BadRequest(user);
            var usuario = await _userService.RegisterUSer(user, _dbContext);
            return CreatedAtRoute("GetUser", new { id = usuario.Id }, usuario);
        }

        [HttpPut("id:int")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdate user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (user == null) return BadRequest();

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = JWT.ValidarToken(identity, _dbContext);
            if (!rToken.success) return Unauthorized(rToken);
            UserResp usuario = rToken.result;

            var result = _userService.UpdateUser(usuario.Id, user, _dbContext);

            if (result.Result.result == "404") return NotFound(result.Result);

            return NoContent();
        }

    }
}
