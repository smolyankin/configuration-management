using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConfigurationManagement.Domain.Entities;

namespace ConfigurationManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация конфигурации пользователя.
/// </summary>
public class ConfigurationEntityConfiguration : IEntityTypeConfiguration<ConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<ConfigurationEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasDefaultValueSql("uuidv7()");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.Property(c => c.Data)
            .HasColumnType("jsonb");

        builder.HasOne(c => c.User)
            .WithMany(u => u.Configurations)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Versions)
            .WithOne(v => v.Configuration)
            .HasForeignKey(v => v.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.UserId, c.Name })
            .IsUnique()
            .IncludeProperties(c => c.CreatedAt);

        builder.HasIndex(c => new { c.UserId, c.CreatedAt });

        builder.Property(c => c.UpdatedAt)
            .IsConcurrencyToken();
    }
}