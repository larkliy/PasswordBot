using PasswordBot.States;
using PasswordBot.States.AddPassword;
using Telegram.Bot;

namespace PasswordBot.Models;

public class UserProfile
{
    public long TelegramId { get; set; }
    public UserState State { get; set; } = UserState.Idle;
    public CallbackMatcherBase? CallbackMatcher { get; set; }

    public async Task HandleStateMachineByCallback(ITelegramBotClient bot,
                                                   string callbackData,
                                                   CancellationToken token = default)
    {
        if (callbackData.StartsWith(CallbackNames.GeneratePassword))
        {
            if (CallbackMatcher is not AddPasswordCallbackMatcher)
            {
                CallbackMatcher = new AddPasswordCallbackMatcher(bot, this, CallbackNames.GeneratePassword);
            }
            await CallbackMatcher.Begin(callbackData, token);
        }
    }
}
