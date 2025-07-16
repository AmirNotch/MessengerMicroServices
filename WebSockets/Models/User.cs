namespace WebSockets.Models;

public class User
{
    public DateTimeOffset? ConfirmedAt { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}