using Messenger.Models.Chats;
using Messenger.Models.DirectChat;
using Messenger.Models.Messages;
using Messenger.Repositories.Interface;
using Messenger.Validation;

namespace Messenger.Services;

public class DirectChatsService
{
    private readonly ILogger<DirectChatsService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly IDirectChatsRepository _directChatsRepository;

    public DirectChatsService(ILogger<DirectChatsService> logger, IValidationStorage validationStorage, IDirectChatsRepository directChatsRepository)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _directChatsRepository = directChatsRepository;
    }

    #region Actions

    public async Task<IEnumerable<MessageDto>> GetDirectMessages(Guid chatId, int skip, int take, CancellationToken ct)
    {
        // bool isValid = await ValidateGetDirectChat(chatId, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var messageDtos = await _directChatsRepository.GetChatMessagesAsync(chatId, skip, take, ct);
        return messageDtos;
    }
    
    public async Task<IEnumerable<ChatDto>> GetDirectChats(Guid userId, CancellationToken ct)
    {
        // bool isValid = await ValidateGetDirectChats(userId, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var chatDtos = await _directChatsRepository.GetUserDirectChatsAsync(userId, ct);
        return chatDtos;
    }
    
    public async Task<CreateDirectChatResponse> CreateDirectChat(CreateDirectChatRequest chatRequest, CancellationToken ct)
    {
        // bool isValid = await ValidateCreateDirectChat(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var chatId = await _directChatsRepository.CreateDirectChatAsync(chatRequest, ct);
        _logger.LogInformation($"Successfully created Direct Chat {chatId} for users  {chatRequest.CurrentUserId} & {chatRequest.TargetUserId}");
        var directChatDto = new CreateDirectChatResponse()
        {
            ChatId = chatId,
        };
        
        return directChatDto;
    }
    
    public async Task<Guid> SendToDirectMessage(SendDirectMessageRequest request, CancellationToken ct)
    {
        // bool isValid = await ValidateSendToGroupMessage(request, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var messageId = await _directChatsRepository.SendDirectMessageAsync(request , ct);
        _logger.LogInformation($"Successfully send user {request.SenderId} a message to chatId {request.ChatId}");
        return messageId;
    }
    
    public async Task<bool> EditMessageChat(EditMessageRequest editMessageRequest, CancellationToken ct)
    {
        // bool isValid = await ValidateEditMessageChat(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        if (editMessageRequest == null)
            throw new Exception("Message not found");
        
        await _directChatsRepository.EditMessageAsync(editMessageRequest, ct);
        _logger.LogInformation($"Successfully edited Direct Chat {editMessageRequest.MessageId} for user {editMessageRequest.SenderId} and text {editMessageRequest.NewText}");
        return true;
    }

    public async Task<bool> DeleteMessageChat(Guid messageId, Guid senderId, CancellationToken ct)
    {
        // bool isValid = await ValidateEditMessageChat(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        await _directChatsRepository.DeleteMessageAsync(messageId, senderId, ct);
        _logger.LogInformation($"Successfully deleted Direct Chat {messageId} for user {senderId}");
        return true;
    }
    
    #endregion
}