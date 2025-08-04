using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public async Task Ready(string role)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId")!;
        if (roomId is null || !Tables.TryGetValue(roomId, out _))
        {
            await Clients.User(Context?.UserIdentifier!).SendAsync(SignalMessage.ExitRoom);
            return;
        }

        // Modify role for the participant (max 10 players per room)
        if (role == "Player" && Tables[roomId].Players.Count < 10)
        {
            Tables[roomId].Players.TryAdd(Context?.User?.Identity?.Name!, new PlayerProperties());
            Tables[roomId].Spectators.Remove(Context?.User?.Identity?.Name!);
        }
        else
        {
            Tables[roomId].Spectators.TryAdd(
                Context?.User?.Identity?.Name!, 
                Tables[roomId].Players[Context?.User?.Identity?.Name!]
            );
            Tables[roomId].Players.Remove(Context?.User?.Identity?.Name!);            
        }

        // When everyone (> 1) ready, allocate cards and notify first player
        if (
            Tables[roomId].Players.Count > 1 &&
            Tables[roomId]
                .Players
                .All(participant => participant.Value.IsReady)
        )
        {
            foreach (
                var player in Tables[roomId]
                    .Players
            )
            {
                player.Value.Cards = Tables[roomId].Deck.GetRange(0, 5);
                Tables[roomId].Deck.RemoveRange(0, 5);
            }

            await Clients.Users(
                Tables[roomId].Players.ElementAt(Tables[roomId].CurrentPlayerIndex).Key
            ).SendAsync(SignalMessage.YourTurn);
        }

        await Clients.Group(roomId).SendAsync(SignalMessage.UpdateState, Tables[roomId].ToString());
    }
}