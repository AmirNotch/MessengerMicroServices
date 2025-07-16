namespace NotificationService.Service.IService;

public interface INotificationProcessor
{
    /// <summary>
    /// Обрабатывает все непрочитанные и неуведомленные сообщения старше указанного времени.
    /// Проверяет онлайн-статус пользователя и отправляет уведомления на Email,
    /// если пользователь оффлайн.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Задача, представляющая процесс уведомлений</returns>
    Task ProcessUnreadMessagesAsync(CancellationToken cancellationToken);
}
