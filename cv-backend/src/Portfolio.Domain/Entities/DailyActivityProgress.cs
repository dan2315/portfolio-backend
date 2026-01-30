namespace Portfolio.Infrastructure.Entities;

public class DailyActivityProgress
{
    public int Id { get; set; }
    public DateOnly LastAggregatedDay { get; set; }
}