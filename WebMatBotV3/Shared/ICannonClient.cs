using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBotV3.Shared.Entity;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBotV3.Shared
{
    public interface ICannonClient
    {
        Task Shot(float angle, float power, string user, TypeBalls skin);
        Task Spray(float angle, float power, string user, TypeBalls skin, int qtdballs);
        Task MoveTarget(int x, int y);
        Task Ranking(IEnumerable<IRankItem> scores, bool shouldUpdateList);
        Task JackPot(float newJackPot);
        Task OpenMarket(double time, IEnumerable<Balls> balls);
        Task OnBuy(string user, Balls ball);
    }
}
