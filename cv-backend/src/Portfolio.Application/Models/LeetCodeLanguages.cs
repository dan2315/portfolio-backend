namespace Portfolio.Application.Models;

public class LeetCodeLanguages
{
    public List<ProblemsSolvedByLanguage> ProblemsSolvedByLanguages {get; set;}
}

public class ProblemsSolvedByLanguage
{
    public string LanguageName { get; set; }
    public int ProblemsSolved { get; set; }
}

