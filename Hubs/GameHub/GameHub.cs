using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Conelards.Hubs.GameHub;

[Authorize]
public partial class GameHub : Hub
{
    readonly Dictionary<string, Table> Tables = [];
    public new static readonly Random Randomizer;
}

class PlayerProperties
{
    public bool IsReady = false;
    public List<Card> Cards = [];
    public byte Rank = 0;
}

struct ActionMessage
{
    public const string UpdateStatus = "STAT";
    public const string YourTurn = "YUTN";
}