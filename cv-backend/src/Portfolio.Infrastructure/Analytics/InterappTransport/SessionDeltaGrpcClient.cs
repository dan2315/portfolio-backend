using Analytics.Live;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Portfolio.Application.Analytics;

namespace Portfolio.Infrastructure.Analytics.InterappTransport;

public class SessionDeltaGrpcClient
{
    private readonly SessionDeltaService.SessionDeltaServiceClient _client;

    public SessionDeltaGrpcClient(IConfiguration configuration)
    {
        var apiUrl = configuration["Grpc:ApiUrl"] ?? throw new Exception("Config for gRpc is not provided");
        var channel = GrpcChannel.ForAddress(apiUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        });
        
        _client = new SessionDeltaService.SessionDeltaServiceClient(channel);
    }

    public async Task PublishSessionDeltaAsync(SessionDeltaState session)
    {
        var payload = new SessionDeltaPayload
        {
            AnonymousId = session.AnonymousId.ToString(),
            SessionId = session.SessionId.ToString(),
            LeastStartTimeUnixMs = session.LeastStartTime.ToUnixTimeMilliseconds(),
            GreatestEndTimeUnixMs = session.GreatestEndTime.ToUnixTimeMilliseconds(),
            PagesViewed = session.PagesViewed,
            CartridgesInserted = session.CartridgesInserted,
            ContactAttempts = session.ContactAttempts,
            TotalTimeMs = session.TotalTimeMs,
            SessionExpiresAtUnixMs = session.SessionExpiresAt.ToUnixTimeMilliseconds()
        };

        var message = new SessionDeltaMessage
        {
            Type = "session_delta",
            Key = session.SessionId.ToString(),
            Version = 0,
            Payload = payload
        };

        var result = await _client.EmitAsync(message);
    }
}