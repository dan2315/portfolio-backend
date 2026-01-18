using Portfolio.Domain.Entities;

namespace Portfolio.Application.Messages.Interfaces;

public interface IMessagesRepository
{
    public Task Add(Message message);
    public Task<IReadOnlyList<Message>> GetAll(string? filter);
    public Task MarkProcessed(IReadOnlyCollection<Guid> ids);
}