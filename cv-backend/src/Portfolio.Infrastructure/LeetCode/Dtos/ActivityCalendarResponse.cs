public class CalendarData
{
    public MatchedUserCalendar MatchedUser { get; set; }
}

public class MatchedUserCalendar
{
    public UserCalendar UserCalendar { get; set; }
}

public class UserCalendar
{
    public int[] ActiveYears { get; set; }
    public int Streak { get; set; }
    public int TotalActiveDays { get; set; }
    public string[] DccBadges { get; set; }
    public string SubmissionCalendar { get; set; } 
}