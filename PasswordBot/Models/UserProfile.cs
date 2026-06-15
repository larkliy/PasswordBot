using PasswordBot.Dialogs;
using Telegram.Bot;

namespace PasswordBot.Models;

public class UserProfile
{
    public long TelegramId { get; set; }
    public UserState State { get; set; } = UserState.Idle;
    public DialogBase? CurrentDialog { get; set; }

    public async Task HandleCallback(ITelegramBotClient bot, string data, CancellationToken token)
    {
        if (CurrentDialog != null && data == CurrentDialog.CancelCallback)
        {
            State = UserState.Idle;
            await CurrentDialog.Cancel(token);
            CurrentDialog = null;
            return;
        }

        if (State == UserState.Idle && data == "generate_password")
        {
            State = UserState.PasswordGenerating;
            CurrentDialog = new PasswordGeneratorDialog(bot, this);
            await CurrentDialog.Start(token);
        }
    }

    public async Task HandleMessage(string text, CancellationToken token)
    {
        if (CurrentDialog == null) return;

        await CurrentDialog.HandleMessage(text, token);

        if (CurrentDialog.IsCompleted)
        {
            State = UserState.Idle;
            CurrentDialog = null;
        }
    }
}
