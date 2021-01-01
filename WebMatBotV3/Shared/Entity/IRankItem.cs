using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBotV3.Shared.Entity
{
    public class IRankItem
    {
        public Scores Score { get; set; }
        public TypeBalls TypeBall { get; set; }

        public IRankItem(TypeBalls _t, Scores _s)
        {
            Score = _s;
            TypeBall = _t;
        }
    }
}
