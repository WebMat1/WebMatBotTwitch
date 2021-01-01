using System;
using System.Collections.Generic;
using System.Text;
using static WebMatBot.Translate;

namespace WebMatBot
{
    public class Speaker : ISpeaker
    {
        public Languages Language { get; set; }

        public string Voice { get; set; }

        public string Alert { get; set; }

        public string Diction { get; set; }

        public string Accent { get; set; }
    }

    public interface ISpeaker
    {
        Languages Language { get; set; }

        string Voice { get; set; }

        string Alert { get; set; }

        string Diction { get; set; }

        string Accent { get; set; }

    }
}
