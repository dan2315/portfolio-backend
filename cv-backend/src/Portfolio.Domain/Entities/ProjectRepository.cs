namespace Portfolio.Domain.Entities
{
    public class ProjectRepository
    {
        public ProjectRepository(
            string name,
            string? description,
            string url,
            string? homepageUrl,
            Language? primaryLanguage,
            List<Language> languages,
            DefaultBranchRef? defaultBranchRef,
            string? readme)
        {
            Name = name;
            Description = description;
            Url = url;
            HomepageUrl = homepageUrl;
            PrimaryLanguage = primaryLanguage;
            Languages = languages;
            DefaultBranchRef = defaultBranchRef;
            ReadmeMarkdown = readme;
        }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string Url { get; set; } = default!;
        public string? HomepageUrl { get; set; }
        public Language? PrimaryLanguage { get; set; }
        public List<Language> Languages { get; set; } = new();
        public DefaultBranchRef? DefaultBranchRef { get; set; }
        public string? ReadmeMarkdown { get; set; }
        public RepositoryDynamicData DynamicData {get; set;}
    }

    public class RepositoryDynamicData
    {
        public int StargazerCount {get; set;}
        public int ForkCount {get; set;}
        public int Watchers {get; set;}
        public int Issues {get; set;}
        public int PullRequests {get; set;}
        public List<Release> Releases {get; set;} = default!;
    }

    public class Release
    {        
        public string TagName {get; set;} = default!;
        public string PublishedAt {get; set;} = default!;
    }

    public class Language
    {
        public string Name { get; set; } = default!;
        public string? Color { get; set; }
    }

    public class DefaultBranchRef
    {
        public string Name { get; set; } = default!;
        public string Oid { get; set; } = default!;
        public List<Commit> Commits {get; set;} = new();
    }

    public class Commit
    {
        public string Message {get; set;} = default!;
        public string CommittedDate {get; set;} = default!;
        public string Oid {get; set;} = default!;
    }

}