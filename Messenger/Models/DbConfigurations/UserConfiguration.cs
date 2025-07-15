using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Models.DbConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");
        
        entity.HasKey(e => e.UserId)
            .HasName("users_pkey");
        
        entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        entity.Property(e => e.UserName)
            .HasColumnName("user_name")
            .IsRequired();
        
        entity.Property(e => e.Email)
            .HasColumnName("email")
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");
        
        entity.Property(e => e.LastLoginAt)
            .HasColumnName("last_login_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        // Навигации - User имеет много ChatParticipants
        entity.HasMany(u => u.ChatParticipants)
            .WithOne(cp => cp.User)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // entity.Property(e => e.IsOnline)
        //     .HasColumnName("is_online")
        //     .IsRequired();
    }
}