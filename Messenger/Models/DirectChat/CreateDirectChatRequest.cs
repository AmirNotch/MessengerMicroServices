using Messenger.Validation.Attributes;

namespace Messenger.Models.DirectChat;

public class CreateDirectChatRequest
{
    [ValidGuid]
    public Guid CurrentUserId { get; set; }
    [ValidGuid]
    public Guid TargetUserId { get; set; }
}