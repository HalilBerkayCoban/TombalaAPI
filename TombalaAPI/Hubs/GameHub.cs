using Microsoft.AspNetCore.SignalR;

namespace TombalaAPI.Hubs
{
    public class GameHub : Hub
    {
        public async Task NewGame()
        {
            await Clients.All.SendAsync("NewGame");
        }

        public async Task GameStatusChanged()
        {
            await Clients.All.SendAsync("GameStatusChanged");
        }

        public async Task NewDraw(int number)
        {
            await Clients.All.SendAsync("NewDraw", number);
        }

        public async Task NewUserAction()
        {
            await Clients.All.SendAsync("NewUserAction");
        }

        public async Task NewCard()
        {
            await Clients.All.SendAsync("NewCard");
        }

        public async Task NewParticipant(string gameId)
        {
            await Clients.All.SendAsync("NewParticipant", gameId);
        }
    }
} 