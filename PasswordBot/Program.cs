using PasswordBot;
using PasswordBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

using var cts = new CancellationTokenSource();
var token = cts.Token;

Console.CancelKeyPress += (s, e) =>
{
    cts.Cancel();
    e.Cancel = true;
};

var bot = new TelegramBotClient("BOT-TOKEN");
await bot.DeleteWebhook();
await bot.DropPendingUpdates();

var me = await bot.GetMe();
Console.WriteLine($"Бот {me.Username} запущён!");

bot.OnMessage += async (msg, type) =>
{
    if (msg.Text is not { } text) return;

    var user = UserManager.GetOrCreate(msg.From!.Id);

    if (user.State == UserState.Idle)
    {
        await bot.SendMessage(msg.From!.Id,
            "Воспользуйтесь кнопками ниже:",
            replyMarkup: InlineKeyboardButton.WithCallbackData("🔑 Сгенерировать пароль", "generate_password"),
            cancellationToken: token);
    }
    else
    {
        await user.HandleMessage(text, token);
    }
};

bot.OnUpdate += async (update) =>
{
    if (update.CallbackQuery is not { Data: { } data }) return;

    var user = UserManager.GetOrCreate(update.CallbackQuery.From.Id);

    await user.HandleCallback(bot, data, token);

    await bot.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: token);
};

bot.OnError += async (exception, source) =>
{
    Console.WriteLine(exception);
    await Task.Delay(2000, cts.Token);
};

while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
cts.Cancel();
