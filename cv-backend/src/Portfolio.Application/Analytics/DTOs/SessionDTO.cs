namespace Portfolio.Application.Analytics.DTOs;

public record SessionDTO(
    Guid SessionId,  // will be funny to replace with slug [adverb+animal]
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,    
    int PagesViewed,
    int CartridgesInserted,
    int ContactAttempted,
    long TotalTimeMs,
    string? AdditionalData
);