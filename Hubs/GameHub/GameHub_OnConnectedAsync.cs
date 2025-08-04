using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId")!;
        if (roomId is null || !Tables.TryGetValue(roomId, out _))
        {
            await Clients.User(Context?.UserIdentifier!).SendAsync(SignalMessage.ExitRoom);
            return;
        }

        // if (!Tables.ContainsKey(roomId)) Tables.Add(roomId, new Table());

        await Groups.AddToGroupAsync(Context!.ConnectionId, roomId);
        Tables[roomId].Players.Add(Context?.User?.Identity?.Name!, new PlayerProperties());
        await Clients.Group(roomId).SendAsync(SignalMessage.UpdateState, Tables[roomId].ToString());

        await base.OnConnectedAsync();
    }
}