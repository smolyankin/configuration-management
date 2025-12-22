using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConfigurationManagement.Domain.Entities;

namespace ConfigurationManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация пользователя.
/// </summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasDefaultValueSql("uuidv7()");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .IsUnicode();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode();

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired();

        builder.HasMany(u => u.Configurations)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<NotificationSubscriptionEntity>()
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.CreatedAt);

        builder.Property(u => u.UpdatedAt)
            .IsConcurrencyToken();
    }
}