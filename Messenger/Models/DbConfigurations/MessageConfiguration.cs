using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Models.DbConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> entity)
    {
        entity.ToTable("messages");

        entity.HasKey(e => e.MessageId)
            .HasName("messages_pkey");

        entity.Property(e => e.MessageId)
            .HasColumnName("message_id")
            .IsRequired();

        entity.Property(e => e.ChatId)
            .HasColumnName("chat_id")
            .IsRequired();

        entity.Property(e => e.SenderId)
            .HasColumnName("sender_id")
            .IsRequired();

        entity.Property(e => e.Text)
            .HasColumnName("text")
            .IsRequired();

        entity.Property(e => e.SentAt)
            .HasColumnName("sent_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.EditedAt)
            .HasColumnName("edited_at");

        entity.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        // Message -> Sender (User)
        entity.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message -> Chat
        entity.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message -> MessageStatuses
        entity.HasMany(m => m.MessageStatuses)
            .WithOne(ms => ms.Message)
            .HasForeignKey(ms => ms.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
