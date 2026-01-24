using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator.SessionGenerators
{
    public sealed class NormalSessionGenerator : ISessionPatternGenerator
    {
        private static readonly string[] Pages =
        [
            "/", "/projects", "/contact"
        ];

        private static readonly string[] ApiRoutes =
        [
            "/api/projects",
            "/api/contact"
        ];

        private readonly Random _rnd = Random.Shared;

        public IEnumerable<ActivityEvent> Generate(
            DateTimeOffset startTime,
            Guid sessionId,
            Guid anonId
        )
        {
            var now = startTime;

            int steps = _rnd.Next(10, 21);

            for (int i = 0; i < steps; i++)
            {
                // page_view
                yield return new ActivityEvent
                {
                    Timestamp = now,
                    EventType = "page_view",
                    Route = RandomPage(),
                    SessionId = sessionId,
                    AnonymousId = anonId
                };

                now = now.AddMilliseconds(_rnd.Next(100, 800));

                // api
                yield return new ActivityEvent
                {
                    Timestamp = now,
                    EventType = "api",
                    Route = RandomApi(),
                    Method = "GET",
                    StatusCode = RandomStatus(),
                    DurationMs = _rnd.Next(30, 900),
                    SessionId = sessionId,
                    AnonymousId = anonId
                };

                now = now.AddMilliseconds(_rnd.Next(50, 400));
            }
        }

        private string RandomPage()
        {
            if (_rnd.NextDouble() < 0.3)
                return $"/projects/{_rnd.Next(1, 500)}";

            return Pages[_rnd.Next(Pages.Length)];
        }

        private string RandomApi()
        {
            if (_rnd.NextDouble() < 0.4)
                return $"/api/projects/{_rnd.Next(1, 500)}";

            return ApiRoutes[_rnd.Next(ApiRoutes.Length)];
        }

        private int RandomStatus()
        {
            var p = _rnd.NextDouble();

            if (p < 0.85) return 200;
            if (p < 0.95) return 404;
            return 500;
        }
    }
}