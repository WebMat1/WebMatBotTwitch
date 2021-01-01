using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Shared.Entity;

namespace WebMatBotV3.Server.Services
{
    public class ContextCache : BaseCache
    {
        private IList<IRankItem> LastRanking { get; set; } = new List<IRankItem>();

        public bool IsSameRanking(IList<IRankItem> newList)
        {
            if (newList.Count != LastRanking.Count)
            {
                //copia a lista pra memoria do singleton
                LastRanking = newList.ToList();

                //retorna resultado;
                return false;
            }
            else
            {
                bool flagNegative = false;

                for(int i = 0; i < newList.Count && flagNegative == false; i++)
                {
                    if (newList[i].Score.Username != LastRanking[i].Score.Username)
                        flagNegative = true;
                }

                if (flagNegative)
                    LastRanking = newList.ToList();

                return !flagNegative;
            }
        }
    }

    public class BaseCache
    {
        private Seasons Season { get; set; }

        private IList<Inventories> Inventories { get; set; }

        private IList<Usernames> Usernames { get; set; }

        public async Task Init(Shared.DataContext dataContext)
        {
            var _season = await dataContext.Seasons.Include(e => e.Scores).ThenInclude(e => e.RecordPoints).Include(e => e.Resources).FirstAsync(q => q.EndDate == null && q.Game == "Cannon");
            var _inventories = await dataContext.Inventories.Include(q => q.Ball).ToListAsync();
            var _usernames = await dataContext.Usernames.ToListAsync();
            lock (this)
            {
                Season = _season;
                Inventories = _inventories;
                Usernames = _usernames;
            }
        }

        public Seasons GetSeason()
        {
            //cloca o objeto da memoria e disponibiliza o necessario
            lock (this)
                return ((BaseCache)this.MemberwiseClone()).Season;
        }
        public IList<Inventories> GetInventories()
        {
            //cloca o objeto da memoria e disponibiliza o necessario
            lock (this)
                return ((BaseCache)this.MemberwiseClone()).Inventories;
        }
        public IList<Usernames> GetUsernames()
        {
            //cloca o objeto da memoria e disponibiliza o necessario
            lock (this)
                return ((BaseCache)this.MemberwiseClone()).Usernames;

        }

        public void UpdateSeason(Seasons _season)
        {
            lock (Season)
                Season = _season;
        }
        public void UpdateInventories(IList<Inventories> _inv)
        {
            lock (Inventories)
                Inventories = _inv;
        }
        public void UpdateUsernames(IList<Usernames> _usernames)
        {
            lock (Usernames)
                Usernames = _usernames;
        }

    }
}
