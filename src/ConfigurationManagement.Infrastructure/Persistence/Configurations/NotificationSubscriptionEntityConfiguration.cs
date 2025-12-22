using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ConfigurationManagement.Domain.Entities;

namespace ConfigurationManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация подписки событий конфигурациию.
/// </summary>
public class NotificationSubscriptionEntityConfiguration : IEntityTypeConfiguration<NotificationSubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<NotificationSubscriptionEntity> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasDefaultValueSql("uuidv7()");

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.UserId);

        builder.HasIndex(s => s.ConfigurationEventTypes);

        builder.Property(s => s.UpdatedAt)
            .IsConcurrencyToken();
    }
}