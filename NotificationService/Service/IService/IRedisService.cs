namespace NotificationService.Service.IService;

public interface IRedisService
{
    /// <summary>
    /// Проверяет, онлайн ли пользователь в Redis.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>true, если пользователь онлайн, иначе false</returns>
    Task<bool> IsUserOnlineAsync(Guid userId);
}
