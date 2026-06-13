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

    var user = await RegisterUserIfNotExists(msg.From!.Id);

    if (user.State == UserState.Idle)
    {
        var welcomeText =
        """
        Воспользуйтесь кнопками ниже:
        """;

        await bot.SendMessage(
            msg.From!.Id,
            welcomeText,
            replyMarkup: InlineKeyboardButton.WithCallbackData("🔑Сгенерировать пароль", "generate_password"),
            cancellationToken: token);
    }
    else
    {
        await user.CallbackMatcher!.HandleMessage(text, token);
    }
};

bot.OnUpdate += async (update) =>
{
    if (update.CallbackQuery is not { Data: { } data }) return;

    var userId = update.CallbackQuery.From.Id;

    var user = await RegisterUserIfNotExists(update.CallbackQuery.From.Id);

    await user.HandleStateMachineByCallback(bot, data, user, token);

    await bot.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: token);
};

bot.OnError += async (exception, source) =>
{
    Console.WriteLine(exception);
    await Task.Delay(2000, cts.Token);
};

while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
cts.Cancel();

async Task<UserProfile> RegisterUserIfNotExists(long userId)
{
    if (!UserManager.s_users.ContainsKey(userId))
    {
        UserManager.AddUser(new()
        {
            TelegramId = userId
        });

        await bot.SendMessage(userId, "Вы успешно зарегистрированы!", cancellationToken: token);
    }

    return UserManager.s_users[userId];
}