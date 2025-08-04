using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    public async Task Ready(string role)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId")!;
        if (roomId is null || !GameState.TryGetValue(roomId, out _))
        {
            await Clients.User(Context?.UserIdentifier!).SendAsync(SignalMessage.ExitRoom);
            return;
        }

        // Modify role for the participant (max 10 players per room)
        if (role == "Player" && GameState[roomId].Players.Count < 12)
        {
            GameState[roomId].Players.TryAdd(Context?.User?.Identity?.Name!, new PlayerProperties());
            GameState[roomId].Spectators.Remove(Context?.User?.Identity?.Name!);
        }
        else
        {
            GameState[roomId].Spectators.TryAdd(
                Context?.User?.Identity?.Name!, 
                GameState[roomId].Players[Context?.User?.Identity?.Name!]
            );
            GameState[roomId].Players.Remove(Context?.User?.Identity?.Name!);            
        }

        // When everyone (> 1) ready, allocate cards and notify first player
        if (
            GameState[roomId].Players.Count > 1 &&
            GameState[roomId]
                .Players
                .All(player => player.Value.IsReady)
        )
        {
            foreach (
                var player in GameState[roomId]
                    .Players
            )
            {
                player.Value.Cards = GameState[roomId].Deck.GetRange(0, 5);
                GameState[roomId].Deck.RemoveRange(0, 5);
            }

            await Clients.Users(
                GameState[roomId].Players.ElementAt(GameState[roomId].CurrentPlayerIndex).Key
            ).SendAsync(SignalMessage.YourTurn);
        }

        await Clients.Group(roomId).SendAsync(SignalMessage.UpdateState, GameState[roomId].ToString());
    }
}