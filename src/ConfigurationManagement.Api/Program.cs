using ConfigurationManagement.Api.Extensions;
using ConfigurationManagement.Application.Extensions;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Infrastructure.Extensions;
using ConfigurationManagement.Infrastructure.Hubs;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using ConfigurationManagement.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ConfigurationManagement.Api")
    .CreateLogger();

try
{
    Log.Information("Starting Configuration Management API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    
    builder.Services.AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();
    
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();

    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Connection string: {connectionString}");

        builder.Services.AddDbContext(options =>
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            }
            else
            {
                Console.WriteLine("Warning: No connection string found, using in-memory database");
                options.UseInMemoryDatabase("InMemoryDb")
                    .UseSnakeCaseNamingConvention();
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding infrastructure services: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }

    builder.Services.AddApplicationServices();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = new[] { "application/json", "text/plain", "text/css", "application/javascript" };
    });

    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();

    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ITokenService, JwtTokenService>();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<INotifyService, NotifyService>();

    builder.Services.AddHealthChecks();

    builder.Services.AddSignalRServices(builder.Configuration, builder.Environment.IsDevelopment());

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    app.UseCors("AllowAll");

    await ApplyDatabaseMigrationsAsync(app.Services);

    app.UseResponseCompression();

    app.UseSimpleErrorHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapOpenApi();
    }

    var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInDocker)
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<NotificationsHub>("/hubs/notifications");

    app.UseHealthChecks("/health");

    app.MapGet("/", () => Results.Ok(new
    {
        Name = "Configuration Management API",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        HealthCheck = "/health",
        Swagger = "/swagger"
    }))
    .WithName("GetInfo");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
{
    try
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        if (dbContext.Database.IsInMemory())
        {
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Using in-memory database");
        }
        else
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrations completed successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error setting up database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");

        try
        {
            Console.WriteLine("Attempting fallback database creation...");
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Fallback database creation completed successfully");
        }
        catch (Exception fallbackEx)
        {
            Console.WriteLine($"Fallback database creation also failed: {fallbackEx.Message}");
            throw;
        }
    }
}