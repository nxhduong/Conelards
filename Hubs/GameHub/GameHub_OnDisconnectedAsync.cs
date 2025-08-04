using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId")!;

        if (roomId is not null)
        {
            await new Task(() => Tables[roomId].Players.Remove(
                Context?.User?.Identity?.Name!
            ));

            if (Tables[roomId].Players.Count + Tables[roomId].Spectators.Count == 0)
            {
                Tables.Remove(roomId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}