using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBotV3.Shared;
using WebMatBotV3.Shared.Entity;

namespace WebMatBot.Games
{
    public class Cannon 
    {
        private static double TimeCoolDownShot = 4;
        private static Status _State = Status.Enabled;
        public static Status State 
        {
            get => _State;
            set
            {
                _State = value;
                NewGame();
            }
        } 

        //private static List<string> UsersCanShot = new List<string>();
        private static Dictionary<string, DateTime> CoolDownShot = new Dictionary<string, DateTime>();

        public static Func<float, float, string ,string,Task> Shot;
        public static Func<float, float, string, string,Task> Spray;
        public static Func<int,int,Task> MoveTarget;
        public static Func<Task> NewGame;
        public static Func<string, int> CheckMyBallsFromServer;
        public static Func<Records,Task> SaveRecord;
        public static Func<string, bool> CanShot;
        public static Func<string, Task<bool>> CanSubRedemption;
        public static Func<string, string,Task<string>> AddInvitation;
        public static Func<string, Nullable<KeyValuePair<int,Scores>>> Rank;
        public static Func<Task> GenerateList;
        public static Func<string, Task> TransferName;

        public static async Task SendMessageToWinner(string user, int points, string args)
        {
            await IrcEngine.Whisper(user, "Parabéns... @" + user + "... Você acertou o alvo, com o comando (" + args + ") e agora está com " + points.ToString("00#") + " pontos...");
        }

        public static async Task ShotCommand(string cmd,string user)
        {
            if (!await CheckStatus(user))
                return;

            #region verifica se o usuario tem balas para atirar
            //verificar se está apto a atirar
            if (!CanShot(user))
            {
                await IrcEngine.Respond("Ops... Me Desculpe... Se você quiser brincar com o canhão, primeiro deve se eleger; Eleções são feitas Por pontos do canal ou donations (verifique com o streamer)!!!", user);
                return;
            }
            #endregion

            #region verifica parametros passados
            //paremetros passados
            float angle, power;
            if (!GetShotParameters(cmd, out angle, out power))
            {
                await IrcEngine.CommandCorrector(cmd, "!Shot", user: user, shouldBeExact: true);
                return;
            }
            #endregion

            #region Verificar frequencia de tiro por usuario
            //verificar se está atirando com muita frequencia
            if (CoolDownShot.ContainsKey(user))
            {
                if (CoolDownShot[user].AddSeconds(TimeCoolDownShot) >= DateTime.Now)
                {
                    await IrcEngine.Respond("Ops... Rápido demais... Dê uma respirada, coleguinha!!!", user);
                    return;
                }
                //atualiza lista de cooldown
                else
                    CoolDownShot[user] = DateTime.Now;
            }
            else
            {
                //adiciona na lista de coolDown
                CoolDownShot.Add(user, DateTime.Now);
            }
            #endregion

            if (Shot != null) await Shot(angle, power, user, cmd);

        }
        public static async Task SprayCommand(string cmd, string user)
        {
            if (!await CheckStatus(user))
                return;

            #region verifica se o usuario tem balas para atirar
            //verificar se está apto a atirar
            if (!CanShot(user))
            {
                await IrcEngine.Respond("Ops... Me Desculpe... Se você quiser brincar com o canhão, primeiro deve se eleger; Eleções são feitas Por pontos do canal ou donations (verifique com o streamer)!!!", user);
                return;
            }
            #endregion

            #region verifica parametros passados
            //paremetros passados
            float angle, power;
            if (!GetShotParameters(cmd, out angle, out power))
            {
                await IrcEngine.CommandCorrector(cmd, "!Shot", user: user, shouldBeExact: true);
                return;
            }
            #endregion

            #region Verificar frequencia de tiro por usuario
            //verificar se está atirando com muita frequencia
            if (CoolDownShot.ContainsKey(user))
            {
                if (CoolDownShot[user].AddSeconds(TimeCoolDownShot*2.5) >= DateTime.Now)
                {
                    await IrcEngine.Respond("Ops... Rápido demais... Quando se usa !Spray o tempo de coolDown é um pouquinho maior... Sorry... marcob3Like marcob3Like ", user);
                    return;
                }
                //atualiza lista de cooldown
                else
                    CoolDownShot[user] = DateTime.Now;
            }
            else
            {
                //adiciona na lista de coolDown
                CoolDownShot.Add(user, DateTime.Now);
            }
            #endregion

            if (Spray != null) await Spray(angle, power, user, cmd);

        }
        public static async Task SubRedemptionCommand(string cmd, string user, UserTitle title)
        {
            #region Verifica se é sub
            if (!title.Permissions.Contains(Permissions.Subscriber))
            {
                await IrcEngine.Respond("Desculpe o resgate é apenas permitido por quem está inscrito no canal... TehePelo", user);
                return;
            }
            #endregion

            #region verifica se o usuario já não resgatou o comando
            //verificar se está apto a atirar
            if (!await CanSubRedemption(user))
            {
                await IrcEngine.Respond("Ops... Me Desculpe... Seu resgate de Subscriber já foi efetuado nessa Season. Tente na Proxima season...", user);
                return;
            }
            #endregion

            //save os records
            await SaveRecord(
                new Records()
                {
                    Action = Records.ActionType.SubRedemption,
                    Arguments = "20 0",
                    Date = DateTime.Now,
                    Username = user,
                });

        }
        public static async Task GenerateListCommand()
        {
            await GenerateList();
        }

        public static async Task RankCommand(string args, string user)
        {
            var result = Rank(user);
            if (result != null)
                await IrcEngine.Whisper(user, "@" + user + "... Você está na posição " + result.Value.Key + ", com " + result.Value.Value.Points+" pontos e sendo destes " + result.Value.Value.PointIntraDay + " somente hoje... Parabéns continue acertando o alvo...");
            else
                await IrcEngine.Whisper(user, "@" + user + "... Você ainda não está no nosso Ranking... Continue tentando acertar o alvo para começarmos a contagem de seus pontos...");
        }

        private static bool GetShotParameters(string stringRaw, out float angle, out float power)
        {
            stringRaw = stringRaw.ToLower().Trim();
            var parts = stringRaw.Split(" ");

            angle = -1;
            power = -1;


            if (parts.Length != 2)
                return false;

            if (!float.TryParse(parts[0].Replace(".",","),out angle) || angle < 0 || angle > 90)
                return false;

            if (!float.TryParse(parts[1].Replace(".", ","), out power) || power < 0 || power > 100)
                return false;


            return true;


        }

        public static async Task MoveTargetCommand(string stringRaw, string user)
        {
            stringRaw = stringRaw.ToLower().Trim();
            bool done = false;

            switch (stringRaw.ToLower())
            {
                case "up":
                    await UpdateAxisY(1);
                    done = true;
                    break;

                case "down":
                    await UpdateAxisY(-1);
                    done = true;
                    break;

                case "left":
                    await UpdateAxisX(-1);
                    done = true;
                    break;

                case "right":
                    await UpdateAxisX(1);
                    done = true;
                    break;
            }


            if (!done)
            {
                await IrcEngine.CommandCorrector(stringRaw, "!Target", user: user, shouldBeExact: true);
            }

        }

        private static async Task UpdateAxisX(int x)
        {
            await MoveTarget(x, 0);
        }
        private static async Task UpdateAxisY(int y)
        {
            await MoveTarget(0,y);
        }

        public static async Task<bool> CheckStatus(string user)
        {
            if (State == Status.Disabled)
            {
                await IrcEngine.Respond("O Cannon Game está off... peça o streamer para acioná-lo...", user);
                return false;
            }
            else if(State == Status.Paused)
            {
                await IrcEngine.Respond("O Cannon Game está pausado... Provavelmente o nosso Skins Store esteja rolando... Confira as promoções de Skins e divirta-se...", user);
                return false;
            }
            else
                return true;
        }

        public static async Task NewRedemptionGame(string user, string args) //args = balls jackpot
        {
            var newRecordBuy = new Records()
            {
                Action = Records.ActionType.Buy,
                Arguments = args,
                Date = DateTime.Now,
                Username = user,
            };

            await SaveRecord(newRecordBuy);

            await CheckMyBalls(user);
        }
        public static async Task CheckMyBalls(string user)
        {
            //responder para o usuario quantas balas ele pode atirar, até o momento
            await IrcEngine.Whisper(user,user + "... Você tem um total de " + CheckMyBallsFromServer(user) + " bolas de canhão para atirar... divirta-se usando o !Shot");
        }

        public static async Task InvitedBy(string args, string user)
        {
            string result = null;

            args = args.Trim().Replace("@","");

            //checar se o username (args) existe
            if (args == null || args.ToLower() == user.Trim().ToLower())
                result = "@"+ user +"... Erro ao processar comando de indicação...";
            else
                result = await AddInvitation(args, user); //mandar comando para o server para adicionar no bd

            if (result == null) // result  == null é que deu tudo certo 
            {
                //manda whisper tanto para quem indicou, quanto pra quem foi indicado, avisando como será dado os pontos
                await IrcEngine.Whisper(user, "Parabéns, você efetivou sua indicação... Acerte o alvo com !Shot [Angulo] [Força] para ganhar pontos extras para você e quem te indicou...");

                //envia mensagem pra o host
                await IrcEngine.Whisper(args, "Parabéns, "+ user +" efetivou sua indicação... Incentive-o a acertar o alvo com !Shot [Angulo] [Força] para ganhar pontos extras para você e sua indicação...");
            }
            else
            {
                await IrcEngine.Respond(result, user);
            }
        }

        public static async Task AddBallsStreamLabs(string user, string cmd)
        {
            if ((user.ToLower() == "streamlabs" || user.ToLower() == Parameters.User.ToLower()) && Cannon.State == Status.Enabled)
            {
                cmd = cmd.Trim();
                var parameters = cmd.Split(" ");
                var userDonate = parameters[0];
                var valueDonate = parameters[1].Replace("r$","").Replace(".",",");

                double doubleValue = 0;

                //calculos de jackpot e bolas a disponibilizar
                if (double.TryParse(valueDonate, out doubleValue))
                {
                    var jackPotAdd = (doubleValue * 0.7d) / 2d;
                    var ballsAdd = (int)(doubleValue / 0.20d);
                    string args = ballsAdd.ToString()+ " " + jackPotAdd.ToString().Replace(".", ",");
                    await Games.Cannon.NewRedemptionGame(userDonate, args);
                }
            }
        }

        public enum Status
        {
            Disabled,
            Enabled,
            Paused,
        }
    }
}
