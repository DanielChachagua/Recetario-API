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
        
        public async Task<int> CreateReceta(RecetaCreate receta, DataBaseContext _dbContext)
        {
            Receta newReceta = new Receta
            {
                Nombre = receta.Nombre,
                Imagen = "Imagenes/RecetaPhotos/default.png",
                TiempoPreparacion = receta.TiempoPreparacion,
                TiempoCoccion = receta.TiempoCoccion,
                UserId = receta.UserID
            };
            if (receta.Imagen != null && receta.Imagen.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(receta.Imagen.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "RecetaPhotos");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await receta.Imagen.CopyToAsync(stream);
                }
                var imagenUrl = Path.Combine("Imagenes", "RecetaPhotos", fileName).Replace("\\", "/");
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

            //foreach (var ingrediente in receta.ListaIngredientes)
            //{
            //    foreach (var ing in ingrediente)
            //    {
            //        List<string> i = ing.Split(',').ToList();
            //        var newIngrediente = new IngredienteReceta
            //        {
            //            Cantidad = double.Parse(i[0]),
            //            Unidad = i[1],
            //            Ingrediente = i[2],
            //            RecetaId = ultimoID
            //        };
            //        _dbContext.IngredientesRecetas.Add(newIngrediente);
            //    }
            //}

            //foreach (var preparacion in receta.Preparacion)
            //{
            //    var prep = new PasosPreparacion
            //    {
            //        PasoPreparacion = preparacion,
            //        RecetaId = ultimoID
            //    };
            //    _dbContext.PasosPreparaciones.Add(prep);
            //}
            await _dbContext.SaveChangesAsync();
            return ultimoID;
        }

        public async Task<bool> DeleteReceta(int id, DataBaseContext _dbContext)
        {
            var receta = await _dbContext.Recetas.FindAsync(id);
            if (receta == null) return false;
            _dbContext.Recetas.Remove(receta);
            await _dbContext.SaveChangesAsync();
            return true;
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

        public async Task<RecetaDto> UpdateReceta(int id, RecetaUpdate receta, DataBaseContext _dbContext)
        {
            var rec = await _dbContext.Recetas.FindAsync(id);

            if (receta == null) return null;

            if (receta.Imagen != null && receta.Imagen.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(receta.Imagen.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "RecetaImage", "RecetaPhotos", receta.Nombre);

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await receta.Imagen.CopyToAsync(stream);
                }

                var photoUrl = Path.Combine("RecetaImage", "RecetaPhotos", receta.Nombre, fileName).Replace("\\", "/");
                rec.Imagen = photoUrl;
            }

            //rec.Nombre = receta.Nombre;
            //rec.ListaIngredientes = receta.ListaIngredientes;
            //rec.Preparacion = receta.Preparacion;
            //rec.TiempoPreparacion = receta.TiempoPreparacion;
            //rec.TiempoCoccion = receta.TiempoCoccion;
            //rec.FechaActualizacion = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return null;
            //return new RecetaDto
            //{
            //    Nombre = rec.Nombre,
            //    ListaIngredientes = rec.ListaIngredientes,
            //    Preparacion = rec.Preparacion,
            //    Imagen = rec.Imagen,
            //    TiempoPreparacion = rec.TiempoPreparacion,
            //    TiempoCoccion = rec.TiempoCoccion,
            //    FechaActualizacion = rec.FechaActualizacion
            //};

        }
    }
}
