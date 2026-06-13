using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PasswordBot.States;

public class AddPasswordStateMachine
{
    public AddPasswordState State { get; set; } = AddPasswordState.Idle;
    public AddPasswordBuffer Buffer { get; set; } = new();
    public ITelegramBotClient Client { get; set; }
    public long FromId { get; set; }

    private readonly InlineKeyboardButton _cancelBtn 
        = InlineKeyboardButton.WithCallbackData("Отменить", "generate_password_cancel");

    public AddPasswordStateMachine(ITelegramBotClient client, long fromId)
    {
        Client = client;
        FromId = fromId;
    }

    public async Task Start(CancellationToken token = default)
    {
        if (State == AddPasswordState.Idle)
        {
            State = AddPasswordState.WaitForAppName;
            await Client.SendMessage(
                FromId, 
                "Введите название сервиса для пароля: ",
                replyMarkup: _cancelBtn,
                cancellationToken: token);
        }
    }

    public async Task Cancel(CancellationToken token = default)
    {
        if (State != AddPasswordState.Idle)
        {
            State = AddPasswordState.Idle;
            Buffer = new();

            await Client.SendMessage(
                FromId, 
                "Действие отменено.",
                cancellationToken: token);

            return;
        }
    }

    public async Task HandleMessage(string text, Action onCompleted, CancellationToken token = default)
    {
        switch (State)
        {
            case AddPasswordState.WaitForAppName:
                Buffer.AppName = text;
                State = AddPasswordState.WaitForPassLength;

                await Client.SendMessage(
                    FromId, 
                    "Введите длину пароля: ",
                    replyMarkup: _cancelBtn, cancellationToken: token);

                break;
            case AddPasswordState.WaitForPassLength:
                if (!int.TryParse(text, out int value) && value < 1 || value > 100)
                {
                    await Client.SendMessage(
                        FromId, 
                        "Введите корректное число от 1 до 100: ", 
                        replyMarkup: _cancelBtn, 
                        cancellationToken: token);

                    return;
                }
                Buffer.PasswordLength = value;
                State = AddPasswordState.WaitForPassSymbols;

                await Client.SendMessage(
                    FromId, 
                    "Введите символы которые должны быть в пароле (пример: 1234567ASDG): ", 
                    replyMarkup: _cancelBtn, 
                    cancellationToken: token);

                break;
            case AddPasswordState.WaitForPassSymbols:
                Buffer.PasswordSymbols = text;
                onCompleted();
                State = AddPasswordState.Idle;
                break;
        }
    }
}