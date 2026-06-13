using PasswordBot.States;
using Telegram.Bot;

namespace PasswordBot.Models;

public class UserProfile
{
    public long TelegramId { get; set; }
    public GlobalState State { get; set; } = GlobalState.Idle;
    public TelegramStateMachineBase? StateMachine {  get; set; }

    public async Task FireAddPasswordMachine(ITelegramBotClient bot, string callbackData, CancellationToken token = default)
    {
        switch (callbackData)
        {
            case "generate_password":
                StateMachine ??= new AddPasswordStateMachine(bot, TelegramId);

                State = GlobalState.PasswordGenerating;
                await StateMachine.Start(token);
                break;

            case "generate_password_cancel":
                State = GlobalState.Idle;
                if (StateMachine is { } machine)
                    await machine.Cancel(token);
                break;
        }
    }
}
