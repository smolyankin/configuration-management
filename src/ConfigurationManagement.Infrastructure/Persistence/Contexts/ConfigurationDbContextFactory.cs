using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConfigurationManagement.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// Для миграций EF Core.
    /// </summary>
    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.Testing.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=configdb_dev;Username=postgres;Password=postgres123;Port=5435";

            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.MigrationsAssembly("ConfigurationManagement.Infrastructure");
            });

            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.EnableDetailedErrors(true);

            return new ConfigurationDbContext(optionsBuilder.Options);
        }
    }
}