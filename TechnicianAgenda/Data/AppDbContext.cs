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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Direction -> Client relationship
            modelBuilder.Entity<Direction>()
                .HasOne(d => d.Client)
                .WithMany(c => c.Directions)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict); // Changed this

            // Work -> Client relationship
            modelBuilder.Entity<Work>()
                .HasOne(w => w.Client)
                .WithMany()
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.Restrict); // Changed this

            // Work -> Direction relationship
            modelBuilder.Entity<Work>()
                .HasOne(w => w.Direction)
                .WithMany()
                .HasForeignKey(w => w.DirectionId)
                .OnDelete(DeleteBehavior.Restrict); // Changed this
        }
    }
}