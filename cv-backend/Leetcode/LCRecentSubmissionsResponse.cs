public class LCRecentSubmissionsResponse
{
    public RecentSubmissionsData Data { get; set; }
}

public class RecentSubmissionsData
{
    public SubmissionItem[] RecentAcSubmissionList  { get; set; }
}

public class SubmissionItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string TitleSlug { get; set; }
    public string Timestamp { get; set; }
}