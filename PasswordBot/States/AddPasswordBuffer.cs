namespace PasswordBot.States;

public class AddPasswordBuffer
{
    public string AppName { get; set; } = null!;
    public int PasswordLength { get; set; }
    public string PasswordSymbols { get; set; } = null!;
}
