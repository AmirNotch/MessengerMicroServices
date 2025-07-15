using Messenger.Models.Chats;
using Messenger.Models.Groups;
using Messenger.Models.Messages;

namespace Messenger.Repositories.Interface;

public interface IGroupChatsRepository
{
    // Создание группы
    public Task<Guid> CreateGroupChatAsync(CreateGroupChatRequest request, CancellationToken ct);
    
    // Добавление пользователя в группу
    public Task AddParticipantToGroupAsync(Guid chatId, Guid userId, CancellationToken ct);
    
    // Отправка сообщения в группу
    public Task<Guid> SendGroupMessageAsync(SendGroupMessageRequest request, CancellationToken ct);
    
    // Получение всех сообщений группы
    public Task<IEnumerable<MessageDto>> GetGroupMessagesAsync(Guid chatId, int skip, int take, CancellationToken ct);
    
    // Редактирование сообщения в группе
    public Task EditGroupMessageAsync(EditMessageRequest request, CancellationToken ct);
    
    // Удаление сообщения в группе
    public Task DeleteGroupMessageAsync(Guid messageId, Guid senderId, CancellationToken ct);
}