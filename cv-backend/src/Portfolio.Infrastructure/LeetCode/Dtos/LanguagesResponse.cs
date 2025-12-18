public class LanguagesResponse
{
    public LanguagesData Data { get; set; }
}

public class LanguagesData
{
    public MatchedUserLanguages MatchedUser { get; set; }
}

public class MatchedUserLanguages
{
    public List<ProblemsSolvedByLanguage> LanguageProblemCount { get; set; }
}

public class ProblemsSolvedByLanguage
{
    public string LanguageName { get; set; }
    public int ProblemsSolved { get; set; }
}