using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBot.Core;
using static WebMatBot.Translate;

namespace WebMatBot
{
    public class AzureSpeakers
    {
        private static SpeechConfig config = (string.IsNullOrEmpty(Parameters.AzureCognitiveKey)||string.IsNullOrEmpty(Parameters.AzureCognitiveRegion)) ? null : SpeechConfig.FromSubscription(Parameters.AzureCognitiveKey, Parameters.AzureCognitiveRegion);//https://portal.azure.com/
        private static IList<Speaker> Speakers = new List<Speaker>() 
        { 
            new Speaker(){ Alert = "Insha'Allah", Voice = "ar-EG-Hoda", Diction = "", Language=Languages.ar, Accent = "ar-XA" },
            new Speaker(){ Alert = "Schweinsteiger", Voice = "de-DE-Stefan", Diction = "", Language=Languages.de , Accent ="de-DE" },
            new Speaker(){ Alert = "Malaká", Voice = "el-GR-Stefanos", Diction = "", Language=Languages.el, Accent = "el-GR"},
            new Speaker(){ Alert = "Heyyy.", Voice = "en-AU-Catherine", Diction = "", Language=Languages.en, Accent = "en-AU" },
            new Speaker(){ Alert = "A buenas horas mangas verdes", Voice = "es-MX-Raul", Diction = "", Language=Languages.es, Accent = "es-ES"},
            new Speaker(){ Alert = "Thierry Henry", Voice = "fr-FR-Julie", Diction = "" , Language=Languages.fr, Accent = "fr-FR"},
            new Speaker(){ Alert = "Mama mia Marcello.", Voice = "it-IT-Cosimo", Diction = "", Language=Languages.it, Accent = "it-IT"},
            new Speaker(){ Alert = "Nani", Voice = "ja-JP-Ichiro", Diction = "", Language=Languages.ja, Accent = "ja-JP"},
            new Speaker(){ Alert = "Ora Pois", Voice = "pt-PT-HeliaRUS", Diction = "", Language=Languages.pt , Accent = "pt-PT"},
            new Speaker(){ Alert = "Sputinik", Voice = "ru-RU-Irina", Diction = "", Language=Languages.ru, Accent = "ru-RU"},
            new Speaker(){ Alert = "wǒ shì bā xī rén", Voice = "zh-CN-Yaoyao", Diction = "", Language=Languages.zh, Accent = "cmn-CN"},
        };

        public static async Task Speak(string textToSpeech, string user,Languages lang)
        {
            if (!await SpeakerCore.CheckStatus() || config == null) return;

            Speaker spk = Speakers.FirstOrDefault(q => q.Language == lang);

            if (spk != null) await SpeakAzure(spk, textToSpeech, user);
        }


        private static async Task SpeakAzure(ISpeaker speaker, string textToSpeech, string user)
        {
            textToSpeech = textToSpeech.Replace("\"", "\"\"");

            config.SpeechSynthesisVoiceName = speaker.Voice;
            using var synthesizer = new SpeechSynthesizer(config);

            var ssml = File.ReadAllText("Speakers/SSML.xml").Replace("{text}", textToSpeech).Replace("{voice}", speaker.Voice).Replace("{posmsg}", speaker.Diction).Replace("{alert}", speaker.Alert);

            SpeakerCore.PreSpeech(user);

            var result = await synthesizer.SpeakSsmlAsync(ssml);

            await AutomaticTranslator.Translate(textToSpeech);
        }

        public static async Task SpeakTranslate(string cmd, string user)
        {
            string msg;
            Languages? src, trg;
            if (GetLanguages(cmd, out src, out trg, out msg))
            {
                var Target = trg.Value;

                msg = await TranslateCore(msg, false, Target);

                await TasksQueueOutput.QueueAddSpeech(async () => await Speak(msg, user,Target));
            }
        }
    }
}
