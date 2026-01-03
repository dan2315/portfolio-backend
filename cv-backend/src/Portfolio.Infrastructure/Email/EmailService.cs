
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Portfolio.Application.Email.Interfaces;
using Portfolio.Application.StaticContent;

public class EmailService : IEmailService
{
    private SmtpOptions _options;
    private readonly string _autoReplyHtml;

    public EmailService(IOptions<SmtpOptions> options, StaticContentPath staticContent)
    {
        _options = options.Value;
        var path = Path.Combine(staticContent.Path, "auto-respond-email.html");   
        _autoReplyHtml = File.ReadAllText(path);
    }
    public async Task SendAsync(string from, string to, string subject, string message, bool isHtml, string? replyTo)
    {
        Console.WriteLine($"Sending email {from} {to} {subject} {message}");
        using var smtp = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(
                _options.User,
                _options.Pass
            ),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_options.User),
            Subject = subject,
            Body = message,
            IsBodyHtml = isHtml
        };

        mail.To.Add(to);
        if (!string.IsNullOrWhiteSpace(replyTo))
        {
            mail.ReplyToList.Add(new MailAddress(replyTo));
        }

        await smtp.SendMailAsync(mail);
    }

    public string GetAutoRespondEmail()
    {
        return _autoReplyHtml;
    }
}