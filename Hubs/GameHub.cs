using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs;

[Authorize]
public partial class GameHub : Hub
{
    readonly Dictionary<string, Table> Tables = [];
}

class PlayerProperties
{
    public bool? IsSpectating = null;

    public List<string> Cards = [];

    public byte Rank = 0;
}

class Table
{
    public SortedList<string, PlayerProperties> Participants = [];

    [JsonIgnore]
    public List<string> Deck = ["Red_0", "Red_1", "Red_2", "Red_3", "Red_4", "Red_5", "Red_6", "Red_7", "Red_8", "Red_9", "Red_Ban", "Red_Rev", "Red_Add2", "Gre_0", "Gre_1", "Gre_2", "Gre_3", "Gre_4", "Gre_5", "Gre_6", "Gre_7", "Gre_8", "Gre_9", "Gre_Ban", "Gre_Rev", "Gre_Add2", "Blu_0", "Blu_1", "Blu_2", "Blu_3", "Blu_4", "Blu_5", "Blu_6", "Blu_7", "Blu_8", "Blu_9", "Blu_Ban", "Blu_Rev", "Blu_Add2", "Yel_0", "Yel_1", "Yel_2", "Yel_3", "Yel_4", "Yel_5", "Yel_6", "Yel_7", "Yel_8", "Yel_9", "Yel_Ban", "Yel_Rev", "Yel_Add2", "Bla_0", "Bla_1", "Bla_2", "Bla_3", "Bla_4", "Bla_5", "Bla_6", "Bla_7", "Bla_8", "Bla_9", "Bla_Ban", "Bla_Rev", "Bla_Add2", "Add4_", "Add4_", "Add4_", "Add4_", "Add4_", "Wild_", "Wild_", "Wild_", "Wild_", "Wild_", "Shuffle_", "Shuffle_", "Shuffle_", "Shuffle_", "Shuffle_"];

    public string Discard = default!;

    private byte _currentPlayerIndex;
    public byte CurrentPlayerIndex
    {
        get;
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
    } = 0;

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
        while (!char.IsDigit(Discard.Split('_')[1][0]));

        Deck.RemoveAt(i);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}