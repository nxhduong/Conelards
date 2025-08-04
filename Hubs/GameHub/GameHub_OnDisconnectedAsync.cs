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
            await new Task(() => GameState[roomId].Players.Remove(
                Context?.User?.Identity?.Name!
            ));

            if (GameState[roomId].Players.Count + GameState[roomId].Spectators.Count == 0)
            {
                GameState.Remove(roomId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}