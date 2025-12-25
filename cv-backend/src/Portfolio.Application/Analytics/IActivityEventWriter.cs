public interface IActivityEventWriter
{
    public ValueTask WriteAsync(ActivityEvent activityEvent);
}