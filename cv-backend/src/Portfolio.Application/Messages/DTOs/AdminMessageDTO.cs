namespace Portfolio.Application.Messages.DTOs
{
    public record AdminMessageDTO(
        Guid Id,
        string From,
        string Subject,
        string Contents
    );
}