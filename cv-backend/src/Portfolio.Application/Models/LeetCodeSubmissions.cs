namespace Portfolio.Application.Models;
public class LeetCodeSubmissions
{
    public SubmissionItem[] Submissions {get; set;}
}

public class SubmissionItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string TitleSlug { get; set; }
    public string Timestamp { get; set; }
}