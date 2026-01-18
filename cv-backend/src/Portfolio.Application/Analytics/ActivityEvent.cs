namespace Portfolio.Application.Analytics
{
    public class ActivityEvent
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string EventType { get; set; } = default!;
        public string Route { get; set; } = default!;
        public string Method { get; set; } = default!;
        public int StatusCode { get; set; }
        public long DurationMs { get; set; }
        public Guid? AnonymousSessionId { get; set; }
        public string? UserId { get; set; }
        public string? UserAgent { get; set; }
    }
}