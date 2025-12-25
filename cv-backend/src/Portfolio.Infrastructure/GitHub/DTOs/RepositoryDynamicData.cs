namespace Portfolio.Infrastructure.GitHub.DTOs
{
    public class RepositoryDynamicData
    {
        public Repository Repository {get; set;}
    }

    public class Repository
    {
        public int StargazerCount {get; set;}
        public int ForkCount {get; set;}
        public Watchers Watchers {get; set;}
        public Issues Issues {get; set;}
        public PullRequests PullRequests {get; set;}
        public Releases Releases {get; set;}
    }

    public class Releases
    {
        public List<ReleaseNode> Nodes {get; set;}
    }

    public class ReleaseNode
    {
        public string TagName {get; set;}
        public string PublishedAt {get; set;}
    }

    public class PullRequests
    {
        public int TotalCount {get; set;}
    }

    public class Issues
    {
        public int TotalCount {get; set;}
    }

    public class Watchers
    {
        public int TotalCount {get; set;}
    }
}