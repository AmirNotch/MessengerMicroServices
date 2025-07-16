using NotificationService.Service.IService;

namespace NotificationService.Service;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(Guid userId, string subject, string content, CancellationToken cancellationToken)
    {
        // В проде здесь интеграция с SMTP, SendGrid и т.п.
        _logger.LogInformation("Email to User {UserId}: {Subject}\n{Content}", userId, subject, content);
        return Task.CompletedTask;
    }
}
