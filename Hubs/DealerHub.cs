using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colards.Hubs
{
    [Authorize]
    public class DealerHub : Hub
    {
        readonly SortedSet<Player> Members = [];
        public async Task Init(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            Members.Add(new Player
            {
                Username = Context.User.FindFirstValue(ClaimTypes.Name),
                IsReady = false,
                IsSpectating = false,
                Cards = new List<string> { }
            });
            await Clients.Group(roomId).SendAsync("Initialized", Members.ToString());
        }

        public async Task Ready()
        { }

        public async Task Next(string card)
        {
            // TODO:
            await Clients.All.SendAsync("YourTurn", card);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await new Task(() => Members.RemoveWhere((player) => player.Username == Context.User?.FindFirstValue(ClaimTypes.Name)));
        }
    }

    class Player
    {
        public string Username;
        public bool IsReady;
        public bool IsSpectating;
        public List<string> Cards;
    }
}