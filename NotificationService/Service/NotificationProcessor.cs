using NotificationService.Service.IService;

namespace NotificationService.Service;

public class NotificationProcessor : INotificationProcessor
{
    private readonly IChatServiceClient _chatServiceClient;
    private readonly IRedisService _redisService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        IChatServiceClient chatServiceClient,
        IRedisService redisService,
        IEmailSender emailSender,
        ILogger<NotificationProcessor> logger)
    {
        _chatServiceClient = chatServiceClient;
        _redisService = redisService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task ProcessUnreadMessagesAsync(CancellationToken cancellationToken)
    {
        var unreadMessages = await _chatServiceClient.GetUnreadMessagesAsync(5, cancellationToken);
        var groupedByUser = unreadMessages.GroupBy(m => m.ReceiverUserId);

        foreach (var group in groupedByUser)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userId = group.Key;
            var isOnline = await _redisService.IsUserOnlineAsync(userId);

            if (isOnline)
            {
                _logger.LogInformation("User {UserId} is online, skipping notification.", userId);
                continue;
            }

            var messageSummary = string.Join("\n", group.Select(m => $"- {m.Text}"));

            await _emailSender.SendEmailAsync(
                userId,
                "Вы пропустили сообщения!",
                $"У вас есть новые сообщения:\n{messageSummary}",
                cancellationToken);

            await _chatServiceClient.MarkMessagesNotifiedAsync(group.Select(m => m.MessageId), cancellationToken);
        }
    }
}
