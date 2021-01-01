using F23.StringSimilarity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMatBot;
using WebMatBot.Core;
using WebMatBot.Lights;
using WebMatBotV3.Server.Hubs;
using WebMatBotV3.Shared;
using WebMatBotV3.Shared.Entity;
using static WebMatBot.Games.Cannon_Store;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBotV3.Server.Services
{
    public class WebMatBotService : BackgroundService
    {
        private readonly IHubContext<CannonHub, ICannonClient> cannonHub;
        private readonly IServiceProvider providerService;
        private readonly ContextCache contextCache;

        public WebMatBotService(IHubContext<CannonHub, ICannonClient> _cannonHub, IServiceProvider _providerService, ContextCache _contextCache)
        {
            cannonHub = _cannonHub;
            providerService = _providerService;
            contextCache = _contextCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeCache();
            await WebMatBot.Program.Start();
            WebMatBot.Games.Cannon.Shot = Shot;
            WebMatBot.Games.Cannon.Spray = Spray;
            WebMatBot.Games.Cannon.MoveTarget = cannonHub.Clients.All.MoveTarget;
            WebMatBot.Games.Cannon.CheckMyBallsFromServer = GetPlayerBalls;
            WebMatBot.Games.Cannon.NewGame = InitClient;
            WebMatBot.Games.Cannon.SaveRecord = SaveRecord;
            WebMatBot.Games.Cannon.CanShot = CanShot;
            WebMatBot.Games.Cannon.CanSubRedemption = CanSubRedemption;
            WebMatBot.Games.Cannon.AddInvitation = AddInvitation;
            WebMatBot.Games.Cannon.Rank = Rank;
            WebMatBot.Games.Cannon.GenerateList = GenerateList;
            WebMatBot.Games.Cannon.TransferName = TransferName;
            WebMatBot.Games.Cannon_Store.OpenMarket = cannonHub.Clients.All.OpenMarket;
            WebMatBot.Games.Cannon_Store.BuyBall = BuyBall;
            WebMatBot.Games.Cannon_Store.EquipUserBall = EquipUserBall;
            WebMatBot.Games.Cannon_Store.GetUserSkins = GetUserSkins;
            WebMatBot.Games.Cannon_Store.GetBallsToSale = GetBallsToSale;
            WebMatBot.Games.Cannon_Store.CanSubGift = CanSubGift;
            WebMatBot.Games.Cannon_Store.SubEnter = SubEnter;
            WebMatBot.IrcEngine.UserInteraction = UserInteraction;



            //auto update cache by time
            while (true)
            {
                await Task.Delay(new TimeSpan(0, 1, 0)); //atualiza o bd a cada 1 minuto
                await InitializeCache();
            }



        }

        public static async Task StopAsync() => await WebMatBot.Lights.Light.Stop();

        public async Task InitClient()
        {
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                { 
                    await cannonService.SendRanking();
                    await cannonService.SendJackPot();
                }
            }
        }

        public async Task Shot(float angle, float power, string user, string cmd)
        {
            TypeBalls ball;
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    ball = cannonService.GetUserEquipedBall(user);
            }

            await cannonHub.Clients.All.Shot(angle, power, user, ball);

            //save os records
            await SaveRecord(
                new Records()
                {
                    Action = Records.ActionType.Shot,
                    Arguments = cmd,
                    Date = DateTime.Now,
                    Username = user,
                });
        }
        public async Task Spray(float angle, float power, string user, string cmd)
        {
            TypeBalls ball;
            int qtdBalls;
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                {
                    ball = cannonService.GetUserEquipedBall(user);
                    qtdBalls = cannonService.GetPlayerBalls(user);
                }
            }

            await cannonHub.Clients.All.Spray(angle, power, user, ball, (qtdBalls < 5) ? qtdBalls : 5);

            await SaveRecord(
            new Records()
            {
                Action = Records.ActionType.Spray,
                Arguments = cmd,
                Date = DateTime.Now,
                Username = user,
            });
        }

        public int GetPlayerBalls(string user)
        {
            int Result = 0;
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    Result = cannonService.GetPlayerBalls(user);
            }

            return Result;
        }

        public async Task SaveRecord(Records record)
        {
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    await cannonService.SaveRecord(record);
            }
        }

        public bool CanShot(string user)
        {
            //verificar aqui se há clientes conectados no hub do signal R

            bool result = false;
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = cannonService.CanShot(user);
            }

            return result;
        }
        public Nullable<KeyValuePair<int, Scores>> Rank(string user)
        {
            //verificar aqui se há clientes conectados no hub do signal R

            Nullable<KeyValuePair<int, Scores>> result;

            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = cannonService.GetUserRank(user);
            }

            return result;
        }

        public async Task<bool> CanSubRedemption(string user)
        {
            bool result = false;
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = await cannonService.CanSubRedemption(user);
            }

            return result;
        }

        public async Task<bool> CanSubGift(string user)
        {
            bool result = false;
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = await cannonService.CanSubGift(user);
            }

            return result;
        }

        public async Task SubEnter(string user)
        {
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    await cannonService.SubEnter(user);
            }
        }

        public async Task<string> AddInvitation(string host, string invited)
        {
            string result = null;
            //maneira de instanciar um serviço scoped dentro de um singleton
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = await cannonService.AddInvitation(host, invited);
            }

            return result;
        }

        public async Task GenerateList()
        {
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    await cannonService.GenerateList();
            }
        }

        public async Task<IEnumerable<Balls>> GetBallsToSale()
        {
            IEnumerable<Balls> list = null;
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    list = await cannonService.GetBallsToSale();
            }

            return list;
        }

        public async Task<string> BuyBall(string user, float bits, List<Balls> ballsOnSale, TypePayment payment)
        {
            string result = "Erro...";
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = await cannonService.BuyBall(user, bits, ballsOnSale, payment);
            }

            return result;
        }

        public TypeBalls[] GetUserSkins(string user)
        {
            TypeBalls[] result = null;
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = cannonService.GetUserSkins(user);
            }

            return result;
        }

        public async Task<string> EquipUserBall(string user, TypeBalls type)
        {
            string result = "Erro...";
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    result = await cannonService.EquipUserBall(user, type);
            }

            return result;
        }

        public async Task TransferName(string user)
        {
            using (var scope = providerService.CreateScope())
            {
                using (var cannonService = scope.ServiceProvider.GetRequiredService<CannonService>())
                    await cannonService.TransferName(user);
            }
        }

        public async Task UserInteraction(string user, int id)
        {
            var cachedList = contextCache.GetUsernames();
            var userCached = cachedList.FirstOrDefault(q => q.Username == user && q.User_id == id);

            if (userCached == null)
            {
                using (var scope = providerService.CreateScope())
                {
                    using (var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                    {
                        var newUserInteration = new Usernames()
                        {
                            User_id = id,
                            Username = user
                        };

                        dataContext.Usernames.Add(newUserInteration);
                        await dataContext.SaveChangesAsync();

                        //atualiza o cache
                        contextCache.UpdateUsernames(await dataContext.Usernames.ToListAsync());

                    }
                }
            }

        }

        public async Task InitializeCache()
        {
            using (var scope = providerService.CreateScope())
            {
                using (var dataService = scope.ServiceProvider.GetRequiredService<DataContext>())
                    await contextCache.Init(dataService);
            }
        }
    }
}
