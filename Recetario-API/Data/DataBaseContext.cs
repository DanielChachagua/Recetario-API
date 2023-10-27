using Microsoft.EntityFrameworkCore;
using Recetario_API.Models;
using Recetario_API.Models.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Recetario_API.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<PasosPreparacion> PasosPreparaciones { get; set; }

        public DbSet<RecetaFavorita> RecetasFavoritas { get; set; }
        public DbSet<IngredienteReceta> IngredientesRecetas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RecetaFavorita>()
            //    .HasOne(rf => rf.PertenecienteAReceta)
            //    .WithMany()
            //    .HasForeignKey(rf => rf.RecetaId)
            //    .OnDelete(DeleteBehavior.NoAction); // Restringir la eliminación en lugar de eliminar en cascada

            //modelBuilder.Entity<RecetaFavorita>()
            //    .HasOne(rf => rf.UsuarioCreador)
            //    .WithMany()
            //    .HasForeignKey(rf => rf.UserID)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Receta>()
            .HasMany(r => r.Ingredientes)
            .WithOne(i => i.Receta)
            .HasForeignKey(i => i.RecetaId);

            modelBuilder.Entity<Receta>()
                .HasMany(r => r.Preparaciones)
                .WithOne(p => p.Receta)
                .HasForeignKey(p => p.RecetaId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(); 
            base.OnConfiguring(optionsBuilder);
        }
    }

}
