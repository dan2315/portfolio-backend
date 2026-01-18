using Portfolio.Application.Messages.DTOs;

namespace Portfolio.Application.Messages.Interfaces;

public interface IMessagesService
{
    Task Add(MessageDTO messageDTO);
    Task<IReadOnlyCollection<AdminMessageDTO>> GetAll(string? filter);
    Task MarkProcessed(IReadOnlyCollection<Guid> messageIds);
}