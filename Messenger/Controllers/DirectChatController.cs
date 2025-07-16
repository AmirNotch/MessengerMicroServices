using Messenger.Models.DirectChat;
using Messenger.Models.Messages;
using Messenger.Services;
using Messenger.Validation;
using Messenger.Validation.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Controllers;

[ApiController]
[Route("api/public")]
public class DirectChatController : BaseController
{
    private readonly DirectChatsService _directChatsService;

    protected DirectChatController(DirectChatsService directChatsService, IValidationStorage validationStorage) 
        : base(validationStorage)
    {
        _directChatsService = directChatsService;
    }
    
    [HttpGet("chat/direct/messages")]
    public async Task<IActionResult> GetDirectMessages([FromQuery, ValidGuid] Guid chatId, [FromQuery, ValidCount] int skip, [FromQuery, ValidCount] int take, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.GetDirectMessages(chatId, skip, take, token), ct);
    }
    
    [HttpGet("chats/direct")]
    public async Task<IActionResult> GetDirectChats([FromQuery, ValidGuid] Guid userId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.GetDirectChats(userId, token), ct);
    }
    
    [HttpPost("chat/direct")]
    public async Task<IActionResult> CreateDirectChat([FromBody] CreateDirectChatRequest chatRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.CreateDirectChat(chatRequest, token), ct);
    }
    
    [HttpPost("chat/direct/message")]
    public async Task<IActionResult> CreateDirectMessage([FromBody] SendDirectMessageRequest directMessageRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.SendToDirectMessage(directMessageRequest, token), ct);
    }
    
    [HttpPut("chat/direct/message")]
    public async Task<IActionResult> EditMessage([FromBody] EditMessageRequest editMessageRequest, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.EditMessageChat(editMessageRequest, token), ct);
    }
    
    [HttpDelete("chat/direct/message")]
    public async Task<IActionResult> DeleteMessage([FromQuery, ValidGuid] Guid messageId, [FromQuery, ValidGuid] Guid senderId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _directChatsService.DeleteMessageChat(messageId, senderId, token), ct);
    }
}