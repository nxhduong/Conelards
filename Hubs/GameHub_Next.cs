using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs;

public partial class GameHub : Hub
{
    public async Task Next(string card)
    {
        // TODO: check card
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId");

        Tables[roomId!].CurrentPlayerIndex += 1;
        await Clients.Users(
            Tables[roomId!].Participants.GetKeyAtIndex(Tables[roomId!].CurrentPlayerIndex)
        ).SendAsync("YourTurn");
    }
}