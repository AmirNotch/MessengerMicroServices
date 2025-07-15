using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Models.DbConfigurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> entity)
    {
        entity.ToTable("chats");
        
        entity.HasKey(e => e.ChatId)
            .HasName("chats_pkey");
        
        entity.Property(e => e.ChatName)
            .HasColumnName("chat_name")
            .IsRequired();
        
        entity.Property(e => e.IsGroup)
            .HasColumnName("is_group")
            .IsRequired();
        
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created")
            .IsRequired();
        
        // Chat -> ChatParticipants
        // Cascade Can be because chats can be deleted
        entity.HasMany(c => c.ChatParticipants)
            .WithOne(cp => cp.Chat)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chat -> Messages
        // Cascade Can be because chats can be deleted
        entity.HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}