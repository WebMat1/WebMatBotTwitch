using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models.ColorFlow;

namespace WebMatBot.Lights
{
    public class AudioVisual
    {
        public static async Task Party(string txt, string user)
        {
            try
            {
                //sem await para não ter q parar as luzes e depois falar o texto
                Light.StartLightFlow(user);

                Sounds.RandomPartySound();

                await Light.StopLightFlow();
                    
            }
            catch (Exception ex)
            {
                await IrcEngine.CommandCorrector(txt, "!Light",user:user, shouldBeExact:true);
            }
        }

        public static async Task Anthem(string txt, string user)
        {
            //sem await para não ter q parar as luzes e depois falar o texto
            Light.StartLightFlowAnthem(user);

            Random random = new Random();
            var index = random.Next(Enum.GetValues(typeof(Translate.Languages)).Length);

            //colocar o speaker pra falar em uma lingua randomicamente
            GoogleSpeakers.Speak("golira, tevelisão, abdominável, estrepe, pobrema, mortandela, entertido, carda?o, salchicha, meia cansada, asterístico, " +
                "ciclo vicioso, bicabornato, beneficiente, metereologia, triologia, conhecidência, célebro, entertido, madastra, imbigo, cocrante, padastro, " +
                "iorgurte, trabisseiro, bassoura, menas, seje, provalecer, esteje, guspe, chuva de granito... Salve todos os BOTES dos DEVES...", "OBOT", (Translate.Languages)Enum.GetValues(typeof(Translate.Languages)).GetValue(index));

            await Light.StopLightFlow();
        }

        public static async Task Xandao(string txt, string user)
        {
            try
            {
                //sem await para não ter q parar as luzes e depois falar o texto
                Light.StartLightFlow(user);

                Sounds.Xandao();

                await Light.StopLightFlow();

            }
            catch (Exception ex)
            {
                await IrcEngine.CommandCorrector(txt, "!AudioVisual",user:user, shouldBeExact:true);
            }
        }
    }
}
