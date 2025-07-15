namespace Messenger.Models.db;

public class Message
{
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = null!;
    public DateTimeOffset SentAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Навигационные свойства
    public virtual Chat Chat { get; set; }
    public virtual User Sender { get; set; }
    public virtual ICollection<MessageStatus> MessageStatuses { get; set; }
}