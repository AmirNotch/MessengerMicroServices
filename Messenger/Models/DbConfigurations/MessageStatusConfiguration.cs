using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Models.DbConfigurations;

public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
{
    public void Configure(EntityTypeBuilder<MessageStatus> entity)
    {
        entity.ToTable("message_statuses");

        entity.HasKey(e => e.MessageStatusId)
            .HasName("message_statuses_pkey");

        entity.Property(e => e.MessageStatusId)
            .HasColumnName("message_status_id")
            .IsRequired();

        entity.Property(e => e.MessageId)
            .HasColumnName("message_id")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        entity.Property(e => e.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false);

        entity.Property(e => e.ReadAt)
            .HasColumnName("read_at");

        // MessageStatus -> Message
        entity.HasOne(ms => ms.Message)
            .WithMany(m => m.MessageStatuses)
            .HasForeignKey(ms => ms.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // MessageStatus -> User
        entity.HasOne(ms => ms.User)
            .WithMany(m => m.MessageStatuses)
            .HasForeignKey(ms => ms.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
