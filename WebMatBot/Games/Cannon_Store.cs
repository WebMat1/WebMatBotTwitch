using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBotV3.Shared;
using WebMatBotV3.Shared.Entity;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBot.Games
{
    public class Cannon_Store
    {
        private static double StoreDurationSeconds = (new TimeSpan(0, 5, 0)).TotalSeconds;
        private static IEnumerable<Balls> BallsOnSale { get; set; }
        public static Cannon.Status State { get; set; } = Cannon.Status.Disabled;
        
        public static Func<double, IEnumerable<Balls>,Task> OpenMarket;
        public static Func<Task<IEnumerable<Balls>>> GetBallsToSale;
        public static Func<string, float, List<Balls>, TypePayment,Task<string>> BuyBall;
        public static Func<string, TypeBalls[]> GetUserSkins;
        public static Func<string, TypeBalls ,Task<string>> EquipUserBall;
        public static Func<string, Task<bool>> CanSubGift;
        public static Func<string, Task> SubEnter;

        public static async Task Start()
        {
            await Task.Delay(10000);

            do
            {
                if (State == Cannon.Status.Paused)
                    await OpenMarketCommand();

                await Task.Delay(5000);

            } while (true);
        }

        public static async Task OpenMarketCommand()
        {
            //Cannon.State = Cannon.Status.Paused;
            State = Cannon.Status.Enabled;

            //pergunta pro servidor quais 4 bolas vão pra leilão
            BallsOnSale = await GetBallsToSale();

            await OpenMarket(StoreDurationSeconds, BallsOnSale.OrderBy(q=>q.BitValue)) ;
        }

        public static async Task CloseMarketCommand()
        {
            //compensar delay da twitch de transmissao
            await Task.Delay(5000);

            //Cannon.State = Cannon.Status.Enabled;
            State = Cannon.Status.Paused;
            BallsOnSale = new List<Balls>();
            //await Task.Delay(500);
        }

        public static async Task SubGiftCommand(string user, UserTitle userTitle)
        {
            //significa que mercado ta fechado...
            if (State != Cannon.Status.Enabled)
                await IrcEngine.Respond("O mercado de Balas de Canhão está fechado =/, mesmo assim, muito obrigado por ser Sub =) ... VoHiYo VoHiYo ", user);
            else
            {
                if (!await CanSubGift(user))
                {//confere e salva a baixa de recurso
                    await IrcEngine.Whisper(user, "@" + user + "... Você não tem recursos para solicitar essa chamada... Provavelmente você já o resgatou... Resubs são uma boa opção para lhe disponibizar novos recursos...");
                    return;
                }

                //pega todos os valores de bolas ofertadas
                var values = BallsOnSale.Select(q => q.BitValue).ToList();

                //mistura a lista
                values.Shuffle();

                //seleciona um valor dentre os valores anteriores
                int RandomIndex = new Random().Next(0, BallsOnSale.Count());

                //envia o buyball com o valor selecionado e metodo bits

                var result = await BuyBall(user, values[RandomIndex], BallsOnSale.ToList(), TypePayment.bits);

                if (result != null) //tudo certo, adiquiriu a bala
                {
                    await IrcEngine.Respond(result, user);
                }
                
            }

            
        }

        public static async Task BuyBallCommand(string user, float bits, TypePayment payment)
        { 
            //significa que mercado ta fechado...
            if (State != Cannon.Status.Enabled)
                await IrcEngine.Respond("O mercado de Balas de Canhão está fechado =/, mesmo assim, muito obrigado pelos Bits =) ... VoHiYo VoHiYo ",user);
            else
            {

                var result = await BuyBall(user, bits, BallsOnSale.ToList(), payment);

                if (result != null) //tudo certo, adiquiriu a bala
                {
                    await IrcEngine.Respond(result,user);
                }
            }
        }

        public static async Task EquipCommand(string cmd,string user)
        {
            cmd = cmd.ToLower().Trim(); //retira os espaços

            var type = Enum.GetNames(typeof(TypeBalls)).FirstOrDefault(q=>q.ToLower() == cmd);

            //sem skin com esse nome
            if (type == null)
            {
                await IrcEngine.Whisper(user, "@" + user + "... Não encontrei nenhuma skin com este nome...");
                await GetUserSkinsCommand(user);
            }
            else
            {
                var Typeball = Enum.Parse<TypeBalls>(type);

                // tentar setar no banco dados que a skin selecionada é a isUsing
                string result;

                //manda para o servidor atualizar a bola
                result = await EquipUserBall(user, Typeball);


                if (string.IsNullOrEmpty(result))
                    await IrcEngine.Whisper(user, "@" + user + "... Parabéns, você equipou a sua " + type + " Ball... Divirta-se usando o !Shot"); //sucesso
                else
                {
                    await IrcEngine.Whisper(user, result); //erro
                    await Task.Delay(5000);
                    await GetUserSkinsCommand(user);
                }
                    
            }

            return;
        }

        public static async Task GetUserSkinsCommand(string user)
        {
            var myTypes = GetUserSkins(user);

            if (myTypes == null || myTypes.Length == 0)
                await IrcEngine.Whisper(user, "Você não possui nenhuma Skin... Aguarde ou Invoque a abertura do Ball Store para comprar alguma... e divirta-se...");
            else
                await IrcEngine.Whisper(user, "@" + user + "... Você possui as skins: " + String.Join(" ", myTypes.Select(q => q.ToString()))+"... Use o !Equip [nome da skin] para ativá-la...");
        }

        public static async Task SubEnterResource(string user) => await SubEnter(user);

        public enum TypePayment
        {
            bits,
            donation
        }
    }
}
