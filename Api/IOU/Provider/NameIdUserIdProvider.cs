using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Api.IOU.Providers;

public class NameIdUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}