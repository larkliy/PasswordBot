namespace PasswordBot.States;

public enum AddPasswordState
{
    Idle,
    WaitForAppName,
    WaitForPassLength,
    WaitForPassSymbols
}
