using Microsoft.EntityFrameworkCore;
using TechnicianAgenda.Models;

namespace TechnicianAgenda.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobSubcategory> JobSubcategories { get; set; }
        public DbSet<WorkFile> WorkFiles { get; set; }
        public DbSet<Technician> Technicians { get; set; } // NUEVO
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones existentes
            modelBuilder.Entity<Direction>()
                .HasOne(d => d.Client)
                .WithMany(c => c.Directions)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Work>()
                .HasOne(w => w.Client)
                .WithMany(c => c.Works)
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Work>()
                .HasOne(w => w.Direction)
                .WithMany()
                .HasForeignKey(w => w.DirectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobSubcategory>()
                .HasOne(js => js.JobCategory)
                .WithMany(jc => jc.Subcategories)
                .HasForeignKey(js => js.JobCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Work>()
                .HasOne(w => w.JobCategory)
                .WithMany()
                .HasForeignKey(w => w.JobCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Work>()
                .HasOne(w => w.JobSubcategory)
                .WithMany()
                .HasForeignKey(w => w.JobSubcategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // NUEVA RELACIÓN - WorkFiles
            modelBuilder.Entity<WorkFile>()
                .HasOne(wf => wf.Work)
                .WithMany(w => w.Files)
                .HasForeignKey(wf => wf.WorkId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el Work, se borran los archivos

            // NUEVA RELACIÓN - Technician (nullable)
            modelBuilder.Entity<Work>()
                .HasOne(w => w.Technician)
                .WithMany(t => t.AssignedWorks)
                .HasForeignKey(w => w.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull) // Si se borra el técnico, el trabajo queda sin técnico asignado
                .IsRequired(false); // El técnico es opcional

            // Configurar decimales para costos
            modelBuilder.Entity<Work>()
                .Property(w => w.Costos)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Work>()
                .Property(w => w.TotalACobrar)
                .HasPrecision(18, 2);

            // NUEVO - Configurar decimales para pago a técnico
            modelBuilder.Entity<Work>()
                .Property(w => w.PorPagarATecnico)
                .HasPrecision(18, 2);

            // Índices para optimización
            modelBuilder.Entity<Technician>()
                .HasIndex(t => t.RutOPasaporte)
                .IsUnique(); // RUT o Pasaporte debe ser único

            modelBuilder.Entity<Technician>()
                .HasIndex(t => t.CorreoElectronico);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.CorreoElectronico);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Work>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Technician>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);



        }
    }
}