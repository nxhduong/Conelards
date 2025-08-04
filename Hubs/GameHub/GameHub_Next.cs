using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

public partial class GameHub : Hub
{
    // TODO
    public bool IsPlayableCard(int color, int number, int action)
    {  return false; }

    public async Task Next(string submittedCards)
    {
        var roomId = Context?.User?.FindFirstValue("CurrentRoomId")!;
        if (roomId is null || !GameState.TryGetValue(roomId, out _))
        {
            await Clients.User(Context?.UserIdentifier!).SendAsync(SignalMessage.ExitRoom);
            return;
        }

        var playStack = JsonSerializer.Deserialize<List<Card>>(submittedCards);
        var cardsToDraw = 0;

        if (GameState[roomId].Players.Count <= 1)
        {
            await Clients.Group(roomId).SendAsync(SignalMessage.GameOver, 
                from participant in GameState[roomId].Spectators
                where participant.Value.Rank > 0
                orderby participant.Value.Rank
                select $"{participant.Key}: {participant.Value.Rank}"
            );
            GameState.Remove(roomId);
            return;
        }

        // Get 1 card from deck when player has no applicable card
        // Player may play newly-drawn card if it's playable
        if (playStack is [] || playStack is null)
        {
            if (GameState[roomId].PenaltyStackCount > 0)
            {
                cardsToDraw = GameState[roomId].PenaltyStackCount;
                GameState[roomId].PenaltyStackCount = 0;
            }

            cardsToDraw += 1;

            if (cardsToDraw <= GameState[roomId].Deck.Count)
            {
                playStack = GameState[roomId].Deck.GetRange(0, cardsToDraw);
                GameState[roomId].Deck.RemoveRange(0, cardsToDraw);
            }
            else
            {
                // TODO
                playStack = GameState[roomId].Deck.GetRange(0, cardsToDraw);
                GameState[roomId].Deck.RemoveRange(0, GameState[roomId].Deck.Count);
            }
        }

        if (
            // All submitted cards match number/action with discard
            playStack.All(card =>
                (card.Number is not null && card.Number == GameState[roomId].Discard.Number)
                || (card.Action is not null && card.Action == GameState[roomId].Discard.Action)
            )
            // All submitted cards match number, one card matches color with discard card
            || (playStack.Any(card => card.Color == GameState[roomId].Discard.Color)
                && playStack.All(card => card.Number == playStack[0].Number)
                && GameState[roomId].PenaltyStackCount == 0)
            // Special cards
            || playStack.All(card => (byte)(card.Action ?? 0) > 1 && card.Action == playStack[0].Action)
        )
        {
            // Pick last card as new discard
            GameState[roomId].Deck.Add(GameState[roomId].Discard);
            GameState[roomId].Discard = playStack[^1];

            switch (playStack[0].Action)
            {
                case CardPower.Draw4:
                    GameState[roomId].PenaltyStackCount += (byte)(4 + playStack.Count);
                    goto case CardPower.Wild;

                case CardPower.Shuffle:
                    List<Card> allCards = [];
                    foreach (var player in GameState[roomId].Players)
                    {
                        allCards.AddRange(player.Value.Cards);
                    }

                    allCards = [.. allCards.OrderBy(i => Randomizer.Next())];

                    foreach (var player in GameState[roomId].Players)
                    {
                        var numberOfCards =
                            (int)Math.Floor((double)allCards.Count /
                            GameState[roomId].Players.Count);
                        player.Value.Cards = allCards.GetRange(0, numberOfCards);
                        allCards.RemoveRange(0, numberOfCards);
                    }

                    GameState[roomId].Deck.AddRange(allCards);
                    goto case CardPower.Wild;

                case CardPower.Wild:
                    playStack.ForEach(card => card.Color = null);
                    break;

                case CardPower.Ban:
                    GameState[roomId].CurrentPlayerIndex += (byte)(1 + playStack.Count);
                    break;

                case CardPower.Reverse:
                    if (playStack.Count % 2 == 1)
                    {
                        GameState[roomId].Players = (Dictionary<string, PlayerProperties>)GameState[roomId].Players.Reverse();
                        GameState[roomId].CurrentPlayerIndex = (byte)(GameState[roomId].Players.Count - GameState[roomId].CurrentPlayerIndex);
                    }
                    break;
            }

            foreach (var card in playStack)
            {                
                // Remove used cards
                // Players can't use cards that they don't have
                if (!GameState[roomId].Players[Context?.User?.Identity?.Name!].Cards.Remove(card))
                {
                    throw new Exception("Invalid card: " + submittedCards + GameState[roomId].Discard);
                }
            }

            // Return cards to deck
            // Should randomize deck after addition?
            GameState[roomId].Deck.AddRange(playStack.SkipLast(1));

            if (GameState[roomId].Players[Context?.User?.Identity?.Name!].Cards.Count == 0)
            {
                // Quit playing
                GameState[roomId].Spectators.TryAdd(
                    Context?.User?.Identity?.Name!,
                    GameState[roomId].Players[Context?.User?.Identity?.Name!]
                );
                GameState[roomId].Players.Remove(Context?.User?.Identity?.Name!);

                GameState[roomId].Players[Context?.User?.Identity?.Name!].Rank = 
                    (byte)(GameState[roomId].Players.Select(player => player.Value.Rank).Max() + 1);
            }
        }
        else
        {
            // Give newly-retrieved card(s) to player if it is not playable
            if (cardsToDraw > 0)
            {
                GameState[roomId].Players[Context?.User?.Identity?.Name!].Cards.AddRange(playStack);
            }
            else
            {
                throw new Exception("Invalid card: " + submittedCards + " " + GameState[roomId].Discard);
            }
        }

        await Clients.Users(
            GameState[roomId].Players.ElementAt(GameState[roomId].CurrentPlayerIndex).Key
        ).SendAsync(SignalMessage.YourTurn);
        await Clients.Group(roomId).SendAsync(SignalMessage.UpdateState, GameState[roomId].ToString());
    }
}