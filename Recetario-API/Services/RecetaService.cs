using System.Data.Entity;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;
using Recetario_API.Data;
using Recetario_API.Models;
using Recetario_API.Models.DTO;
using Recetario_API.Models.Usuarios;

namespace Recetario_API.Services
{
    public class RecetaService : IRecetaService
    {

        public async Task<int> CreateReceta(RecetaCreate receta, int usuarioID, DataBaseContext _dbContext)
        {
            Receta newReceta = new Receta
            {
                Nombre = receta.Nombre,
                Imagen = "Images/RecetaPhotos/default.png",
                TiempoPreparacion = receta.TiempoPreparacion,
                TiempoCoccion = receta.TiempoCoccion,
                UserId = usuarioID
            };
            if (receta.Imagen != null && receta.Imagen.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(receta.Imagen.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "RecetaPhotos");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await receta.Imagen.CopyToAsync(stream);
                }
                var imagenUrl = Path.Combine("Images", "RecetaPhotos", fileName).Replace("\\", "/");
                newReceta.Imagen = imagenUrl;
            }
            _dbContext.Recetas.Add(newReceta);
            await _dbContext.SaveChangesAsync();
            var ultimaReceta = _dbContext.Recetas
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            var user = await _dbContext.Users.FindAsync(ultimaReceta.UserId);
            //// Mapear la entidad Receta a RecetaDto y retornarlo
            int ultimoID = ultimaReceta.Id;

            foreach (var ingrediente in receta.ListaIngredientes)
            {
                var newIngrediente = new IngredienteReceta
                {
                    Cantidad = ingrediente.Cantidad,
                    Unidad = ingrediente.Unidad,
                    Ingrediente = ingrediente.Ingrediente,
                    RecetaId = ultimoID
                };
                _dbContext.IngredientesRecetas.Add(newIngrediente);
            }

            foreach (var preparacion in receta.Preparacion)
            {
                var prep = new PasosPreparacion
                {
                    PasoPreparacion = preparacion.PasoPreparacion,
                    RecetaId = ultimoID
                };
                _dbContext.PasosPreparaciones.Add(prep);
            }

            await _dbContext.SaveChangesAsync();
            return ultimoID;
        }

        public async Task<dynamic> DeleteReceta(int id, int usuarioID, DataBaseContext _dbContext)
        {
            usuarioID =  2;
            var receta = await _dbContext.Recetas.FindAsync(id);
            if (receta == null) return new
            {
                success = false,
                message = "La receta no se ha encontrado en la base de datos",
                result = "404"
            };
            if (receta.UserId == usuarioID)
            {
                _dbContext.Recetas.Remove(receta);
                await _dbContext.SaveChangesAsync();
                return new
                {
                    success = true,
                    message = "Receta Eliminada",
                    result = receta.Nombre
                };
            }
            return new
            {
                success = false,
                message = "No está Autorizado para eliminar esta receta",
                result = "403"
            };
        }

        public async Task<RecetaDto?> GetReceta(int id, DataBaseContext _dbContext)
        {
            Receta receta = await _dbContext.Recetas.FindAsync(id);
            if (receta == null) return null;
            var ingredientes = _dbContext.IngredientesRecetas.Where(x => x.RecetaId == id).ToList();
            var preparaciones = _dbContext.PasosPreparaciones.Where(x => x.RecetaId == id).ToList();
            var user = _dbContext.Users.Find(receta.UserId);

            if (ingredientes == null || preparaciones == null || user == null)
            {
                return null;
            }

            return new RecetaDto
            {
                Id = receta.Id,
                Nombre = receta.Nombre,
                ListaIngredientes = receta.Ingredientes.Select(i => new IngredientesDto
                {
                    Cantidad = i.Cantidad,
                    Unidad = i.Unidad,
                    Ingrediente = i.Ingrediente,
                }).ToList(),
                Preparacion = receta.Preparaciones.Select(p => new PreparacionDto
                {
                    PasoPreparacion = p.PasoPreparacion,
                }).ToList(),
                Imagen = receta.Imagen,
                TiempoPreparacion = receta.TiempoPreparacion,
                TiempoCoccion = receta.TiempoCoccion,
                FechaCreacion = receta.FechaCreacion,
                FechaActualizacion = receta.FechaActualizacion,
                UserCreador = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Imagen = user.Imagen
                }
            };
        }

        public async Task<IEnumerable<RecetaDto>> GetRecetas(DataBaseContext _dbContext)
        {

            var recetas = _dbContext.Recetas
                .Include(r => r.Ingredientes)
                .Include(r => r.Preparaciones)
                .Include(r => r.UsuarioCreador)
                .ToList();

            var recetaDtos = recetas.Select(r => new RecetaDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                // Mapear otras propiedades y DTOs aquí
                ListaIngredientes = r.Ingredientes.Select(i => new IngredientesDto
                {
                    Cantidad = i.Cantidad,
                    Unidad = i.Unidad,
                    Ingrediente = i.Ingrediente,
                }).ToList(),
                Preparacion = r.Preparaciones.Select(p => new PreparacionDto
                {
                    PasoPreparacion = p.PasoPreparacion,
                }).ToList(),
                Imagen = r.Imagen,
                TiempoPreparacion = r.TiempoPreparacion,
                TiempoCoccion = r.TiempoCoccion,
                FechaCreacion = r.FechaCreacion,
                FechaActualizacion = r.FechaActualizacion,
                UserCreador = new UserDto
                {
                    Id = r.UsuarioCreador.Id,
                    Username = r.UsuarioCreador.Username,
                    FirstName = r.UsuarioCreador.FirstName,
                    LastName = r.UsuarioCreador.LastName,
                    Imagen = r.UsuarioCreador.Imagen
                }
            }).ToList();

            return recetaDtos;
        }

        public async Task<dynamic> UpdateReceta(int id, RecetaUpdate receta, int usuarioID, DataBaseContext _dbContext)
        {
            var recetaExiste = _dbContext.Recetas
                               .Include(r => r.UsuarioCreador)
                               .FirstOrDefault(r => r.Id == id);

            if (recetaExiste == null) return new
            {
                success = false,
                message = "La receta no se ha encontrado en la base de datos",
                result = "404"
            };

            if (recetaExiste.UserId == usuarioID)
            {


                if (receta.Imagen != null && receta.Imagen.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(receta.Imagen.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "RecetaPhotos");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await receta.Imagen.CopyToAsync(stream);
                    }

                    var photoUrl = Path.Combine("Images", "RecetaPhotos", fileName).Replace("\\", "/");
                    recetaExiste.Imagen = photoUrl;
                }

                recetaExiste.Nombre = receta.Nombre;
                recetaExiste.TiempoPreparacion = receta.TiempoPreparacion;
                recetaExiste.TiempoCoccion = receta.TiempoCoccion;
                recetaExiste.FechaActualizacion = DateTime.UtcNow;

                _dbContext.Recetas.Update(recetaExiste);

                var ingredientes = _dbContext.IngredientesRecetas.Where(i => i.RecetaId == id).ToHashSet();
                var preparaciones = _dbContext.PasosPreparaciones.Where(i => i.RecetaId == id).ToHashSet();
                _dbContext.IngredientesRecetas.RemoveRange(ingredientes);
                _dbContext.PasosPreparaciones.RemoveRange(preparaciones);

                foreach (var ingrediente in receta.ListaIngredientes)
                {
                    var newIngrediente = new IngredienteReceta
                    {
                        Cantidad = ingrediente.Cantidad,
                        Unidad = ingrediente.Unidad,
                        Ingrediente = ingrediente.Ingrediente,
                        RecetaId = id
                    };
                    _dbContext.IngredientesRecetas.Add(newIngrediente);
                }

                foreach (var preparacion in receta.Preparacion)
                {
                    var prep = new PasosPreparacion
                    {
                        PasoPreparacion = preparacion.PasoPreparacion,
                        RecetaId = id
                    };
                    _dbContext.PasosPreparaciones.Add(prep);
                }

                await _dbContext.SaveChangesAsync();

                return new
                {
                    success = true,
                    message = "Usuario actualizado correctamente",
                    result = new RecetaDto
                    {
                        Nombre = recetaExiste.Nombre,
                        ListaIngredientes = receta.ListaIngredientes,
                        Preparacion = receta.Preparacion,
                        Imagen = recetaExiste.Imagen,
                        TiempoPreparacion = recetaExiste.TiempoPreparacion,
                        TiempoCoccion = recetaExiste.TiempoCoccion,
                        FechaActualizacion = recetaExiste.FechaActualizacion
                    }
                };
                
            }
            return new
            {
                success = true,
                message = "Usuario actualizado correctamente",
                result = "403"
            };
        }
    }
}
