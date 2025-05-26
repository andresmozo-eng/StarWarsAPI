using Microsoft.EntityFrameworkCore;
using StarWarsAPI.Domain.Entities;
using StarWarsAPI.Infrastructure.Configurations;
using System;

namespace StarWarsAPI.Infrastructure
{
    public class StarWarsDbContext : DbContext
    {
        public StarWarsDbContext(DbContextOptions<StarWarsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StarWarsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Description = "User" },
                new Role { Id = 2, Description = "Admin" }
            );
        }

    }
}
