namespace PasswordBot.States.AddPassword;

public enum AddPasswordState
{
    Idle,
    WaitForAppName,
    WaitForPassLength,
    WaitForPassSymbols
}
