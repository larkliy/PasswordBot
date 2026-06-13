using System.Security.Cryptography;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PasswordBot.States.AddPassword;

public class AddPasswordStateMachine : TelegramStateMachineBase
{
    public AddPasswordState State { get; set; } = AddPasswordState.Idle;

    private readonly InlineKeyboardButton _cancelBtn 
        = InlineKeyboardButton.WithCallbackData("Отменить", "generate_password_cancel");

    public AddPasswordStateMachine(ITelegramBotClient client, long fromId) 
        : base(client, fromId) { }

    public override async Task Start(CancellationToken token = default)
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

    public override async Task Cancel(CancellationToken token = default)
    {
        if (State != AddPasswordState.Idle)
        {
            State = AddPasswordState.Idle;
            Buffer = [];

            await Client.SendMessage(
                FromId, 
                "Действие отменено.",
                cancellationToken: token);

            return;
        }
    }

    public override async Task HandleMessage(string text, CancellationToken token = default)
    {
        switch (State)
        {
            case AddPasswordState.WaitForAppName:

                Buffer[AddPasswordBufferConstants.AppName] = text;

                State = AddPasswordState.WaitForPassLength;

                await Client.SendMessage(
                    FromId, 
                    "Введите длину пароля: ",
                    replyMarkup: _cancelBtn, cancellationToken: token);

                break;
            case AddPasswordState.WaitForPassLength:
                if (!int.TryParse(text, out int value) || value < 1 || value > 100)
                {
                    await Client.SendMessage(
                        FromId, 
                        "Введите корректное число от 1 до 100: ", 
                        replyMarkup: _cancelBtn, 
                        cancellationToken: token);

                    return;
                }

                Buffer[AddPasswordBufferConstants.PasswordLength] = value;

                State = AddPasswordState.WaitForPassSymbols;

                await Client.SendMessage(
                    FromId, 
                    "Введите символы которые должны быть в пароле (пример: 1234567ASDG): ", 
                    replyMarkup: _cancelBtn, 
                    cancellationToken: token);

                break;
            case AddPasswordState.WaitForPassSymbols:

                Buffer[AddPasswordBufferConstants.PasswordSymbols] = text;

                await Completed(token);

                State = AddPasswordState.Idle;
                break;
        }
    }

    private async Task Completed(CancellationToken token = default)
    {
        var password = RandomNumberGenerator.GetString(
            (string)Buffer[AddPasswordBufferConstants.PasswordSymbols],
            (int)Buffer[AddPasswordBufferConstants.PasswordLength]);

        var passwordText =
        $"""
        Ваш пароль для приложения {(string)Buffer[AddPasswordBufferConstants.AppName]}:

        {password}
        """;

        await Client.SendMessage(FromId, passwordText, cancellationToken: token);
    }
}