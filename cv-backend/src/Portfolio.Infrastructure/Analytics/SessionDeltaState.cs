using Portfolio.Application.Analytics;

namespace Workers.Analytics;

public class SessionDeltaState
{
    public Guid AnonymousId { get; private set; }
    public Guid SessionId { get; private set; }
    public DateTimeOffset LeastStartTime { get; private set; }
    public DateTimeOffset GreatestEndTime { get; private set; }
    public int PagesViewed { get; private set; }
    public int CartridgesInserted { get; private set; }
    public int ContactAttempts { get; private set; }
    public long TotalTimeMs { get; private set; }

    public SessionDeltaState(ActivityEvent activityEvent)
    {
        AnonymousId = activityEvent.AnonymousId.Value;
        SessionId = activityEvent.SessionId ?? throw new ArgumentException("Session cannot be created from event without Session ID");
        LeastStartTime = activityEvent.Timestamp;
        GreatestEndTime = activityEvent.TimeOnPageMs != null ?
            activityEvent.Timestamp.AddMilliseconds(activityEvent.TimeOnPageMs.Value) :
            activityEvent.Timestamp;
        IncrementStat(activityEvent);
        TotalTimeMs = activityEvent.TimeOnPageMs??0;
    }

    public void ApplyEvent(ActivityEvent activityEvent)
    {
        if (activityEvent.SessionId == null || activityEvent.SessionId != SessionId)
            throw new ArgumentException("Event does not belong to this session");

        if (activityEvent.Timestamp < LeastStartTime)
        {
            LeastStartTime = activityEvent.Timestamp;
        }

        var eventEndTime = activityEvent.TimeOnPageMs != null
            ? activityEvent.Timestamp.AddMilliseconds(activityEvent.TimeOnPageMs.Value) // TODO: bug 2082 year
            : activityEvent.Timestamp;

        if (eventEndTime > GreatestEndTime)
        {
            GreatestEndTime = eventEndTime;
        }

        IncrementStat(activityEvent);

        TotalTimeMs = (long)(GreatestEndTime - LeastStartTime).TotalMilliseconds;
    }

    public void IncrementStat(ActivityEvent activityEvent)
    {
        switch (activityEvent.EventType)
        {
            case "page_view": this.PagesViewed++; break;
            case "cartridge_inserted": this.CartridgesInserted++; break;
            case "contact_attempt": this.ContactAttempts++; break;
        } 
    }
}