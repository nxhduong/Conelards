using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs;

public partial class GameHub : Hub
{
    public async Task Ready(string role)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId");
        var randomizer = new Random();

        Tables[roomId!].Participants[Context?.User?.Identity?.Name!].IsSpectating = role == "Spectator";

        //TODO: limit players to 10
        if (
            Tables[roomId!]
                .Participants
                .All(participant => participant.Value.IsSpectating is not null)
        )
        {
            foreach (
                var player in Tables[roomId!]
                    .Participants
                    .Where(participant => participant.Value.IsSpectating ?? false)
            )
            {
                player.Value.Cards = Tables[roomId!].Deck.GetRange(0, 5);
                Tables[roomId!].Deck.RemoveRange(0, 5);
            }

            await Clients.Users(
                Tables[roomId!].Participants.GetKeyAtIndex(Tables[roomId!].CurrentPlayerIndex)
            ).SendAsync("YourTurn");
        }

        await Clients.Group(roomId!).SendAsync("UpdateStatus", Tables[roomId!].ToString());
    }
}