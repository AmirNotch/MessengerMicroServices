namespace Messenger.Models.Chats;

public class UserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}