using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static WebMatBot.Translate;
using System.Threading.Tasks;

namespace WebMatBot
{
    public class AutomaticTranslator
    {
        public static Languages Target { get; set; }

        public static Status status { get; set; } = Status.Disabled;

        public enum Status
        {
            Enabled,
            Disabled
        }

        public static async Task Command (string cmd)
        {
            cmd = cmd.ToLower().Trim();

            if (cmd == "false")
            {
                status = Status.Disabled;
            }
            else
            {
                string msg;
                Languages? src, trg;
                if (GetLanguages(cmd,out src,out trg, out msg))
                {
                    status = Status.Enabled;
                    Target = trg.Value;

                    await IrcEngine.Respond("O tradutor está automatico para " + Target.ToString() + "...");
                }
            }
        }

        public static async Task Translate(string text)
        {
            if (status == Status.Disabled)
                return;

            await TranslateCore(text, true, Target);

        }
    }
}
