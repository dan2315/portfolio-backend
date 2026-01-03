public sealed class SmtpOptions
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string User { get; init; } = null!;
    public string Pass { get; init; } = null!;
}