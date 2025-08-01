using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public async Task Ready(string role)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId");

        // Modify role for the participant (max 10 players per room)
        if (role == "Player" && Tables[roomId!].Players.Count < 10)
        {
            Tables[roomId!].Players.TryAdd(Context?.User?.Identity?.Name!, new PlayerProperties());
            Tables[roomId!].Spectators.Remove(Context?.User?.Identity?.Name!);
        }
        else
        {
            Tables[roomId!].Players.Remove(Context?.User?.Identity?.Name!);
            Tables[roomId!].Spectators.Add(Context?.User?.Identity?.Name!);
        }

        // When everyone ready, allocate cards and notify first player
        if (
            Tables[roomId!]
                .Players
                .All(participant => participant.Value.IsReady)
        )
        {
            foreach (
                var player in Tables[roomId!]
                    .Players
            )
            {
                player.Value.Cards = Tables[roomId!].Deck.GetRange(0, 5);
                Tables[roomId!].Deck.RemoveRange(0, 5);
            }

            await Clients.Users(
                Tables[roomId!].Players.ElementAt(Tables[roomId!].CurrentPlayerIndex).Key
            ).SendAsync(ActionMessage.YourTurn);
        }

        await Clients.Group(roomId!).SendAsync(ActionMessage.UpdateStatus, Tables[roomId!].ToString());
    }
}