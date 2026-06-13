using PasswordBot.Models;
using Telegram.Bot;

namespace PasswordBot.States.AddPassword;

public class AddPasswordCallbackMatcher : CallbackMatcherBase
{
    private readonly UserProfile _userProfile;

    public override TelegramStateMachineBase StateMachine { get; init; }

    public AddPasswordCallbackMatcher(ITelegramBotClient bot, UserProfile userProfile, string callback) :
        base(callback)
    {
        _userProfile = userProfile;
        StateMachine = new AddPasswordStateMachine(bot, _userProfile.TelegramId);
    }

    public override async Task Begin(string callback, CancellationToken token = default)
    {
        if (_userProfile.State == UserState.Idle)
        {
            _userProfile.State = UserState.PasswordGenerating;
            await StateMachine.Start(token);
        }
        else if (_userProfile.State != UserState.Idle && callback == CancelCallback)
        {
            _userProfile.State = UserState.Idle;
            
            if (StateMachine is { } machine)
            {
                await machine.Cancel(token);
            }
        }
    }

    public override async Task HandleMessage(string text, CancellationToken token = default)
    {
        await StateMachine.HandleMessage(text, token);
    }
}
