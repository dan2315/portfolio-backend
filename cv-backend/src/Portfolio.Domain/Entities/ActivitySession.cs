using System.ComponentModel.DataAnnotations;

namespace Portfolio.Domain.Entities;

public class ActivitySession
{
    [Key]
    public Guid SessionId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int PagesViewed { get; set; }
    public int CartridgesInserted { get; set; }
    public int ContactAttempted { get; set; }
    public long TotalTimeMs { get; set; }
    public string? AdditionalData { get; set; }
}