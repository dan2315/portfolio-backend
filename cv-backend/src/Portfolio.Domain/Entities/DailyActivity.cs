using System.ComponentModel.DataAnnotations;

namespace Portfolio.Domain.Entities;

public class DailyActivity
{   
    [Key]
    public DateTime Date { get; set; }
    public int SessionsCount { get; set; }
    public int UniqueUsersCount { get; set; }
    public int PageViews { get; set; }
    public int AverageSessionDurationMs { get; set; }
}