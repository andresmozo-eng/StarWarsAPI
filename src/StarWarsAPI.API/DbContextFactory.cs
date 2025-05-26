using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarWarsAPI.Infrastructure;
using System.IO;

namespace StarWarsAPI.API
{
    public class DbContextFactory : IDesignTimeDbContextFactory<StarWarsDbContext>
    {
        public StarWarsDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") 
                .Build();

            var builder = new DbContextOptionsBuilder<StarWarsDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString); 

            return new StarWarsDbContext(builder.Options);
        }
    }
}
