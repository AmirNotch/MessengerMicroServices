using NotificationService.Models.UnreadMessages;
using NotificationService.Service.IService;

namespace NotificationService.Service;

public class ChatServiceClient : IChatServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatServiceClient> _logger;

    public ChatServiceClient(HttpClient httpClient, ILogger<ChatServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<UnreadMessageDto>> GetUnreadMessagesAsync(int olderThanMinutes, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"/api/messages/unread?olderThanMinutes={olderThanMinutes}", ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<UnreadMessageDto>>(cancellationToken: ct);
        return result ?? new List<UnreadMessageDto>();
    }

    public async Task MarkMessagesNotifiedAsync(IEnumerable<Guid> messageIds, CancellationToken ct)
    {
        var content = JsonContent.Create(messageIds.ToList());
        var response = await _httpClient.PostAsync("/api/messages/mark-notified", content, ct);
        response.EnsureSuccessStatusCode();
    }
}
