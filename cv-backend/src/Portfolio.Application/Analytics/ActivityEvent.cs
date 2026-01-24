using System.Net;

namespace Portfolio.Application.Analytics
{
    public class ActivityEvent
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string EventType { get; set; } = default!;
        public string Route { get; set; } = default!;
        public string? Method { get; set; } = default!;
        public int StatusCode { get; set; }
        public long DurationMs { get; set; }
        public Guid? AnonymousId { get; set; }
        public Guid? SessionId { get; set; }
        public string? IPAddress;
        public string? UserAgent { get; set; }
        public string? AdditionalData { get; set; }
        public string? Referer { get; set; }
        public long? TimeOnPageMs { get; set; }
        public bool Processed { get; set; }
    }
}