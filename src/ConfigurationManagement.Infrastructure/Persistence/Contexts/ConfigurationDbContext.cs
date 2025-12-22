using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Infrastructure.Persistence.Contexts;

/// <summary>
/// База данных.
/// </summary>
public class ConfigurationDbContext : DbContext
{
    public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ConfigurationEntity> Configurations { get; set; }
    public DbSet<ConfigurationVersionEntity> ConfigurationVersions { get; set; }
    public DbSet<NotificationSubscriptionEntity> NotificationSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigurationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigurationVersionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationSubscriptionEntityConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(e => e.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Property(e => e.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}