using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBot.Core;
using WebMatBot.Lights;
using WebMatBotV3.Shared;

namespace WebMatBot
{
    public static class Commands
    {
        public static string ProjectLink { get; set; }
        public static string TetrisLink { get; set; }


        public static IList<Command> List = new List<Command>()
        {
            
            {new Command("!TaIrritadoTio" , async (string text, string user, UserTitle title) => await FieisEscudeiros(user), "" ) },
            {new Command("!Eletronoob" , async (string text, string user, UserTitle title) => await Traira(user), "" ) },
            {new Command("!Tetris" , async (string text, string user, UserTitle title) => await Tetris(user), "Quando estivermos jogando tetris, envie esse comando para receber o link da sala e jogar conosco;" ) },
            {new Command("!Store" , async (string text, string user, UserTitle title) => await Store(user), "Para ver itens na nossa loja da streamelements, ou até mesmo utilizar a voz do Ricardo (9? 99? G? 3? X?);" ) },
            {new Command("!Donate" , async (string text, string user, UserTitle title) => await Donate(user), "Apoie nosso canal financeiramente... Muito obrigado! Namastê! Link do PicPay;" ) },
            {new Command("!Donate" , async (string text, string user, UserTitle title) => await DonatePP(user), "Apoie nosso canal financeiramente... Muito obrigado! Namastê! Link do PayPal;"  ) },

            //{new Command("!Pesquisa" , async (string text, string user, UserTitle title) => await Pesquisa(user), "Deixe-me entender mais sobre o que você gostaria de ver na live;" ) },
            {new Command("!Salve" , async (string text, string user, UserTitle title) => await Salve(user), "Cumprimento Alah Igão;" ) },
            {new Command("!GitHub" , async (string text, string user, UserTitle title) => await GitHub(user), "Caso tenha curiosidade de entender mais sobre esse que vos fala (nosso bot);" ) },
            {new Command("!YouTube" , async (string text, string user, UserTitle title) => await Youtube(user), "Compilados do que há de super interessante nessa live, divirta-se;" ) },
            {new Command("!Discord" , async (string text, string user, UserTitle title) => await Discord(user), "Aqui a comunidade é forte e parceira, junte-se a nós;" ) },

            {new Command("!Exclamação" , async (string text, string user, UserTitle title) =>  await Exclamacao(user), "Lista todos os comandos disponíveis aqui;" )},
            {new Command("!Piada" , (string text, string user, UserTitle title) =>  Sounds.Piada(), "Faz o nosso bot tocar aquele som de fim de piada (bateria);" , new Permissions[] { Permissions.Subscriber, Permissions.Moderator, Permissions.VIP})},
            {new Command("!Clap" , (string text, string user, UserTitle title) =>  Sounds.Clap(), "Faz o nosso bot tocar som de palmas, pra exaltar o streamer;" , new Permissions[] { Permissions.Subscriber, Permissions.Moderator, Permissions.VIP})},
            {new Command("!DeiF5" ,async (string text, string user, UserTitle title) => await Cache.Respond(user), "Caso sua live tenha travado e você recarregar a pagina, use esse comando para obter as ultimas mensagens enviadas no nosso chat;" ) },

            {new Command("!Translate", async(string text, string user, UserTitle title) => await Translate.TranslateText(text,true, user), "Traduz um texto para linguagem desejada : !Translate {sigla da lingua de origem}-{sigla da lingua de destino} {texto a ser traduzido} : !Translate en-pt Hello World! ;" ) },
            {new Command("!SetTranslate",async (string text, string user, UserTitle title) => await AutomaticTranslator.Command(text, user), "Todo texto enviado nos comandos de Speak são traduzidos para a lingua desejada e enviados automaticamente no chat : !SetTranslate {lingua desejada} : !SetTranslate pt ;" ) },
            {new Command("!SpeakTranslate", async(string text, string user, UserTitle title) =>  await GoogleSpeakers.SpeakTranslate(text, user), "Se você quiser dizer algo em outra lingua mas não sabe como se escreve, tente isso : !SpeakTranslate {lingua que a voz irá falar} {texto para a voz falar} : !speaktranslate en Olá a todos lindões;" )},
            {new Command("!Subtitle", async(string text, string user, UserTitle title) =>  await Subtitle.Command(text, user), "Caso você queira legendas na nossa live : !Subtitle {lingua da legenda} : !Subtitle en : !Subtitle false" )},

            {new Command("!Speak", async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await SpeakerCore.Speak(text, user), user), "Fala com lingua em pt-br : !speak {seu texto aqui} : !speak Falo do brasil..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakHigh", async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await SpeakerCore.Speak(text, user, speakRate:"-3"), user), "Fala com lingua em pt-br com um leve sotaque de bêbado com sono : !speakDrunk {seu texto aqui} : !speakDrunk Falo do brasil... alcoolizado..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakPt",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.pt), user), "Fala com lingua em pt-pt : !speakpt {seu texto aqui} : !speakpt falo de Portugal..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakEn",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.en), user), "Fala com lingua em english : !speaken {seu texto aqui} : !speaken falo em Inglês..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakDe",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.de), user), "Fala com lingua em alemão : !speakde {seu texto aqui} : !speakde falo em Alemão..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakRu",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.ru), user), "Fala com lingua em russo : !speakru {seu texto aqui} : !speakru falo em Russo (cabeça)..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakFr",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.fr), user), "Fala com lingua em francês : !speakfr {seu texto aqui} : !speakfr falo em francês (we we)..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakIt",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.it), user) , "Fala com lingua em italiano : !speakit {seu texto aqui} : !speakit falo em Italiano (nono nona)..." )}, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakAr",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.ar), user), "Fala com lingua em árabe : !speakar {seu texto aqui} : !speakar falo em Árabe..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakEl",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.el), user), "Fala com lingua em grego : !speakel {seu texto aqui} : !speakel falo em Grego..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakJa",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.ja), user), "Fala com lingua em japones : !speakja {seu texto aqui} : !speakja falo em Japones (uéb mat san)..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakZh",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.zh), user), "Fala com lingua em chinês : !speakzh {seu texto aqui} : !speakzh falo em Chinês..." ) }, //to activate spekaer... goes to console and type "!setspeaker true"
            {new Command("!SpeakEs",async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await GoogleSpeakers.Speak(text, user, Translate.Languages.es), user), "Fala com lingua em espanhol : !speakes {seu texto aqui} : !speakes falo em Espanhol..." ) }, //to activate spekaer... goes to console and type "!setspeaker true

            {new Command("!ScreenVS", async (string text, string user, UserTitle title) => await Screens.VS(user), "Muda a tela para ver o Visual Studio", new Permissions[] { Permissions.Moderator, Permissions.VIP} ) }, //to activate screen change... goes to console and type "!setscreen true"
            {new Command("!ScreenVSCode", async (string text, string user, UserTitle title) => await Screens.VSCode(user), "Muda a tela para ver o Visual Studio CODE", new Permissions[] { Permissions.Moderator, Permissions.VIP} ) }, //to activate screen change... goes to console and type "!setscreen true"
            {new Command("!ScreenCozinha", async (string text, string user, UserTitle title) => await Screens.Kitchen(user), "Muda a tela para ver a cozinha... (Nem sempre habilitado... Privacidade né...)", new Permissions[] { Permissions.Moderator, Permissions.VIP} ) }, //to activate screen change... goes to console and type "!setscreen true"
            {new Command("!ScreenTela1", async (string text, string user, UserTitle title) => await Screens.Browser(user), "Muda a tela para ver o que há no monitor 1", new Permissions[] { Permissions.Moderator, Permissions.VIP} ) }, //to activate screen change... goes to console and type "!setscreen true"
            {new Command("!ScreenChat", async (string text, string user, UserTitle title) => await Screens.Chat(user), "Muda a tela para ver o chat e o streamer lindão...", new Permissions[] { Permissions.Moderator, Permissions.VIP} ) }, //to activate screen change... goes to console and type "!setscreen true"
            {new Command("!ScreenCannon", async (string text, string user, UserTitle title) => await Screens.Cannon(), "Muda a tela para ver o chat e o streamer lindão..." ) }, //to activate screen change... goes to console and type "!setscreen true"


            {new Command("!Anthem", async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech(async () => await AudioVisual.Anthem(text,user), user), "Hino de todos os bots...", new Permissions[] { Permissions.Bits} )  },
            {new Command("!Light", async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddLight( async() => await Light.Command(text, user), user), "Muda a cor da lâmpada no quarto : !light rgb({0:255} ,{0:255}, {0:255})  : !light rgb(255,255,255)" ) },
            {new Command("!Party", async (string text, string user, UserTitle title) => await TasksQueueOutput.QueueAddSpeech( async() => await AudioVisual.Party(text, user), user), "Convoca uma festa de musica e luzes para o streamer : !Party", new Permissions[] { Permissions.Moderator, Permissions.VIP, Permissions.Subscriber} ) },

            {new Command("!Shot", async (string text, string user, UserTitle title) => await Games.Cannon.ShotCommand(text, user), "Depois de se elegar resgatando com pontos do canal o CannonGame, use este comando para efetivar o disparo : !Shot {angulo[0,90]} {força[0,100]} : !Shot 65.0 45.0 " ) },
            {new Command("!Spray", async (string text, string user, UserTitle title) => await Games.Cannon.SprayCommand(text, user), "Depois de se elegar resgatando com pontos do canal o CannonGame, use este comando para efetivar uma rajada de disparo : !Spray {angulo[0,90]} {força[0,100]} : !Spray 65.0 45.0 " ) },
            {new Command("!Target", async (string text, string user, UserTitle title) => await Games.Cannon.MoveTargetCommand(text, user), "Quando Alguem estiver disparando no Cannon, você pode alterar onde está o alvo : !Target {'Up' || 'Down' || 'Left' || 'Right'} : !Target Up " ) },
            {new Command("!CheckBalls", async (string text, string user, UserTitle title) => await Games.Cannon.CheckMyBalls(user), "Verifique quantas bolas de canhão ainda restam para você tentar acertar o alvo : !CheckBalls" ) },
            {new Command("!InvitedBy", async (string text, string user, UserTitle title) => await Games.Cannon.InvitedBy(text, user), "Se alguem te indicar, use esse comando para no próximo aceto do canhão, você e quem te indicou ganharem pontos... : !InvitedBy [nick de quem te indicou] : !InvitedBy WebMat1" ) },
            {new Command("!SubCannon", async (string text, string user, UserTitle title) => await Games.Cannon.SubRedemptionCommand(text, user, title), "Se alguem te indicar, use esse comando para no próximo aceto do canhão, você e quem te indicou ganharem pontos... : !InvitedBy [nick de quem te indicou] : !InvitedBy WebMat1" ) },
            {new Command("!Rank", async (string text, string user, UserTitle title) => await Games.Cannon.RankCommand(text, user), "Verifique quantos pontos e posição atual você tem alcançado com o nosso canhão: !Rank" ) },
            {new Command("!Equip", async (string text, string user, UserTitle title) => await Games.Cannon_Store.EquipCommand(text, user), "Equipa uma skin nos seus proximos shots : !Equip [nome_da_skin]" ) },
            {new Command("!MySkins", async (string text, string user, UserTitle title) => await Games.Cannon_Store.GetUserSkinsCommand(user), "Lista no sussurro todas as skins que você possui no Cannon Game : !MySkins" ) },
            {new Command("!SubGift", async (string text, string user, UserTitle title) => await Games.Cannon_Store.SubGiftCommand(user, title), "Resgate uma skin aleatória como um dos benefícios de sub : !SubGift", new Permissions[] { Permissions.Subscriber} ) },
            {new Command("!Transfer", async (string text, string user, UserTitle title) => await Games.Cannon.TransferName(user), "Transfere os dados do game para o seu novo usuario, caso você tenha trocado de nome na Twitch : !Transfer" ) },
            {new Command("!AddBalls", async (string text, string user, UserTitle title) => await Games.Cannon.AddBallsStreamLabs(user, text), "", new Permissions[] { Permissions.Moderator} ) },
            {new Command("!BuyBall" , async (string text, string user, UserTitle title) => await DonatePP(user), "Apoie nosso canal financeiramente... Muito obrigado! Namastê! Link do PayPal;"  ) },

            {new Command("!Caipirinha", async (string text, string user, UserTitle title) => await IrcEngine.Respond("Caipirinha 1 copo super top do webmat. 1. Corte o limão no meio começando pelo topo do limão. 2. Tire a parte branca do limão. 3. Corte o limão em 4 pedaços. 4. Coloque em um copo. 5. Jogue açúcar a gosto ( recomendado 1 colher, pode adicionar mais depois). 6. Esmague o limão com açúcar. 7. Adicione 100 Ml de vodca. 8. Coloque gelo e misture. 9. Beba com moderação, se beber não dirija.",user), "Aprenda a fazer uma deliciosa caipirinha com GuiCortiz" ) },


        };

        private static async Task FieisEscudeiros(string user) =>
            await IrcEngine.Respond("Pessoa Magnifica, dotada de belezam, sensualidade e inteligência... SQN",user);

        private static async Task Traira(string user) =>
            await IrcEngine.Respond("Comando Removido pois este é TRAIRA... NÃO É AMIGO..", user);

        private static async Task Youtube(string user) =>
            await IrcEngine.Respond(StreamerDefault.Youtube, user);

        private static async Task Discord(string user) =>
            await IrcEngine.Respond(StreamerDefault.Discord,user);

        private static async Task GitHub(string user) => 
            await IrcEngine.Respond(StreamerDefault.GitHub, user);

        private static async Task Tetris(string user) =>
            await IrcEngine.Respond(TetrisLink, user);

        private static async Task Salve(string user)=>
            await IrcEngine.Respond("Salve alá Igão... FortBush CurseLit", user);

        private static async Task Store(string user)=>
            await IrcEngine.Respond(StreamerDefault.StreamElements_Store, user);
        

        private static async Task Donate(string user)
        {
            await IrcEngine.Respond(StreamerDefault.PicPay, user);
        }

        private static async Task DonatePP(string user)
        {
            await IrcEngine.Respond(StreamerDefault.PayPal, user);
        }

        private static async Task Exclamacao(string user)
        {
            user = user.Replace("mod ", "");

            foreach(var item in List)
            {
                await IrcEngine.Whisper(user, item.Key + "    : " + item.Description);
                await Task.Delay(1000);
            }
                

            await IrcEngine.Respond("Por favor, confira a aba de sussurros...", user);
        }

        private static async Task Pesquisa(string user)
        {
            await IrcEngine.Respond(StreamerDefault.Form, user);
        }



        public static async Task<bool> CheckPermissions(Command command, UserTitle userTitle)
        {
            bool canExec = false;
            for (int i = 0; i < command.Permissions.Length && !canExec; i++)
                canExec = userTitle.Permissions.Contains(command.Permissions[i]);

            if (canExec || userTitle.Permissions.Contains(Permissions.Broadcaster))
                return true;

            await IrcEngine.Respond($"Esta ação requer título de {String.Join(" ou ", command.Permissions)}... Desculpe-me, mas não posso executá-la.");

            return false;
        }
    }
}
