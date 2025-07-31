using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs;

[Authorize]
public partial class GameHub : Hub
{
    readonly Dictionary<string, Table> Tables = [];

    readonly string[] ColorfulCards = ["Wild", "Shuffle", "Add4"];
}

class PlayerProperties
{
    public bool IsReady = false;

    public List<Card> Cards = [];

    public byte Rank = 0;
}

class Table
{
    public Dictionary<string, PlayerProperties> Players = [];
    public HashSet<string> Spectators = [];

    [JsonIgnore]
    public List<Card> Deck = [new Card(Color.Red, 0, null), new Card(Color.Red, 1, null), new Card(Color.Red, 2, null), new Card(Color.Red, 3, null), new Card(Color.Red, 4, null), new Card(Color.Red, 5, null), new Card(Color.Red, 6, null), new Card(Color.Red, 7, null), new Card(Color.Red, 8, null), new Card(Color.Red, 9, null), new Card(Color.Red, null, "Ban"), new Card(Color.Red, null, "Rev"), new Card(Color.Red, null, "Add2"), new Card(Color.Green, 0, null), new Card(Color.Green, 1, null), new Card(Color.Green, 2, null), new Card(Color.Green, 3, null), new Card(Color.Green, 4, null), new Card(Color.Green, 5, null), new Card(Color.Green, 6, null), new Card(Color.Green, 7, null), new Card(Color.Green, 8, null), new Card(Color.Green, 9, null), new Card(Color.Green, null, "Ban"), new Card(Color.Green, null, "Rev"), new Card(Color.Green, null, "Add2"), new Card(Color.Blue, 0, null), new Card(Color.Blue, 1, null), new Card(Color.Blue, 2, null), new Card(Color.Blue, 3, null), new Card(Color.Blue, 4, null), new Card(Color.Blue, 5, null), new Card(Color.Blue, 6, null), new Card(Color.Blue, 7, null), new Card(Color.Blue, 8, null), new Card(Color.Blue, 9, null), new Card(Color.Blue, null, "Ban"), new Card(Color.Blue, null, "Rev"), new Card(Color.Blue, null, "Add2"), new Card(Color.Yellow, 0, null), new Card(Color.Yellow, 1, null), new Card(Color.Yellow, 2, null), new Card(Color.Yellow, 3, null), new Card(Color.Yellow, 4, null), new Card(Color.Yellow, 5, null), new Card(Color.Yellow, 6, null), new Card(Color.Yellow, 7, null), new Card(Color.Yellow, 8, null), new Card(Color.Yellow, 9, null), new Card(Color.Yellow, null, "Ban"), new Card(Color.Yellow, null, "Rev"), new Card(Color.Yellow, null, "Add2"), new Card(Color.Black, 0, null), new Card(Color.Black, 1, null), new Card(Color.Black, 2, null), new Card(Color.Black, 3, null), new Card(Color.Black, 4, null), new Card(Color.Black, 5, null), new Card(Color.Black, 6, null), new Card(Color.Black, 7, null), new Card(Color.Black, 8, null), new Card(Color.Black, 9, null), new Card(Color.Black, null, "Ban"), new Card(Color.Black, null, "Rev"), new Card(Color.Black, null, "Add2"), new Card(null, null, "Add4"), new Card(null, null, "Add4"), new Card(null, null, "Add4"), new Card(null, null, "Add4"), new Card(null, null, "Add4"), new Card(null, null, "Wild"), new Card(null, null, "Wild"), new Card(null, null, "Wild"), new Card(null, null, "Wild"), new Card(null, null, "Wild"), new Card(null, null, "Shuffle")];
    public Card Discard = default!;

    private byte _currentPlayerIndex = 0;
    public byte CurrentPlayerIndex
    {
        get => _currentPlayerIndex;
        set
        {
            if (value >= 0 && value < Deck.Count)
            {
                _currentPlayerIndex = value;
            }
            else if (value >= Deck.Count)
            {
                _currentPlayerIndex = 0;
            }
        }
    }

    public Table()
    {
        var randomizer = new Random();
        Deck = [.. Deck.OrderBy(i => randomizer.Next())];

        byte i = 0;
        do
        {
            Discard = Deck[i];
            i += 1;
        }
        while (Discard.Number is null);

        Deck.RemoveAt(i);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

record Card(Color? Color = null, byte? Number = null, string? Action = null);

enum Color
{
    Red,
    Green,
    Blue,
    Yellow,
    Black
}

struct ActionMessage
{
    public const string UpdateStatus = "STAT";
    public const string YourTurn = "YUTN";
}