namespace Messenger.Models.Chats;

public class ChatDto
{
    public Guid ChatId { get; set; }
    public bool IsGroup { get; set; }
    public ICollection<Guid>? Participants { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public MessageDto? LastMessage { get; set; }
}