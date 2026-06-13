using PasswordBot.Models;
using System.Collections.Concurrent;

namespace PasswordBot;

internal class UserManager
{
    public static ConcurrentDictionary<long, UserProfile> s_users = [];

    public static void AddUser(UserProfile user)
    {
        s_users.TryAdd(user.TelegramId, user);
    }

    public static void RemoveUser(UserProfile user)
    {
        s_users.Remove(user.TelegramId, out _);
    }
}
