using Messenger.Models;
using Messenger.Models.Chats;
using Messenger.Models.db;
using Messenger.Models.DirectChat;
using Messenger.Models.Messages;
using Messenger.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Repositories;

public class DirectChatsRepository : IDirectChatsRepository
{
    private readonly MessengerDbContext _dbContext;

    public DirectChatsRepository(MessengerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MessageDto>> GetChatMessagesAsync(Guid chatId, int skip, int take, CancellationToken ct)
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

    public async Task<Guid> CreateDirectChatAsync(CreateDirectChatRequest createDirectChatRequest, CancellationToken ct)
    {
        var existingChat = await _dbContext.Chats
            .Where(c => !c.IsGroup)
            .Where(c => c.ChatParticipants.Any(cp => cp.UserId == createDirectChatRequest.CurrentUserId) &&
                        c.ChatParticipants.Any(cp => cp.UserId == createDirectChatRequest.TargetUserId))
            .FirstOrDefaultAsync(ct);

        if (existingChat != null)
            return existingChat.ChatId;

        var chat = new Chat
        {
            ChatId = Guid.NewGuid(),
            IsGroup = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _dbContext.Chats.Add(chat);

        _dbContext.ChatParticipants.AddRange(
            new ChatParticipant
            {
                ChatParticipantId = Guid.NewGuid(),
                ChatId = chat.ChatId,
                UserId = createDirectChatRequest.CurrentUserId,
                JoinedAt = DateTimeOffset.UtcNow
            },
            new ChatParticipant
            {
                ChatParticipantId = Guid.NewGuid(),
                ChatId = chat.ChatId,
                UserId = createDirectChatRequest.TargetUserId,
                JoinedAt = DateTimeOffset.UtcNow
            }
        );

        await _dbContext.SaveChangesAsync(ct);
        return chat.ChatId;
    }

    public async Task<Guid> SendDirectMessageAsync(SendDirectMessageRequest request, CancellationToken ct)
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

    public async Task<IEnumerable<ChatDto>> GetUserDirectChatsAsync(Guid userId, CancellationToken ct)
    {
        var chats = await _dbContext.Chats
            .Where(c => !c.IsGroup && c.ChatParticipants.Any(cp => cp.UserId == userId))
            .Include(c => c.ChatParticipants)
            .ToListAsync(ct);

        return chats.Select(c => new ChatDto
        {
            ChatId = c.ChatId,
            Participants = c.ChatParticipants.Select(cp => cp.UserId).ToList(),
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task EditMessageAsync(EditMessageRequest editMessageRequest, CancellationToken ct)
    {
        await _dbContext.Messages
            .Where(m => m.MessageId == editMessageRequest.MessageId && m.SenderId == editMessageRequest.SenderId)
            .ExecuteUpdateAsync(message => message
                .SetProperty(m => m.Text, _ => editMessageRequest.NewText)
                .SetProperty(m => m.EditedAt, _ => DateTimeOffset.UtcNow), ct);
    }

    public async Task DeleteMessageAsync(Guid messageId, Guid senderId, CancellationToken ct)
    {
        await _dbContext.Messages
            .Where(m => m.MessageId == messageId && m.SenderId == senderId)
            .ExecuteUpdateAsync(message => message
                .SetProperty(m => m.IsDeleted , _ => true)
                .SetProperty(m => m.Text, _ => "This message was deleted.")
                .SetProperty(m => m.EditedAt, _ => DateTimeOffset.UtcNow), ct);
    }
}