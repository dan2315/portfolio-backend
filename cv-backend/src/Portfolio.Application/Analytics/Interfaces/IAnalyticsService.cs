namespace Portfolio.Application.Analytics.Interfaces;

public interface IAnalyticsService
{
    Task<object> GetOrComputeSessionsHeatmap(int year);
}