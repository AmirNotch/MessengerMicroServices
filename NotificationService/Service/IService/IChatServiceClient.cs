using NotificationService.Models.UnreadMessages;

namespace NotificationService.Service.IService;

public interface IChatServiceClient
{
    Task<List<UnreadMessageDto>> GetUnreadMessagesAsync(int olderThanMinutes, CancellationToken ct);
    Task MarkMessagesNotifiedAsync(IEnumerable<Guid> messageIds, CancellationToken ct);
}