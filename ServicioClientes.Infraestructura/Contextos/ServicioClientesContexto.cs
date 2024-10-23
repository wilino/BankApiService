using Microsoft.EntityFrameworkCore;
using ServicioClientes.Dominio.Entidades;
using ServicioCuentas.Dominio.Entidades;

namespace ServicioClientes.Infraestructura.Contextos
{
    public class ServicioClientesContexto : DbContext
    {
        public ServicioClientesContexto(DbContextOptions<ServicioClientesContexto> options)
            : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Persona>()
                .HasKey(p => p.Id); 

            modelBuilder.Entity<Persona>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.ClienteId);  

            modelBuilder.Entity<Cliente>()
                .Property(c => c.ClienteId)
                .ValueGeneratedOnAdd();

            
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Persona)
                .WithOne()  
                .HasForeignKey<Cliente>(c => c.PersonaId);

            modelBuilder.Entity<Movimiento>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Movimiento>()
                .HasOne<Cuenta>()
                .WithMany()
                .HasForeignKey(m => m.NumeroCuenta);

            modelBuilder.Entity<Cuenta>()
                .HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Cuenta>()
               .HasKey(c=>c.Id);

            modelBuilder.Entity<Cuenta>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
        }

    }
}

