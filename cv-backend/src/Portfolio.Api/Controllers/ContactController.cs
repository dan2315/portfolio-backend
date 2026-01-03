using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Application.Email.Interfaces;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("/contact")]
public class ContactController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly string _myEmail;

    public ContactController(IEmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _myEmail = configuration["SMTP:User"] ?? throw new Exception("Configuration for SMTP:User is not set");
    }

    [HttpPost("email")]
    public async Task<ActionResult<SendEmailResponse>> SendEmail(SendEmailRequest request)
    {
        try
        {
            await _emailService.SendAsync(
                from: _myEmail,
                to: _myEmail,
                subject: request.Subject,
                message: request.Message,
                replyTo: request.From
            );

            await _emailService.SendAsync(
                from: _myEmail,
                to: request.From,
                subject: "Thanks for reaching out",
                message: _emailService.GetAutoRespondEmail(),
                isHtml: true
            );

            return Ok(new SendEmailResponse(true));
        }
        catch (Exception e)
        {
            return BadRequest(new SendEmailResponse(false, $"Failed to send email: {e.Message}"));
        }
    }
}