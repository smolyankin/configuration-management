using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация базы данных.
    /// </summary>
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? configureDbContext)
    {
        if (configureDbContext != null)
        {
            services.AddDbContext<ConfigurationDbContext>(configureDbContext);
        }
        else
        {
            services.AddDbContext<ConfigurationDbContext>();
        }

        return services;
    }
}