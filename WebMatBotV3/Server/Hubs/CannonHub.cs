using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Server.Services;
using WebMatBotV3.Shared;

namespace WebMatBotV3.Server.Hubs
{
    public class CannonHub : Hub<ICannonClient>
    {
        private readonly CannonService cService;
        private IList<string> connectedIds { get; set; } = new List<string>();

        public CannonHub(CannonService _cService)
        {
            cService = _cService;
        }

        public async Task Winner(string user, string args)
        {
            //avisa que o cidadão acertou e recebe os pontos que ele possui
            await cService.AddPoint(user, args);

            await cService.BonusInvitations(user);

            await cService.SendRanking();
        }
        public async Task CloseStore() =>
            await WebMatBot.Games.Cannon_Store.CloseMarketCommand();

        public async Task Init()
        {
            await WebMatBot.Games.Cannon_Store.OpenMarketCommand();
            await cService.SendRanking();
            await cService.SendJackPot();
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected " + Context.ConnectionId);
            connectedIds.Add(Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {

            Console.WriteLine("Disconnected " + Context.ConnectionId);
            connectedIds.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public bool alala() => true;
    }
}
