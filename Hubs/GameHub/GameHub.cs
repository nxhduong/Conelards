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

struct SignalMessage
{
    public const string UpdateState = "UpdateState";
    public const string YourTurn = "MyTurn";
    public const string ExitRoom = "ExitRoom";
    public const string GameOver = "GameOver";
}