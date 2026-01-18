
using Microsoft.Extensions.Options;
using Portfolio.Application.Email.Interfaces;
using Portfolio.Application.Options;
using Portfolio.Application.StaticContent;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailService : IEmailService
{
    private readonly SendGridClient _sendGridClient;
    private readonly string _autoReplyHtml;

    public EmailService(IOptions<SendGridOptions> options, StaticContentPath staticContent)
    {
        _sendGridClient = new SendGridClient(options.Value.ApiKey);
        var path = Path.Combine(staticContent.Path, "auto-respond-email.html");   
        _autoReplyHtml = File.ReadAllText(path);
    }
    public async Task SendAsync(string from, string to, string subject, string message, bool isHtml, string? replyTo)
    {
        var fromAddress = new EmailAddress(from);
        var toAddress = new EmailAddress(to);

        var mail = MailHelper.CreateSingleEmail(
            from: fromAddress,
            to: toAddress,
            subject: subject,
            plainTextContent: isHtml ? string.Empty : message,
            htmlContent: isHtml ? message : string.Empty
        );

        await _sendGridClient.SendEmailAsync(mail);
    }

    public string GetAutoRespondEmail()
    {
        return _autoReplyHtml;
    }
}