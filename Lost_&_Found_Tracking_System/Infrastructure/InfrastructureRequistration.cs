using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Infrastructure.DBContext;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class InfrastructureRequistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddDbContext<ApplicationDBContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(120);
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null);
                });
                option.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            }
            );

            services.AddMemoryCache();

            return services;
        }

        public static async Task InfrastructureConfigMiddleware(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect && env.IsDevelopment())
                {
                    await context.Database.EnsureCreatedAsync();
                }
            }
            catch
            {
                if (env.IsDevelopment())
                {
                    await context.Database.EnsureCreatedAsync();
                }
            }
        }

    }
}
