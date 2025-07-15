namespace Messenger.Models.db;

public class ChatParticipant
{
    public Guid ChatParticipantId { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }

    public virtual Chat Chat { get; set; }
    public virtual User User { get; set; }
}