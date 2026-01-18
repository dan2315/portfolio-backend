using Portfolio.Application.Messages.DTOs;
using Portfolio.Application.Messages.Interfaces;

namespace Portfolio.Application.Messages;

public class MessagesService : IMessagesService
{
    IMessagesRepository _messagesRepository;

    public MessagesService(IMessagesRepository messagesRepository)
    {
        _messagesRepository = messagesRepository;
    }

    public async Task Add(MessageDTO messageDTO)
    {
        await _messagesRepository.Add(new Domain.Entities.Message
        {
            Id = Guid.NewGuid(),
            From = messageDTO.From,
            Subject = messageDTO.Subject,
            Contents = messageDTO.Contents,
            Status = Domain.Entities.MessageStatus.Pending
        });
    }

    public async Task<IReadOnlyCollection<AdminMessageDTO>> GetAll(string? filter)
    {
        var messages = await _messagesRepository.GetAll(filter);
        return messages.Select(m => new AdminMessageDTO(m.Id, m.From, m.Subject, m.Contents)).ToList();
    }

    public async Task MarkProcessed(IReadOnlyCollection<Guid> messageIds)
    {
        await _messagesRepository.MarkProcessed(messageIds);
    }
}