using Analytics.Live;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;

namespace Portfolio.Infrastructure.Analytics.InterappTransport;

public class SessionDeltaGrpcService : SessionDeltaService.SessionDeltaServiceBase
{
    private readonly IHubContext<AnalyticsHub> _hub;

    public SessionDeltaGrpcService(IHubContext<AnalyticsHub> hub)
    {
        _hub = hub;
    }

    public override async Task<EmitAck> Emit(SessionDeltaMessage request, ServerCallContext context)
    {
        var sessionDelta = SessionDeltaPayload.Parser.ParseFrom(request.Payload.ToByteArray());
        await AnalyticsHub.BroadcastSession(_hub, sessionDelta, context.CancellationToken);

        return new EmitAck { Accepted = true };
    }
}