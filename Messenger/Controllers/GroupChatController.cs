using Messenger.Models.DirectChat;
using Messenger.Models.Groups;
using Messenger.Models.Messages;
using Messenger.Services;
using Messenger.Validation;
using Messenger.Validation.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Controllers;

[ApiController]
[Route("api/public")]
public class GroupChatController : BaseController
{
    private readonly GroupChatsService _groupChatsService;

    protected GroupChatController(GroupChatsService groupChatsService, IValidationStorage validationStorage) 
        : base(validationStorage)
    {
        _groupChatsService = groupChatsService;
    }
    
    [HttpGet("chat/group/messages")]
    public async Task<IActionResult> GetGroupMessages([FromQuery, ValidGuid] Guid chatId, [FromQuery, ValidCount] int skip, [FromQuery, ValidCount] int take, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.GetGroupMessages(chatId, skip, take, token), ct);
    }
    
    [HttpPost("chats/grouo")]
    public async Task<IActionResult> AddParticipantToGroup([FromQuery, ValidGuid] Guid chatId, [FromQuery, ValidGuid] Guid userId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.AddParticipantToGroup(chatId, userId, token), ct);
    }
    
    [HttpPost("chat/group")]
    public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatRequest createGroupChatRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.CreateGroupChat(createGroupChatRequest, token), ct);
    }
    
    [HttpPost("chat/group/message")]
    public async Task<IActionResult> CreateDirectMessage([FromBody] SendGroupMessageRequest request, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.SendToGroupMessage(request, token), ct);
    }
    
    [HttpPut("chat/group/message")]
    public async Task<IActionResult> EditMessageGroup([FromBody] EditMessageRequest editMessageRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.EditMessageChat(editMessageRequest, token), ct);
    }
    
    [HttpDelete("chat/group/message")]
    public async Task<IActionResult> DeleteMessageGroup([FromQuery, ValidGuid] Guid messageId, [FromQuery, ValidGuid] Guid senderId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _groupChatsService.DeleteMessageChat(messageId, senderId, token), ct);
    }
}