using System.Collections.Concurrent;

namespace Portfolio.Infrastructure.GitHub.Cache
{
    public class ParsedUrlCache<TClient>
    {
        private ConcurrentDictionary<string, Tuple<string, string>> parsedUrls = new();

        public Tuple<string, string> TryGet(string url, Func<string, Tuple<string, string>> factoryMethod)
        {
            return parsedUrls.GetOrAdd(url, factoryMethod);
        }
    }
}