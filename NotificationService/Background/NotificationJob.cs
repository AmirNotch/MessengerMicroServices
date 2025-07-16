using NotificationService.Service.IService;

namespace NotificationService.Background;

public class NotificationJob : AbstractJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public NotificationJob(IServiceScopeFactory serviceScopeFactory, ILogger<NotificationJob> logger) : base(logger, nameof(NotificationJob), 60000)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessor>();
        await processor.ProcessUnreadMessagesAsync(stoppingToken);
    }


    protected override async Task ExecuteOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            await DoWorkAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("{name} has been stopped.", _className);
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{name} encountered error", _className);
        }
    }
}