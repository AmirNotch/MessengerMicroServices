namespace NotificationService.Models.UnreadMessages;

public class UnreadMessageDto
{
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public string Text { get; set; } = null!;
    public DateTimeOffset SentAt { get; set; }
}
