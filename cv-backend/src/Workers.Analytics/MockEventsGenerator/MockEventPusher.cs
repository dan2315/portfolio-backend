using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator;

public class MockEventPusher : BackgroundService
{
    private const int MAX_ACTIVE_PUBLISHERS = 15;
    private readonly IActivityEventWriter _writer;
    private readonly Random random = new();
    List<Publisher> activePublishers = [];
    public MockEventPusher(IActivityEventWriter writer)
    {
        _writer = writer;
        AddNewPublisher();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await TickPublishMessages();
        } 
    }

    private async Task TickPublishMessages()
    {
        if (activePublishers.Count < MAX_ACTIVE_PUBLISHERS && random.NextSingle() < 0.1)
        {
            AddNewPublisher();
        }
        Console.WriteLine($"Active publishers: {activePublishers.Count}");

        for (int i = 0; i < activePublishers.Count; i++)
        {
            var publisher = activePublishers[i];

            if (--publisher.Ttl <= 0)
            {
                activePublishers.RemoveAt(i);
                continue;
            }

            if (random.NextSingle() < 0.1)
            {
                Console.WriteLine($"Sending event from: {publisher.AnonymousId}");
                var @event = new ActivityEvent
                {
                    SessionId = publisher.SessionId,
                    AnonymousId = publisher.AnonymousId,
                    EventType = "page_view",
                    Timestamp = DateTimeOffset.UtcNow
                }; 
                await _writer.WriteAsync(@event);
            }
        }
    }

    private void AddNewPublisher()
    {
        activePublishers.Add(new Publisher(Guid.NewGuid(), Guid.NewGuid(), random.Next(1*60, 5*60+1)));
    }

}

sealed class Publisher
{
    public Publisher(Guid guid1, Guid guid2, int v)
    {
        SessionId = guid1;
        AnonymousId = guid2;
        Ttl = v;
    }

    public Guid SessionId { get; }
    public Guid AnonymousId { get; }
    public int Ttl { get; set; }
}