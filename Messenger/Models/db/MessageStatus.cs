namespace Messenger.Models.db;

public class MessageStatus
{
    public Guid MessageStatusId { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    public virtual Message Message { get; set; }
    public virtual User User { get; set; }
}