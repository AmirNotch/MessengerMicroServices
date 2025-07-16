using NotificationService.Service.IService;
using StackExchange.Redis;

namespace NotificationService.Service;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        // Предположим, что ключ сохраняется как: user:{userId}:online
        var key = $"user:{userId}:online";
        var value = await _database.StringGetAsync(key);

        return value.HasValue && value == "online";
    }
}
