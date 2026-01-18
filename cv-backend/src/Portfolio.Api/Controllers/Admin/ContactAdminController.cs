using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Email.Interfaces;
using Portfolio.Application.Messages.DTOs;
using Portfolio.Application.Messages.Interfaces;

namespace Portfolio.Api.Controllers.Admin;

[ApiController]
[RequireAdminApiKey]
[Route("/admin/contact")]
public class ContactAdminController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IMessagesService _messagesService;
    private readonly string _myEmail;

    public ContactAdminController(IEmailService emailService, IMessagesService messagesService, IConfiguration configuration)
    {
        _emailService = emailService;
        _messagesService = messagesService;
        _myEmail = configuration["SMTP:User"] ?? throw new Exception("Configuration for SMTP:User is not set");
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IReadOnlyCollection<AdminMessageDTO>>> GetAllMessages([FromQuery] string? filter)
    {
        var messages = await _messagesService.GetAll(filter);

        return Ok(messages);
    } 

    [HttpPatch("messages")]
    public async Task<ActionResult> MarkMessagesAsProcessed([FromBody] IReadOnlyCollection<Guid> messageIds)
    {
        await _messagesService.MarkProcessed(messageIds);

        return Ok();
    }
}