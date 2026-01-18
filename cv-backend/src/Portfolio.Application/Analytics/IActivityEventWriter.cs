using Portfolio.Application.Analytics;

public interface IActivityEventWriter
{
    public ValueTask WriteAsync(ActivityEvent activityEvent);
}