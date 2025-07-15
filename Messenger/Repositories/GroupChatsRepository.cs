using Messenger.Models;
using Messenger.Models.Chats;
using Messenger.Models.db;
using Messenger.Models.Groups;
using Messenger.Models.Messages;
using Messenger.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories;

public class GroupChatsRepository : IGroupChatsRepository
{
    private readonly MessengerDbContext _dbContext;

    public GroupChatsRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateGroupChatAsync(CreateGroupChatRequest request, CancellationToken ct)
    {
        var chat = new Chat
        {
            ChatId = Guid.NewGuid(),
            ChatName = request.ChatName,
            IsGroup = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Chats.Add(chat);

        var participants = request.ParticipantUserIds
            .Append(request.CreatorUserId)
            .Distinct()
            .Select(userId => new ChatParticipant
            {
                ChatParticipantId = Guid.NewGuid(),
                ChatId = chat.ChatId,
                UserId = userId,
                JoinedAt = DateTimeOffset.UtcNow
            });

        _dbContext.ChatParticipants.AddRange(participants);

        await _dbContext.SaveChangesAsync(ct);

        return chat.ChatId;
    }

    public async Task AddParticipantToGroupAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        var exists = await _dbContext.ChatParticipants
            .AnyAsync(cp => cp.ChatId == chatId && cp.UserId == userId, ct);

        if (!exists)
        {
            var participant = new ChatParticipant
            {
                ChatParticipantId = Guid.NewGuid(),
                ChatId = chatId,
                UserId = userId,
                JoinedAt = DateTimeOffset.UtcNow
            };

            _dbContext.ChatParticipants.Add(participant);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<Guid> SendGroupMessageAsync(SendGroupMessageRequest request, CancellationToken ct)
    {
        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Text = request.Text,
            SentAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync(ct);

        return message.MessageId;
    }

    public async Task<IEnumerable<MessageDto>> GetGroupMessagesAsync(Guid chatId, int skip, int take, CancellationToken ct)
    {
        var messages = await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Text = m.IsDeleted ? "This message was deleted." : m.Text,
                SentAt = m.SentAt,
                EditedAt = m.EditedAt,
                IsDeleted = m.IsDeleted
            })
            .ToListAsync(ct);

        return messages;
    }

    public async Task EditGroupMessageAsync(EditMessageRequest request, CancellationToken ct)
    {
        await _dbContext.Messages
            .Where(m => m.MessageId == request.MessageId && m.SenderId == request.SenderId)
            .ExecuteUpdateAsync(message => message
                .SetProperty(m => m.Text, _ => request.NewText)
                .SetProperty(m => m.EditedAt, _ => DateTimeOffset.UtcNow), ct);
    }

    public async Task DeleteGroupMessageAsync(Guid messageId, Guid senderId, CancellationToken ct)
    {
        await _dbContext.Messages
            .Where(m => m.MessageId == messageId && m.SenderId == senderId)
            .ExecuteUpdateAsync(message => message
                .SetProperty(m => m.IsDeleted, _ => true)
                .SetProperty(m => m.EditedAt, _ => DateTimeOffset.UtcNow), ct);
    }
}