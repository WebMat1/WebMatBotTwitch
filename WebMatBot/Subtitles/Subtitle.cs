using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebMatBot.Translate;

namespace WebMatBot
{
    public class Subtitle
    {
        public static bool IsActive { get; set; } = false;

        public static Translate.Languages TargetLanguage { get; set; } = Translate.Languages.en;

        public static void TurnOn()
        {
            IsActive = false;
        }

        public static void TurnOff()
        {
            IsActive = false;
        }

        public static async Task Command(string cmd, string user)
        {
            cmd = cmd.ToLower().Trim();

            if (cmd == "false" || cmd== "true")
            {
                IsActive = bool.Parse(cmd);
            }
            else
            {
                string msg;
                Languages? src, trg;
                if (GetLanguages(cmd, out src, out trg, out msg))
                {
                    IsActive = true;
                    TargetLanguage = trg.Value;

                    await IrcEngine.Respond("A Legenda está traduzindo para " + TargetLanguage.ToString() + "...", user);
                }
            }
            
        }
    }
}
