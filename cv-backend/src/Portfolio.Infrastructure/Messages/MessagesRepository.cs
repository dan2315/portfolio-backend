using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Messages.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Messages;

public class MessagesRepository : IMessagesRepository
{
    private readonly AppDbContext _db;
    public MessagesRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task Add(Message message)
    {
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Message>> GetAll(string? filter)
    {
        MessageStatus? status = null;

        if (!string.IsNullOrEmpty(filter))
        {
            if (Enum.TryParse<MessageStatus>(filter, true, out var parsedStatus))
            {
                status = parsedStatus;
            }
            else
            {
                throw new ArgumentException($"Invalid MessageStatus: {filter}");
            }
        }

        var query = _db.Messages.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Message>> GetAllUnprocessed()
    {
        return await _db.Messages.Where(m => m.Status == MessageStatus.Pending).ToListAsync();
    }

    public async Task MarkProcessed(IReadOnlyCollection<Guid> ids)
    {
        await _db.Messages.Where(m => ids.Contains(m.Id)).ExecuteUpdateAsync(s => s.SetProperty(m => m.Status, MessageStatus.Processed));
    }
}