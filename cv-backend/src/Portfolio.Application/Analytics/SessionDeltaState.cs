namespace Portfolio.Application.Analytics;

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
    public DateTimeOffset SessionExpiresAt { get; private set; }

    public SessionDeltaState(ActivityEvent activityEvent)
    {
        AnonymousId = activityEvent.AnonymousId.Value;
        SessionId = activityEvent.SessionId ?? throw new ArgumentException("Session cannot be created from event without Session ID");
        LeastStartTime = activityEvent.Timestamp;
        GreatestEndTime = activityEvent.Timestamp;
        IncrementStat(activityEvent);
        TotalTimeMs = activityEvent.TimeOnPageMs??0;
        SessionExpiresAt = GreatestEndTime + TimeSpan.FromMinutes(30);
    }

    public void ApplyEvent(ActivityEvent activityEvent)
    {
        if (activityEvent.SessionId == null || activityEvent.SessionId != SessionId)
            throw new ArgumentException("Event does not belong to this session");

        if (activityEvent.Timestamp < LeastStartTime)
        {
            LeastStartTime = activityEvent.Timestamp;
        }

        if (activityEvent.Timestamp > GreatestEndTime)
        {
            GreatestEndTime = activityEvent.Timestamp;
        }

        IncrementStat(activityEvent);

        TotalTimeMs = (long)(GreatestEndTime - LeastStartTime).TotalMilliseconds;
        SessionExpiresAt = GreatestEndTime + TimeSpan.FromMinutes(30);
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

    public void Reset()
    {
        PagesViewed = 0;
        CartridgesInserted = 0;
        ContactAttempts = 0;
    }
}