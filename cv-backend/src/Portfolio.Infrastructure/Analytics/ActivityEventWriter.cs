
using System.Threading.Channels;

public class ActivityEventWriter : IActivityEventWriter
{
    private readonly Channel<ActivityEvent> _channel;

    public ActivityEventWriter(Channel<ActivityEvent> channel)
    {
        _channel = channel;
    }

    public ValueTask WriteAsync(ActivityEvent activityEvent)
    {
        return _channel.Writer.WriteAsync(activityEvent);
    }
}