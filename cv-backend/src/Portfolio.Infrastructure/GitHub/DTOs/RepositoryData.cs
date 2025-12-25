public class RepositoryData
{
    public Repository Repository { get; set; } = default!;
}

public class Repository
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Url { get; set; } = default!;
    public string? HomepageUrl { get; set; }
    public Language? PrimaryLanguage { get; set; }
    public Languages Languages { get; set; } = default!;
    public DefaultBranchRef? DefaultBranchRef { get; set; }
    public RepositoryObject? Object { get; set; }
}

public class Language
{
    public string Name { get; set; } = default!;
    public string? Color { get; set; }
}

public class Languages
{
    public List<LanguageEdge> Edges { get; set; } = new();
}

public class LanguageEdge
{
    public Language Node { get; set; } = default!;
}

public class DefaultBranchRef
{
    public string Name { get; set; } = default!;
    public Target Target { get; set; } = default!;
}

public class Target
{
    public string Oid { get; set; } = default!;
    public History History { get; set; } = default!;
}

public class History
{
    public List<HistoryEdge> Edges { get; set; } = new();
}

public class HistoryEdge
{
    public HistoryNode Node {get; set;} = default!;
}

public class HistoryNode
{
    public string Message {get; set;} = default!;
    public string CommittedDate {get; set;} = default!;
    public string Oid {get; set;} = default!;
}

public class RepositoryObject
{
    public string? Text { get; set; }
}
