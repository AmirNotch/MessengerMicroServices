namespace Messenger.Models.Messages;

public class EditMessageRequest
{
    public Guid MessageId { get; set; }
    public Guid SenderId { get; set; }
    public string NewText { get; set; } = null!;
}