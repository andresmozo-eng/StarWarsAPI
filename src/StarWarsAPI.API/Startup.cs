using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarWarsAPI.API.Middlewares;
using StarWarsAPI.API.Validators;
using StarWarsAPI.Application.Helpers;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Application.Services;
using StarWarsAPI.Application.Settings;
using StarWarsAPI.Infrastructure;
using StarWarsAPI.Infrastructure.Repositories;
using System;
using System.IO;
using System.Reflection;

namespace StarWarsAPI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            }); ;

            services.AddDbContext<StarWarsDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            //Inyeccion de configuracion JWT
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

            // Inyección repositorios
            services.AddScoped<IUserRepository, UserRepository>();

            //Inyeccion servicios
            services.AddScoped<IPasswordHasherHelper, PasswordHasherHelper>();
            services.AddScoped<IUserService, UserService>();


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

            // Add Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "StarWars API",
                    Version = "v1",
                    Description = "API de gestión de películas de Star Wars"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Swagger config
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StarWars API v1");
                c.RoutePrefix = string.Empty;
            });



            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
