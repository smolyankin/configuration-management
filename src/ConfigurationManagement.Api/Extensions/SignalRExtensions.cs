using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace ConfigurationManagement.Api.Extensions;

/// <summary>
/// Расширения для SignalR.
/// </summary>
public static class SignalRExtensions
{
    /// <summary>
    /// Добавить SignalR.
    /// </summary>
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment = false)
    {
        var signalRBuilder = services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.StreamBufferCapacity = 10;
        })
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
            options.PayloadSerializerOptions.WriteIndented = isDevelopment;
        });

        if (!isDevelopment && configuration.GetConnectionString("Redis") != null)
        {
            try
            {
                var redisBackplaneType = Type.GetType("Microsoft.AspNetCore.SignalR.StackExchangeRedis.RedisHubLifetimeManagerExtensions, Microsoft.AspNetCore.SignalR.StackExchangeRedis");
                if (redisBackplaneType != null)
                {
                    var addRedisMethod = redisBackplaneType.GetMethod("AddStackExchangeRedis", new[] { typeof(ISignalRBuilder), typeof(string) });
                    if (addRedisMethod != null)
                    {
                        addRedisMethod.Invoke(null, new object[] { signalRBuilder, configuration.GetConnectionString("Redis")! });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis backplane not available: {ex.Message}");
            }
        }

        return services;
    }
}