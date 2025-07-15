namespace Messenger.Models.Groups;

public class CreateGroupChatRequest
{
    public string ChatName { get; set; } = null!;
    public Guid CreatorUserId { get; set; }
    public List<Guid> ParticipantUserIds { get; set; } = new();
}