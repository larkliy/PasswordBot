
namespace PasswordBot.Dialogs;

public abstract class DialogBase
{
    public abstract string Callback { get; }
    public string CancelCallback => Callback + "_cancel";
    public bool IsCompleted { get; protected set; }

    public abstract Task Start(CancellationToken token);
    public abstract Task HandleMessage(string text, CancellationToken token);
    public abstract Task Cancel(CancellationToken token);
}
