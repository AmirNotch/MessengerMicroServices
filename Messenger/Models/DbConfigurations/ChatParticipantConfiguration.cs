using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Models.DbConfigurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> entity)
    {
        entity.ToTable("chat_participants");

        entity.HasKey(e => e.ChatParticipantId)
            .HasName("chat_participants_pkey");

        entity.Property(e => e.ChatParticipantId)
            .HasColumnName("chat_participant_id")
            .IsRequired();

        entity.Property(e => e.ChatId)
            .HasColumnName("chat_id")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        entity.Property(e => e.JoinedAt)
            .HasColumnName("joined_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        // Навигационные свойства и связи
        entity.HasOne(e => e.Chat)
            .WithMany(a => a.ChatParticipants) // Изменено на AuctionLots для связи
            .HasForeignKey(e => e.ChatId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Навигационные свойства и связи
        entity.HasOne(e => e.User)
            .WithMany(a => a.ChatParticipants) // Изменено на AuctionLots для связи
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
