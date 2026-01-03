namespace Portfolio.Application.Email.Interfaces;

public interface IEmailService
{
    public Task SendAsync(string from, string to, string subject, string message, bool isHtml = false, string? replyTo = null);
    public string GetAutoRespondEmail();
}