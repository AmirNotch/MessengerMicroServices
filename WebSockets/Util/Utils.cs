using WebSockets.Models;

namespace WebSockets.Util;

public class Utils
{
    public static User? GetUserFromContext(HttpContext context)
    {
        object? user = context.Items["user"];
        if (user == null || !(user is User))
        {
            return null;
        }
        return (User)user;
    }
}