public interface IActivityEventWriter
{
    public Task WriteAsync(ActivityEvent activityEvent);
}