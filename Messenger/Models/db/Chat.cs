namespace Messenger.Models.db;

public class Chat
{
    public Guid ChatId { get; set; }
    public string ChatName { get; set; } = null!;
    public bool IsGroup { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}