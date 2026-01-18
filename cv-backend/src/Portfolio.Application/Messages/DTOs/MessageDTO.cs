namespace Portfolio.Application.Messages.DTOs
{
    public record MessageDTO(
        string From,
        string Subject,
        string Contents
    );
}