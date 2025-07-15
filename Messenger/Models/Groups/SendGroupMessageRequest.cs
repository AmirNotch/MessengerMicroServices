namespace Messenger.Models.Groups;

public class SendGroupMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = null!;
}