public static class GitHubQuerries
{
    public const string RepoStaticDataQuery = """
        query RepoStatic($owner: String!, $name: String!, $id: ID) {
        repository(owner: $owner, name: $name) {
            name
            description
            url
            homepageUrl
            licenseInfo {
            name
            }
            primaryLanguage {
            name
            color
            }
            languages(first: 10, orderBy: { field: SIZE, direction: DESC }) {
            edges {
                node {
                name
                color
                }
            }
            }
            defaultBranchRef {
            name
            target {
                ... on Commit {
                oid
                history(first: 5, author: { id: $id }) {
                    edges {
                    node {
                        message
                        committedDate
                        oid
                    }
                    }
                }
                }
            }
            }
            object(expression: "HEAD:README.md") {
            ... on Blob {
                text
            }
            }
        }
        }
    """;
    
    public const string RepoDynamicDataQuery = """
        query RepoDynamic($owner: String!, $name: String!) {
        repository(owner: $owner, name: $name) {
            stargazerCount
            forkCount
            watchers {
            totalCount
            }
            issues(states: OPEN) {
            totalCount
            }
            pullRequests(states: OPEN) {
            totalCount
            }
            releases(last: 1) {
            nodes {
                tagName
                publishedAt
            }
            }
        }
        }
    """;

    public const string GetUserId = """
        query GetUserId($owner: String!) {
        user(login: $owner) {
            id
        }
        }
    """;
}