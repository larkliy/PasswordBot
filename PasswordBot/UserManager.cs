using PasswordBot.Models;
using System.Collections.Concurrent;

namespace PasswordBot;

internal static class UserManager
{
    private static ConcurrentDictionary<long, UserProfile> s_users = [];

    public static UserProfile GetOrCreate(long telegramId)
    {
        return s_users.GetOrAdd(telegramId, id => new UserProfile { TelegramId = id });
    }
}
