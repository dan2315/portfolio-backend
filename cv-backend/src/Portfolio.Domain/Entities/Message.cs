namespace Portfolio.Domain.Entities;

public class Message()
{
    public Guid Id { get; set; }
    public required string From { get; set; }
    public required string Subject { get; set; }
    public required string Contents { get; set; }
    public MessageStatus Status { get; set; }
}

public enum MessageStatus
{
    Pending = 0,
    Processed = 1,
}