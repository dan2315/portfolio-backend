namespace Portfolio.Api.DTOs;

public record SendEmailRequest(
    string From,
    string Subject,
    string Message
);