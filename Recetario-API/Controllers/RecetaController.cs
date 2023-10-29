using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.DTO;
using Recetario_API.Models.Usuarios;
using Recetario_API.Services;

namespace Recetario_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetaController : ControllerBase
    {
        private readonly IRecetaService _recetaService;
        private readonly IUserService _userService;
        private readonly DataBaseContext _dbContext;
        private readonly ILogger<RecetaController> _logger;

        public RecetaController(ILogger<RecetaController> logger, IRecetaService recetaService, IUserService userService, DataBaseContext dbContext)
        {
            _logger = logger;
            _recetaService = recetaService;
            _userService = userService;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<RecetaDto>> GetRecetas()
        {
            _logger.LogInformation("Obtener todas las recetas");

            try
            {
                var recetas = _recetaService.GetRecetas(_dbContext);
                return Ok(recetas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las recetas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("id:int", Name = "GetReceta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecetaDto>> GetReceta(int id)
        {
            if (id == 0) return BadRequest();
            var receta = await _recetaService.GetReceta(id, _dbContext);
            if (receta == null) return NotFound();
            return Ok(receta);
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecetaDto>> CrearReceta([FromForm] RecetaCreate receta)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = JWT.ValidarToken(identity, _dbContext);
            if (!rToken.success) return Unauthorized(rToken);
            UserResp usuario = rToken.result;

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var listaIngredientes = "[" + ModelState["ListaIngredientes"].AttemptedValue + "]";
            var listaPreparacion = "[" + ModelState["Preparacion"].AttemptedValue + "]";

            List<IngredientesDto> listaIng = JsonConvert.DeserializeObject<List<IngredientesDto>>(listaIngredientes);
            List<PreparacionDto> listaPrep = JsonConvert.DeserializeObject<List<PreparacionDto>>(listaPreparacion);

            foreach (var ingrediente in listaIng)
            {
                receta.ListaIngredientes.Add(new IngredientesDto
                {
                    Cantidad = ingrediente.Cantidad,
                    Unidad = ingrediente.Unidad,
                    Ingrediente = ingrediente.Ingrediente
                });
            }

            foreach (var preparacion in listaPrep)
            {
                receta.Preparacion.Add(new PreparacionDto
                {
                    PasoPreparacion = preparacion.PasoPreparacion
                });
            }
            if (receta == null) return BadRequest(receta);

            var newReceta = await _recetaService.CreateReceta(receta, usuario.Id, _dbContext);
            return CreatedAtRoute("GetReceta", new {id = newReceta},receta); 
           
        }

        [HttpDelete("delete/id:int")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult DeleteReceta(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = JWT.ValidarToken(identity, _dbContext);
            if (!rToken.success) return Unauthorized(rToken);
            UserResp usuario = rToken.result;

            if (id == 0) return BadRequest();

            var result = _recetaService.DeleteReceta(id, usuario.Id, _dbContext);

            if (result.Result.result == "404") return NotFound(result.Result);
            if (result.Result.result == "403") return StatusCode(403, result.Result);

            return NoContent();

        }

        [HttpPut("update/id:int")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult UpdateReceta(int id, [FromForm] RecetaUpdate receta)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = JWT.ValidarToken(identity, _dbContext);
            if (!rToken.success) return Unauthorized(rToken);
            UserResp usuario = rToken.result;

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (receta == null || id == 0) return BadRequest();

            var listaIngredientes = "[" + ModelState["ListaIngredientes"].AttemptedValue + "]";
            var listaPreparacion = "[" + ModelState["Preparacion"].AttemptedValue + "]";

            List<IngredientesDto> listaIng = JsonConvert.DeserializeObject<List<IngredientesDto>>(listaIngredientes);
            List<PreparacionDto> listaPrep = JsonConvert.DeserializeObject<List<PreparacionDto>>(listaPreparacion);

            foreach (var ingrediente in listaIng)
            {
                receta.ListaIngredientes.Add(new IngredientesDto
                {
                    Cantidad = ingrediente.Cantidad,
                    Unidad = ingrediente.Unidad,
                    Ingrediente = ingrediente.Ingrediente
                });
            }

            foreach (var preparacion in listaPrep)
            {
                receta.Preparacion.Add(new PreparacionDto
                {
                    PasoPreparacion = preparacion.PasoPreparacion
                });
            }

            var result = _recetaService.UpdateReceta(id, receta, usuario.Id, _dbContext);

            if (result.Result.result == "404") return NotFound(result.Result);
            if (result.Result.result == "403") return StatusCode(403, result.Result);

            return NoContent();
        }

        //[HttpPatch("id:int")]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public IActionResult PatchReceta(int id, JsonPatchDocument<RecetaDto> receta)
        //{
        //    if (receta == null || id == 0) return BadRequest();

        //    var result = RecetasList.recetaDtos.FirstOrDefault(x => x.Id == id);

        //    if (result == null) return NotFound();

        //    receta.ApplyTo(result, ModelState);

        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    return NoContent();
        //}

    }
}
