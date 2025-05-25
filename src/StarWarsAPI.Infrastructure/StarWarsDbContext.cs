using Microsoft.EntityFrameworkCore;
using StarWarsAPI.Domain.Entities;
using StarWarsAPI.Infrastructure.Configurations;

namespace StarWarsAPI.Infrastructure
{
    public class StarWarsDbContext : DbContext
    {
        public StarWarsDbContext(DbContextOptions<StarWarsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Description = "User" },
                new Role { Id = 2, Description = "Admin" }
            );
        }

    }
}
