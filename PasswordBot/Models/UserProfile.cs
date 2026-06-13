using PasswordBot.States;
using Telegram.Bot;

namespace PasswordBot.Models;

public class UserProfile
{
    public long TelegramId { get; set; }
    public GlobalState State { get; set; } = GlobalState.Idle;
    public AddPasswordStateMachine? PasswordGenStateMachine { get; set; }

    public async Task FireAddPasswordMachine(ITelegramBotClient bot, string callbackData, CancellationToken token = default)
    {
        switch (callbackData)
        {
            case "generate_password":
                PasswordGenStateMachine ??= new(bot, TelegramId);
                State = GlobalState.PasswordGenerating;
                await PasswordGenStateMachine.Start(token);
                break;

            case "generate_password_cancel":
                State = GlobalState.Idle;
                if (PasswordGenStateMachine is { } machine)
                    await machine.Cancel(token);
                break;
        }
    }
}
