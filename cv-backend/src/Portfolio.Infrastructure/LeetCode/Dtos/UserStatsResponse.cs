public class UserStatsResponse
{
    public UserStatsData Data { get; set; }
}

public class UserStatsData
{
    public List<QuestionCount> AllQuestionsCount { get; set; }
    public MatchedUser MatchedUser { get; set; }
}

public class QuestionCount
{
    public string Difficulty { get; set; }
    public int Count { get; set; }
}

public class MatchedUser
{
    public List<BeatsStats> ProblemsSolvedBeatsStats { get; set; }
    public SubmitStatsGlobal SubmitStatsGlobal { get; set; }
}

public class BeatsStats
{
    public string Difficulty { get; set; }
    public double? Percentage { get; set; }
}

public class SubmitStatsGlobal
{
    public List<AcSubmission> AcSubmissionNum { get; set; }
}

public class AcSubmission
{
    public string Difficulty { get; set; }
    public int Count { get; set; }
}