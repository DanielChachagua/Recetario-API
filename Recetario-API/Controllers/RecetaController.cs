using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.DTO;
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

        [HttpPost("crear")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecetaDto>> CrearReceta([FromForm] RecetaCreate receta)
        {
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

            var newReceta = await _recetaService.CreateReceta(receta, _dbContext);
            return CreatedAtRoute("GetReceta", new {id = newReceta},receta); 
           
        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteReceta(int id)
        {
            if (id == 0) return BadRequest();

            var result = _recetaService.DeleteReceta(id, _dbContext);

            if (result.Result == false ) return NotFound();

            return NoContent();

        }

        [HttpPut("id:int")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateReceta(int id, [FromForm] RecetaUpdate receta)
        {
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

            if (receta == null || id == 0) return BadRequest();

            var result = _recetaService.UpdateReceta(id, receta, _dbContext);

            if (result == null) return NotFound();

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
