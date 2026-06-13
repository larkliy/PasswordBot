namespace PasswordBot.States;

public abstract class CallbackMatcherBase
{
    public abstract TelegramStateMachineBase StateMachine { get; init; }

    public string Callback { get; init; }
    public string CancelCallback { get; init; }

    public CallbackMatcherBase(string callback)
    {
        Callback = callback;
        CancelCallback = Callback + "_cancel";
    }

    public abstract Task Begin(string callback, CancellationToken token);
    public abstract Task HandleMessage(string text, CancellationToken token);
}
