using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConfigurationManagement.Domain.Entities;

namespace ConfigurationManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация версии конфигурации.
/// </summary>
public class ConfigurationVersionEntityConfiguration : IEntityTypeConfiguration<ConfigurationVersionEntity>
{
    public void Configure(EntityTypeBuilder<ConfigurationVersionEntity> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasDefaultValueSql("uuidv7()");

        builder.Property(v => v.ConfigurationId)
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode();

        builder.Property(v => v.Data)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired();

        builder.HasOne(v => v.Configuration)
            .WithMany(c => c.Versions)
            .HasForeignKey(v => v.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.ConfigurationId, c.VersionNumber });
        
        builder.Property(v => v.UpdatedAt)
            .IsConcurrencyToken();
    }
}