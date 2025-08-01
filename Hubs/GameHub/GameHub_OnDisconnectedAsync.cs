using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId");

        await new Task(() => Tables[roomId!].Players.Remove(
            Context?.User?.Identity?.Name!
        ));

        await base.OnDisconnectedAsync(exception);
    }
}