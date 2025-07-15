namespace Messenger.Models.db;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTimeOffset LastLoginAt { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    // public bool IsOnline { get; set; } = false;
    
    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    public virtual ICollection<MessageStatus> MessageStatuses { get; set; }
}