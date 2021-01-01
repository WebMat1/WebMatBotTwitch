using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Server.Hubs;
using WebMatBotV3.Shared;
using WebMatBotV3.Shared.Entity;
using static WebMatBot.Games.Cannon_Store;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBotV3.Server.Services
{
    public class CannonService : IDisposable
    {
        private readonly DataContext dataContext;
        private readonly IHubContext<CannonHub, ICannonClient> cannonHub;
        private readonly ContextCache contextCache;

        public CannonService(DataContext _dataContext, IHubContext<CannonHub, ICannonClient> _cannonHub, ContextCache _contextCache)
        {
            dataContext = _dataContext;
            cannonHub = _cannonHub;
            contextCache = _contextCache;
        }


        #region Writing to DB

        //ALTERAR - DONE
        public async Task<string> EquipUserBall(string user, TypeBalls type)
        {
            //verificar se o usuario já possui a bola... se sim ok
            if (!DoesUserHaveSkin(user, type))
                return "@" + user + "... Você não possui esta skin..." + type.ToString();

            #region Atualiza no banco de dados
            var userInventory = await dataContext.Inventories.Include(e => e.Ball).Where(q => q.Username == user).ToListAsync();

            //atualiza para apenas a bola solicitada seja isUsing
            foreach (var item in userInventory)
            {
                if (item.Ball.Type == type) item.IsUsing = true;
                else item.IsUsing = false;
            }

            await dataContext.SaveChangesAsync();
            contextCache.UpdateInventories(await dataContext.Inventories.Include(q=>q.Ball).ToListAsync());
            #endregion

            //TODO - atualizar no cache

            return "@" + user + "... Você adiquiriu a linda Skin " + type.ToString() + "... Nós já a ativamos... para checar todas as suas skins use !MySkins...";

        }

        //ALTERAR - DONE
        public async Task<Scores> AddPoint(string user, string args)
        {
            //pega do cache
            //pega a season aberta desse jogo
            var season = await GetCurrentSeason();

            // tenta pegar o usuario que já está no leaderboard
            var userInLB = season.Scores.FirstOrDefault(q => q.Username.ToLower() == user.ToLower());

            //o usuario não marcou pontos pra essa season
            if (userInLB == null)
            {
                //cria o registro pra add no banco
                var newScore = new Scores()
                {
                    Points = 1,
                    Season = season,
                    Username = user
                };

                var newPointRecord = new Points()
                {
                    Score = newScore,
                    Date = DateTime.Now
                };

                dataContext.Scores.Add(newScore);
                dataContext.Points.Add(newPointRecord);

            }
            //o usuario já marcou pontos
            else
            {
                var newPointRecord = new Points()
                {
                    Score = userInLB,
                    Date = DateTime.Now
                };

                userInLB.Points += 1;

                dataContext.Points.Add(newPointRecord);

            }

            //salva todas alterações
            await dataContext.SaveChangesAsync();
            contextCache.UpdateSeason(season);

            //avisa no chat qual a pontuação do cidadão que acertou
            await WebMatBot.Games.Cannon.SendMessageToWinner(user, season.Scores.FirstOrDefault(q => q.Username.ToLower() == user.ToLower()).Points, args);

            //atualizar leaderboard NO CLIENT
            

            return userInLB;
        }


        public async Task SaveRecord(Records record)
        {
            //pega a season aberta desse jogo
            var season = await GetCurrentSeason();

            //atualizando o impacto do record no resource
            var ballsUsed = await RecordResource(season, record);

            //atualizando o impacto do record no score
            if (record.Action == Records.ActionType.Shot || record.Action == Records.ActionType.Spray) await RecordScore(season, record, ballsUsed);
        }

        //ALTERAR - DONE
        private async Task RecordScore(Seasons season, Records record, int ballsUsed)
        {
            // tenta pegar o usuario que já está no leaderboard
            var userInLB = season.Scores.FirstOrDefault(q => q.Username.ToLower() == record.Username.ToLower());

            //o usuario não marcou pontos pra essa season
            if (userInLB == null)
            {
                //cria o registro pra add no banco
                var newScore = new Scores()
                {
                    Points = 0,
                    Season = season,
                    Username = record.Username,
                    Tries = ballsUsed,
                };

                dataContext.Scores.Add(newScore);

            }
            else
            {
                userInLB.Tries += ballsUsed;
            }

            if (record.RecordsID == 0) dataContext.Records.Add(record);
            await dataContext.SaveChangesAsync();

            //ATUALIZA CAHCE
            contextCache.UpdateSeason(season);

            //atualizar leaderboard
            //await SendRanking();
        }

        //ATUALIZAR CACHE - DONE
        private async Task<int> RecordResource(Seasons season, Records record)
        {
            int ballsShotted = 0;

            // tenta pegar o usuario que já está no leaderboard
            var userInLB = season.Resources.FirstOrDefault(q => q.Username.ToLower() == record.Username.ToLower());

            //o usuario não marcou pontos pra essa season
            if (userInLB == null)
            {
                if (record.Action == Records.ActionType.Shot || record.Action == Records.ActionType.Spray) throw new Exception(record.Username + "... Não pode atirar sem ter balas a consumir... erro de dev...");
                else if (record.Action == Records.ActionType.Buy || record.Action == Records.ActionType.SubRedemption)
                {

                    #region Resource
                    //cria o registro pra add no banco
                    var newResource = new Resources()
                    {
                        Balls = int.Parse(record.Arguments.Split(" ")[0]),
                        Season = season,
                        Username = record.Username,
                    };

                    //vincula o record com o resource
                    record.Resource = newResource;

                    dataContext.Resources.Add(newResource);

                    #endregion

                    #region Season Value
                    season.JackPot += float.Parse(record.Arguments.Split(" ")[1]);
                    #endregion
                }
            }
            else
            {

                if (record.Action == Records.ActionType.Shot || record.Action == Records.ActionType.Spray)
                {
                    if (userInLB.Balls <= 0)
                        return 0;
                    else
                    {
                        if (record.Action == Records.ActionType.Shot)
                        {
                            userInLB.Balls--;
                            ballsShotted = 1;
                        }
                        else if (record.Action == Records.ActionType.Spray)
                        {
                            ballsShotted = (userInLB.Balls <= 5) ? userInLB.Balls : 5;
                            userInLB.Balls = (userInLB.Balls <= 5) ? 0 : userInLB.Balls - 5;
                        }

                    }
                }
                else if (record.Action == Records.ActionType.Buy || record.Action == Records.ActionType.SubRedemption)
                {
                    //adiciona bolas para o jogador
                    userInLB.Balls += int.Parse(record.Arguments.Split(" ")[0]);

                    #region Season Value
                    season.JackPot += float.Parse(record.Arguments.Split(" ")[1]);
                    #endregion                            
                }

                //vincula o record com o resource
                record.Resource = userInLB;
            }

            if (record.RecordsID == 0) dataContext.Records.Add(record);

            await dataContext.SaveChangesAsync();

            //ATUALIZA CACHE
            contextCache.UpdateSeason(season);

            //atualiza o client do jogo
            if (record.Action == Records.ActionType.Buy) await SendJackPot();

            return ballsShotted;
        }

        //ALTERAR => toda vez que comprar irá equipar... logo faça cache no equipar
        public async Task<string> BuyBall(string user, float value, List<Balls> ballsOnSale, TypePayment payment)
        {
            var inventory = await dataContext.Inventories.Include(q => q.Ball).Where(q => q.Username.ToLower() == user.ToLower()).Select(e => e.Ball.Type).ToListAsync();

            IList<TypeBalls> typeToBuy = new List<TypeBalls>();

            switch (payment)
            {
                case TypePayment.bits:
                    ballsOnSale = ballsOnSale.OrderByDescending(q => q.BitValue).ToList();
                    break;
                case TypePayment.donation:
                    ballsOnSale = ballsOnSale.OrderByDescending(q => q.DonationValue).ToList();
                    break;
            }

            int index = 0;


            for (; index < ballsOnSale.Count; index++)
            {
                switch (payment)
                {
                    case TypePayment.bits:
                        if (ballsOnSale[index].BitValue <= value && !inventory.Contains(ballsOnSale[index].Type))
                        {
                            typeToBuy.Add(ballsOnSale[index].Type);
                            value -= ballsOnSale[index].BitValue;
                        }
                        break;

                    case TypePayment.donation:
                        if (ballsOnSale[index].DonationValue <= value && !inventory.Contains(ballsOnSale[index].Type))
                        {
                            typeToBuy.Add(ballsOnSale[index].Type);
                            value -= ballsOnSale[index].DonationValue;
                        }
                        break;
                }
            }


            if (typeToBuy.Count <= 0)
                return "@" + user + "... Você já possui as skins possiveis de compra... Verifique suas skins com o comando !MySkins e cheque o Sussurro...";

            foreach (var ttb in typeToBuy)
            {
                var ball = await dataContext.Balls.FirstOrDefaultAsync(q => q.Type == ttb);

                var newInventory = new Inventories()
                {
                    Ball = ball,
                    Date = DateTime.Now,
                    IsUsing = false,
                    Username = user,
                };

                ball.SoldAmount += 1;

                dataContext.Inventories.Add(newInventory);

                await cannonHub.Clients.All.OnBuy(user, ball);

            }

            //salvar no bd
            await dataContext.SaveChangesAsync();

            contextCache.UpdateInventories(await dataContext.Inventories.Include(q=>q.Ball).ToListAsync());

            return await EquipUserBall(user, typeToBuy[0]);


        }

        //NÃO ALTERAR
        public async Task SubEnter(string user)
        {
            //eu considero que o usuario que chega até aqui já é um subscriber

            var bdRecord = await dataContext.SubscribersResources.FirstOrDefaultAsync(q => q.Username == user);

            if (bdRecord == null)
            {
                //adiciona uma linha na tabela
                var subResource = new SubscribersResources()
                {
                    Username = user,
                    SubGiftEnable = true
                };

                dataContext.SubscribersResources.Add(subResource);

            }
            else // não tem recurso disponivel
            {
                bdRecord.SubGiftEnable = true;
            }

            await dataContext.SaveChangesAsync();
        }

        //NÃO ALTERAR
        public async Task<string> AddInvitation(string host, string invited)
        {
            //tenta achar um registro com esse nome
            var userDB = await dataContext.Invitations.FirstOrDefaultAsync(q => q.Invited == invited);

            //se encontrar registro
            if (userDB != null)
                return "Desculpe @" + invited + "... Você já foi indicado anteriormente pelo(a) @" + userDB.Host + "...";

            var newInvitation = new Invitations()
            {
                Host = host,
                Invited = invited,
            };

            dataContext.Invitations.Add(newInvitation);

            await dataContext.SaveChangesAsync();

            return null;
        }

        //NÃO ALTERAR
        public async Task BonusInvitations(string user)
        {
            user = user.ToLower();

            //tentar achar no bd o user que foi indicado e que ainda não acertou o target, ou seja, scoreid == null
            var invite = await dataContext.Invitations.FirstOrDefaultAsync(q => q.Invited.ToLower() == user && q.ScoreID == null);

            if (invite == null) return;

            #region +5 Convidado
            await AddPoint(invite.Invited, "Bonus Invitation");
            await AddPoint(invite.Invited, "Bonus Invitation");
            await AddPoint(invite.Invited, "Bonus Invitation");
            await AddPoint(invite.Invited, "Bonus Invitation");
            invite.Score = await AddPoint(invite.Invited, "Bonus Invitation");
            #endregion

            #region +5 Convidante
            await AddPoint(invite.Host, "Bonus Invitation");
            await AddPoint(invite.Host, "Bonus Invitation");
            await AddPoint(invite.Host, "Bonus Invitation");
            await AddPoint(invite.Host, "Bonus Invitation");
            await AddPoint(invite.Host, "Bonus Invitation");
            #endregion

            await dataContext.SaveChangesAsync();

        }

        public async Task TransferName(string user)
        {
            var cache = contextCache.GetUsernames();
            var userCache = cache.FirstOrDefault(q => q.Username == user);

            if (userCache != null)
            {
                IList<Usernames> old = dataContext.Usernames.Where(q => q.User_id == userCache.User_id).ToList();
                IList<string> oldUsers = old.Select(q => q.Username).ToList();
                oldUsers.Remove(user);

                // pegar todos os usuarios com o mesmo id desse desgramado e atualiza nas tabelas
                #region SubscribersGifts // ok
                bool hasRedemption = false;
                foreach (var item in dataContext.SubscribersResources.Where(q => oldUsers.Contains(q.Username)))
                {
                    hasRedemption = (hasRedemption || item.SubGiftEnable);
                    dataContext.SubscribersResources.Remove(item);
                }
                #endregion

                #region Inventories //ok
                foreach (var item in dataContext.Inventories.Where(q => oldUsers.Contains(q.Username)))
                {
                    item.Username = user;
                    item.IsUsing = false;
                }
                #endregion 

                #region Invitations //ok
                foreach (var item in dataContext.Invitations.Where(q => oldUsers.Contains(q.Host)))
                {
                    item.Host = user;
                }
                foreach (var item in dataContext.Invitations.Where(q => oldUsers.Contains(q.Invited)))
                {
                    item.Invited = user;
                }
                #endregion

                #region Records
                foreach (var item in dataContext.Records.Where(q => oldUsers.Contains(q.Username)))
                {
                    item.Username = user;
                }
                #endregion

                #region Resources
                foreach (var res in dataContext.Resources.Where(q => oldUsers.Contains(q.Username)))
                {
                    var updated = await dataContext.Resources.FirstOrDefaultAsync(q => q.Username == user && q.SeasonID == res.SeasonID);
                    if (updated != null)
                    {
                        updated.Balls += res.Balls;
                        res.Balls = 0;
                    }
                    else
                    {
                        res.Username = user;
                    }

                }
                #endregion

                #region Scores
                foreach (var res in dataContext.Scores.Where(q => oldUsers.Contains(q.Username)))
                {
                    var updated = await dataContext.Scores.FirstOrDefaultAsync(q => q.Username == user && q.SeasonID == res.SeasonID);
                    if (updated != null)
                    {
                        updated.Points += res.Points;
                        updated.Tries += res.Tries;

                        res.Points = 0;
                        res.Tries = 0;
                    }
                    else
                    {
                        res.Username = user;
                    }

                }
                #endregion

                foreach (var item in old)
                {
                    item.Username = user;
                }

                await dataContext.SaveChangesAsync();


                //atualiza cache de nomes
                contextCache.UpdateInventories(await dataContext.Inventories.Include(q=>q.Ball).ToListAsync());
                contextCache.UpdateSeason(await GetCurrentSeason());
                contextCache.UpdateUsernames(await dataContext.Usernames.ToListAsync());
            }
        }

        #endregion


        #region Reading from DB

        //consulta do cache somente - //CONSULTAR SOMENTE NO CACHE - DONE
        private bool DoesUserHaveSkin(string user, TypeBalls type)
        {
            var UserBall = contextCache.GetInventories().FirstOrDefault(q => q.Username == user && q.Ball.Type == type);
            if (UserBall != null)
                return true;
            else
                return false;
        }

        //cachable - //CONSULTAR SOMENTE NO CACHE - DONE
        public TypeBalls GetUserEquipedBall(string user)
        {
            //CONSULTAR SOMENTE NO CACHE
            var inventory = contextCache.GetInventories().FirstOrDefault(q => q.Username == user && q.IsUsing);
            if (inventory != null)
                return inventory.Ball.Type;
            else
                return TypeBalls.none;
        }

        //cacheable
        private async Task<Seasons> GetCurrentSeason() => await dataContext.Seasons.Include(e => e.Scores).ThenInclude(e => e.RecordPoints).Include(e => e.Resources).FirstAsync(q => q.EndDate == null && q.Game == "Cannon");

        //BUSCA NO CACHE - DONE
        public TypeBalls[] GetUserSkins(string user) => contextCache.GetInventories().Where(q => q.Username == user).Select(e => e.Ball.Type).ToArray();


        //LER DO CACHE - DONE
        public async Task SendRanking()
        {
            Dictionary<Scores, TypeBalls> list = new Dictionary<Scores, TypeBalls>();

            //var season = GetCurrentSeason(); -- Alternativa invalida devido ao excesso de dados para uma request dos tres primeiros no ranking
            var scores = contextCache.GetSeason().Scores.Where(q => q.Season.Game == "Cannon" && q.Season.EndDate == null).OrderByDescending(q => q.Points)/*.ThenByDescending(q => q.PointIntraDay)*/.Take(3).ToList();

            //verifica se encontrou scores
            if (scores != null && scores.Count > 0)
            {
                var usernamesSelecteds = scores.Select(e => e.Username.ToLower());
                var ballsInventory = contextCache.GetInventories().Where(q => usernamesSelecteds.Contains(q.Username.ToLower()) && q.IsUsing).ToList();

                //montando lista de envio
                IList<IRankItem> ListToSend = new List<IRankItem>();

                foreach (var score in scores)
                {
                    //pega o tipo que esta usando atualmente
                    var type = ballsInventory.FirstOrDefault(q => q.Username.ToLower() == score.Username.ToLower());

                    //adiciona na lista para enviar
                    ListToSend.Add(new IRankItem((type == null) ? TypeBalls.none : type.Ball.Type, score));
                }

                var shoudUpdateList = !contextCache.IsSameRanking(ListToSend);

                //envia dados
                await cannonHub.Clients.All.Ranking(ListToSend.AsEnumerable(), shoudUpdateList);
            }
        }


        //MUDAR CODIGO... CONSULTAR NO CACHE
        public async Task SendJackPot() => await cannonHub.Clients.All.JackPot(contextCache.GetSeason().JackPot);

        //CONSULTAR DO CACHE - DONE
        public bool CanShot(string user)
        {
            //pego a season vigente
            var season = contextCache.GetSeason();

            //tenta selecionar o recurso que o usuario tem nessa season
            var resourceUser = season.Resources.FirstOrDefault(q => q.Username == user);

            //verifica se há recurso
            if (resourceUser == null)
                return false; // não há recurso, logo não pode atirar
            else
                return resourceUser.Balls > 0; //há recurso, e verifica se tem balas disponiveis
        }

        //CONSULTAR DO CACHE - DONE
        public Nullable<KeyValuePair<int, Scores>> GetUserRank(string user)
        {
            var season = contextCache.GetSeason();

            var ranking = season.Scores.OrderByDescending(q => q.Points).ToList();

            int i = 0;
            for (; i < ranking.Count && ranking[i].Username.ToLower() != user.ToLower(); i++) ;

            if (i >= ranking.Count)
                return null;
            else
                return new KeyValuePair<int, Scores>(i + 1, ranking[i]);
        }

        //CONSULTAR DO CACHE - DONE
        public int GetPlayerBalls(string user)
        {
            //pega a season vigente
            var season = contextCache.GetSeason();

            //tenta selecionar o recurso que o usuario tem nessa season
            var resourceUser = season.Resources.FirstOrDefault(q => q.Username == user);

            //verifica se há recurso
            if (resourceUser == null)
                return 0; // não há recurso, logo não pode atirar
            else
                return resourceUser.Balls; //há recurso, e verifica se tem balas disponiveis
        }


        //NÃO ALTERA
        public async Task<bool> CanSubGift(string user)
        {
            //eu considero que o usuario que chega até aqui já é um subscriber

            var bdRecord = await dataContext.SubscribersResources.FirstOrDefaultAsync(q => q.Username.ToLower() == user.ToLower());

            if (bdRecord == null)
            {
                //adiciona uma linha na tabela
                var subResource = new SubscribersResources()
                {
                    Username = user,
                    SubGiftEnable = false
                };

                dataContext.SubscribersResources.Add(subResource);
                await dataContext.SaveChangesAsync();

                return true;
            }
            else if (bdRecord.SubGiftEnable) // ele é resub
            {
                bdRecord.SubGiftEnable = false;
                await dataContext.SaveChangesAsync();

                return true;
            }
            else // não tem recurso disponivel
            {
                return false;
            }
        }

        //NÃO ALTERAR
        public async Task<IEnumerable<Balls>> GetBallsToSale()
        {
            //TODO
            var AllBalls = await dataContext.Balls.Where(q => q.IsActive).AsNoTracking().ToListAsync();

            IList<Balls> ListToSale;

            do
            {
                //cria dicionario de randomização de probabilidades
                IDictionary<Balls, decimal> SortingBalls = new Dictionary<Balls, decimal>();

                //Lista para retornar os valores
                ListToSale = new List<Balls>();

                foreach (var item in AllBalls)
                    SortingBalls.Add(item, item.MarketProbability + (decimal)(new Random().NextDouble() * 110.0)); // TODO adicionar SoldAmount no calculo de probabilidaes


                foreach (var item in SortingBalls.OrderByDescending(q => q.Value))
                {
                    var inlist = ListToSale.FirstOrDefault(q => q.BitValue == item.Key.BitValue || (q.DonationValue == item.Key.DonationValue && q.DonationValue != 0));

                    if (inlist == null) //adiciona na lista safe...
                        ListToSale.Add(item.Key);
                }

                if (ListToSale.Count > 4) ListToSale = ListToSale.Take(4).ToList();

            } while (ListToSale == null || ListToSale.Count != 4);

            //retorna apenas 4 itens pra oferta

            return ListToSale;
        }

        //NÃO ALTERAR
        public async Task<bool> CanSubRedemption(string user)
        {
            //pego a season vigente
            var season =await GetCurrentSeason();

            //verificar no banco se já existe resgate nessa season
            //redemption e mesma season e mesmo usuario
            var bdRecord = await dataContext.Records.Include(e => e.Resource).FirstOrDefaultAsync(q => q.Action == Records.ActionType.SubRedemption && q.Resource.SeasonID == season.SeasonsID && q.Username == user);

            return bdRecord == null;
        }

        // NÃO ALTERAR
        public async Task GenerateList()
        {
            try
            {
                var season = await GetCurrentSeason();
                var listScores = dataContext.Scores.Where(q => q.SeasonID == season.SeasonsID);
                IList<string> listToOutput = new List<string>();


                #region Gera lista
                foreach (var item in listScores)
                {
                    for (int i = 0; i < item.Points; i++)
                        listToOutput.Add(item.Username);
                }

                listToOutput.Shuffle();
                #endregion

                #region Salva Lista
                var fileName = "S" + season.VersionName + "-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
                var targetDirectory = @Directory.GetCurrentDirectory() + @"\CannonLists";

                #region criando path
                Directory.CreateDirectory(targetDirectory);
                #endregion



                using (System.IO.StreamWriter file = new System.IO.StreamWriter(targetDirectory + @"\" + fileName))
                    foreach (string line in listToOutput)
                        file.WriteLine(line);


                #endregion

            }
            catch (Exception expt)
            {
                Console.WriteLine(expt.Message);
            }
        }

        #endregion

        public async void Dispose()
        {
            await dataContext.DisposeAsync();
        }
    }
}
