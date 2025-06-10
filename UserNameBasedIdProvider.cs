using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class UserNameBasedIdProvider : IUserIdProvider
{
    public virtual string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
    }
}