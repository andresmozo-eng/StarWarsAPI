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
            // Construye la configuración para obtener la cadena de conexión
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") 
                .Build();

            var builder = new DbContextOptionsBuilder<StarWarsDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // O el nombre de tu ConnectionString

            builder.UseNpgsql(connectionString); 

            return new StarWarsDbContext(builder.Options);
        }
    }
}
