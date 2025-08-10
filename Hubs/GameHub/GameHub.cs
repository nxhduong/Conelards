using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace Conelards.Hubs.GameHub;

[Authorize]
public partial class GameHub : Hub
{
    readonly Dictionary<string, Table> GameState = [];
    public new static readonly Random Randomizer;
}

class PlayerProperties
{
    [JsonInclude]
    public byte CardCount = 0;
    [JsonIgnore]
    private List<Card> _cards = [];
    [JsonIgnore]
    public List<Card> Cards
    {
        get {
            CardCount = (byte)_cards.Count;
            return _cards; 
        }
        set { 
            _cards = value;
            CardCount = (byte)_cards.Count;
        }
    }

    [JsonInclude]
    public bool IsReady = false;
    [JsonInclude]
    public byte Rank = 0;
}

struct SignalMessage
{
    public const string UpdateState = "UpdateState";
    public const string YourTurn = "MyTurn";
    public const string ExitRoom = "ExitRoom";
    public const string GameOver = "GameOver";
}