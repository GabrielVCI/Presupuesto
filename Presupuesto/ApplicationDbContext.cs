using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presupuesto.Entidades;

namespace Presupuesto
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions options)
      : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Trabajo>().Property(t => t.Titulo).HasMaxLength(50).IsRequired();
            //This is Fluent API, we can edit the properties of the tables using this format.

        }

        public DbSet<Personas> Trabajo { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
        public DbSet<Aportes> Aportes { get; set; }
        public DbSet<Objetivos> Objetivos { get; set; }
      

    }
}
