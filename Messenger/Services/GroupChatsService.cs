using Messenger.Models.Chats;
using Messenger.Models.DirectChat;
using Messenger.Models.Groups;
using Messenger.Models.Messages;
using Messenger.Repositories.Interface;
using Messenger.Validation;

namespace Messenger.Services;

public class GroupChatsService
{
    private readonly ILogger<GroupChatsService> _logger;
    private readonly IValidationStorage _validationStorage;
    private readonly IGroupChatsRepository _groupChatsRepository;

    public GroupChatsService(ILogger<GroupChatsService> logger, IValidationStorage validationStorage, IGroupChatsRepository groupChatsRepository)
    {
        _logger = logger;
        _validationStorage = validationStorage;
        _groupChatsRepository = groupChatsRepository;
    }

    #region Action

    public async Task<IEnumerable<MessageDto>> GetGroupMessages(Guid chatId, int skip, int take, CancellationToken ct)
    {
        // bool isValid = await ValidateGetGroupMessages(userId, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var messageDtos = await _groupChatsRepository.GetGroupMessagesAsync(chatId, skip, take, ct);
        return messageDtos;
    }

    public async Task<CreateDirectChatResponse> CreateGroupChat(CreateGroupChatRequest createGroupChatRequest, CancellationToken ct)
    {
        // bool isValid = await ValidateCreateGroupChat(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var chatId = await _groupChatsRepository.CreateGroupChatAsync(createGroupChatRequest, ct);
        _logger.LogInformation($"Successfully created Chat {chatId} with name {createGroupChatRequest.ChatName} & {createGroupChatRequest.CreatorUserId}");
        var directChatDto = new CreateDirectChatResponse()
        {
            ChatId = chatId,
        };
        
        return directChatDto;
    }
    
    public async Task<bool> AddParticipantToGroup(Guid chatId, Guid userId, CancellationToken ct)
    {
        // bool isValid = await ValidateAddParticipantToGroup(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        await _groupChatsRepository.AddParticipantToGroupAsync(chatId, userId, ct);
        _logger.LogInformation($"Successfully added user {userId} with chatId {chatId}");
        return true;
    }
    
    public async Task<Guid> SendToGroupMessage(SendGroupMessageRequest request, CancellationToken ct)
    {
        // bool isValid = await ValidateSendToGroupMessage(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        var messageId = await _groupChatsRepository.SendGroupMessageAsync(request , ct);
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
        
        await _groupChatsRepository.EditGroupMessageAsync(editMessageRequest, ct);
        _logger.LogInformation($"Successfully edited message in Chat {editMessageRequest.MessageId} by user {editMessageRequest.SenderId} and text {editMessageRequest.NewText}");
        return true;
    }

    public async Task<bool> DeleteMessageChat(Guid messageId, Guid senderId, CancellationToken ct)
    {
        // bool isValid = await ValidateDeleteMessageChat(chatRequest, ct);
        // if (!isValid)
        // {
        //     return null!;
        // }
        
        await _groupChatsRepository.DeleteGroupMessageAsync(messageId, senderId, ct);
        _logger.LogInformation($"Successfully deleted Chat {messageId}");
        return true;
    }

    #endregion
}