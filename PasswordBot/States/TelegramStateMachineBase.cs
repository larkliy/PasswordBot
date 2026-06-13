using Telegram.Bot;

namespace PasswordBot.States;

public abstract class TelegramStateMachineBase
{
    public ITelegramBotClient Client { get; set; } = null!;
    public long FromId { get; set; }
    public Dictionary<string, object> Buffer { get; set; }

    public TelegramStateMachineBase(ITelegramBotClient client, long fromId)
    {
        Client = client;
        FromId = fromId;
        Buffer = [];
    }

    public abstract Task Start(CancellationToken token);
    public abstract Task Cancel(CancellationToken token);
    public abstract Task HandleMessage(string text, CancellationToken token);
}
