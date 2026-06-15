using PasswordBot.Models;
using System.Security.Cryptography;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PasswordBot.Dialogs;

public class PasswordGeneratorDialog : DialogBase
{
    public override string Callback => "generate_password";

    private readonly ITelegramBotClient _bot;
    private readonly UserProfile _user;

    private InlineKeyboardButton CancelBtn
        => InlineKeyboardButton.WithCallbackData("Отменить", CancelCallback);

    private enum Step { Idle, WaitAppName, WaitLength, WaitSymbols }
    private Step _step;
    private string _appName = "";
    private int _passwordLength;
    private string _symbols = "";

    public PasswordGeneratorDialog(ITelegramBotClient bot, UserProfile user)
    {
        _bot = bot;
        _user = user;
    }

    public override async Task Start(CancellationToken token)
    {
        _step = Step.WaitAppName;
        await _bot.SendMessage(_user.TelegramId,
            "Введите название сервиса для пароля:",
            replyMarkup: CancelBtn,
            cancellationToken: token);
    }

    public override async Task HandleMessage(string text, CancellationToken token)
    {
        switch (_step)
        {
            case Step.WaitAppName:
                _appName = text;
                _step = Step.WaitLength;
                await _bot.SendMessage(_user.TelegramId,
                    "Введите длину пароля:",
                    replyMarkup: CancelBtn,
                    cancellationToken: token);
                break;

            case Step.WaitLength:
                if (!int.TryParse(text, out int length) || length < 1 || length > 100)
                {
                    await _bot.SendMessage(_user.TelegramId,
                        "Введите число от 1 до 100:",
                        replyMarkup: CancelBtn,
                        cancellationToken: token);
                    return;
                }
                _passwordLength = length;
                _step = Step.WaitSymbols;
                await _bot.SendMessage(_user.TelegramId,
                    "Какие символы использовать? (например: abc123!@#):",
                    replyMarkup: CancelBtn,
                    cancellationToken: token);
                break;

            case Step.WaitSymbols:
                _symbols = text;
                await Complete(token);
                break;
        }
    }

    public override async Task Cancel(CancellationToken token)
    {
        _step = Step.Idle;
        await _bot.SendMessage(_user.TelegramId, "Действие отменено.", cancellationToken: token);
    }

    private async Task Complete(CancellationToken token)
    {
        var password = RandomNumberGenerator.GetString(_symbols, _passwordLength);
        var msg = $"Ваш пароль для {_appName}:\n\n{password}";

        await _bot.SendMessage(_user.TelegramId, msg, cancellationToken: token);

        IsCompleted = true;
    }
}
