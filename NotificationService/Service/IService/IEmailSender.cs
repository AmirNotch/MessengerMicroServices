namespace NotificationService.Service.IService;

public interface IEmailSender
{
    Task SendEmailAsync(Guid userId, string subject, string content, CancellationToken cancellationToken);
}
