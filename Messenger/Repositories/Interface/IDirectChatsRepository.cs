using Messenger.Models.Chats;
using Messenger.Models.DirectChat;
using Messenger.Models.Messages;

namespace Messenger.Repositories.Interface;

public interface IDirectChatsRepository
{
    public Task<IEnumerable<MessageDto>> GetChatMessagesAsync(Guid chatId, int skip, int take, CancellationToken ct);
    public Task<Guid> CreateDirectChatAsync(CreateDirectChatRequest createDirectChatRequest,CancellationToken ct);
    public Task<Guid> SendDirectMessageAsync(SendDirectMessageRequest request, CancellationToken ct);
    public Task<IEnumerable<ChatDto>> GetUserDirectChatsAsync(Guid userId, CancellationToken ct);
    public Task EditMessageAsync(EditMessageRequest editMessageRequest, CancellationToken ct);
    public Task DeleteMessageAsync(Guid messageId, Guid senderId, CancellationToken ct);
}