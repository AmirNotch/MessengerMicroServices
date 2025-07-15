namespace Messenger.Models.DirectChat;

public class SendDirectMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = null!;
}