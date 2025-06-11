using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs;

public partial class GameHub : Hub
{
    public async Task Next(string cards)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId");
        var stack = JsonSerializer.Deserialize<IList<Card>>(cards);

        if (stack is [] || stack is null)
        {
            stack = [Tables[roomId!].Deck[0]];
        }

        if (
            stack.All(card =>
                (card.Number is not null && card.Number == Tables[roomId!].Discard.Number)
                || (card.Action is not null && card.Action == Tables[roomId!].Discard.Action)
            )
            || (stack.Any(card => card.Color == Tables[roomId!].Discard.Color)
                && stack.All(card => card.Number == stack[0].Number))
            || stack.All(card => new string[] {"Wild", "Shuffle", "Add4"}.Contains(card.Action))
        )
        {
            foreach (var card in stack)
            {
                Tables[roomId!].Deck.Add(Tables[roomId!].Discard);
                Tables[roomId!].Discard = card;
                if (!Tables[roomId!].Players[Context?.User?.Identity?.Name!].Cards.Remove(card))
                {
                    throw new Exception("Invalid card: " + cards + Tables[roomId!].Discard);
                }
            }

            switch (stack[0].Action)
            {
                case "Ban":
                    Tables[roomId!].CurrentPlayerIndex += (byte)(1 + stack.Count);
                    break;
                case "Rev":
                    Tables[roomId!].Players = (Dictionary<string, PlayerProperties>)Tables[roomId!].Players.Reverse();
                    Tables[roomId!].CurrentPlayerIndex = (byte)(Tables[roomId!].Players.Count - Tables[roomId!].CurrentPlayerIndex);
                    break;
                case "Shuffle":
                    List<Card> total = [];
                    foreach (var player in Tables[roomId!].Players)
                    {
                        total.AddRange(player.Value.Cards);
                    }

                    foreach (var player in Tables[roomId!].Players)
                    {
                        var numberOfCards =
                            (int)Math.Floor((double)total.Count /
                            Tables[roomId!].Players.Count());
                        player.Value.Cards = total.GetRange(0, numberOfCards);
                        total.RemoveRange(0, numberOfCards);
                    }

                    Tables[roomId!].Deck.AddRange(total);
                    break;
            }
        }
        else
        {
            if (stack.Count == 1)
            {
                Tables[roomId!].Players[Context?.User?.Identity?.Name!].Cards.Add(stack[0]);
            }
            else
            {
                throw new Exception("Invalid card: " + cards + Tables[roomId!].Discard);
            }
        }

        await Clients.Users(
            Tables[roomId!].Players.ElementAt(Tables[roomId!].CurrentPlayerIndex).Key
        ).SendAsync("YourTurn");
    }
}