using Google.Apis.Http;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatBot.Core;
using static WebMatBot.Translate;

namespace WebMatBot
{
    public class GoogleSpeakers
    {

        private static IList<Speaker> Speakers = new List<Speaker>()
        {
            new Speaker(){ Alert = "Insha'Allah?", Voice = "ar-XA-Standard-D", Diction = "", Language=Languages.ar , Accent = "ar-XA" },
            new Speaker(){ Alert = "Schweinsteiger?", Voice = "de-DE-Standard-F", Diction = "", Language=Languages.de , Accent ="de-DE"},
            new Speaker(){ Alert = "Malaká?", Voice = "el-GR-Standard-A", Diction = "", Language=Languages.el , Accent = "el-GR"},
            new Speaker(){ Alert = "Hey?", Voice = "en-US-Standard-C", Diction = "", Language=Languages.en , Accent = "en-AU"},
            new Speaker(){ Alert = "¿A buenas horas mangas verdes?", Voice = "es-ES-Standard-A", Diction = "", Language=Languages.es , Accent = "es-ES"},
            new Speaker(){ Alert = "Thierry Henry?", Voice = "fr-FR-Standard-E", Diction = "" , Language=Languages.fr , Accent = "fr-FR"},
            new Speaker(){ Alert = "Mama mia Marcello?", Voice = "it-IT-Standard-D", Diction = "", Language=Languages.it, Accent = "it-IT" },
            new Speaker(){ Alert = "Nani?", Voice = "ja-JP-Standard-A", Diction = "", Language=Languages.ja, Accent = "ja-JP" },
            new Speaker(){ Alert = "Ora Pois?", Voice = "pt-PT-Standard-C", Diction = "", Language=Languages.pt, Accent = "pt-PT" },
            new Speaker(){ Alert = "Sputinik?", Voice = "ru-RU-Standard-E", Diction = "", Language=Languages.ru , Accent = "ru-RU"},
            new Speaker(){ Alert = "wǒ shì bā xī rén?", Voice = "cmn-CN-Standard-D", Diction = "", Language=Languages.zh , Accent = "cmn-CN"},
        };


        public static async Task Speak(string textToSpeech, string user,Languages lang)
        {
            if (!await SpeakerCore.CheckStatus(user) || Parameters.GoogleTranslateApiKey == null) return;

            Speaker spk = Speakers.FirstOrDefault(q => q.Language == lang);

            if (spk != null) await SpeakGoogle(spk, textToSpeech, user);
        }

        private static async Task SpeakGoogle(ISpeaker speaker, string textToSpeech, string user)
        {
            textToSpeech = textToSpeech.Replace("\"", "\"\"");

            // Instantiate a client
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Set the text input to be synthesized.
            SynthesisInput input = new SynthesisInput
            {
            //Text = textToSpeech,
                Ssml = File.ReadAllText("Speakers/SSML.xml").Replace("{text}", textToSpeech).Replace("{voice}", speaker.Voice).Replace("{posmsg}", speaker.Diction).Replace("{alert}", speaker.Alert),
            };

            // Build the voice request, select the language code ("en-US"),
            // and the SSML voice gender ("neutral").
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = speaker.Accent.ToString(),
                //SsmlGender = SsmlVoiceGender.Neutral
            };

            // Select the type of audio file you want returned.
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            // Perform the Text-to-Speech request, passing the text input
            // with the selected voice parameters and audio file type
            var response = await client.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            // create a temp file with .ps1 extension  
            var cFile = System.IO.Path.GetTempPath() + Guid.NewGuid() + ".mp3";

            // Write the binary AudioContent of the response to an MP3 file.
            using (Stream output = File.Create(cFile))
                response.AudioContent.WriteTo(output);

            Sounds.RandomTrollSound();

            SpeakerCore.PreSpeech(user);

            SpeakerCore.ExecuteMP3File(cFile);

            await AutomaticTranslator.Translate(textToSpeech, user);

        }

        public static async Task SpeakTranslate(string cmd, string user)
        {
            string msg;
            Languages? src, trg;
            if (GetLanguages(cmd, out src, out trg, out msg))
            {
                var Target = trg.Value;

                msg = await TranslateCore(msg, false, Target, user);

                await TasksQueueOutput.QueueAddSpeech(async () => await Speak(msg, user,Target), user);
            }
            else
            {
                await IrcEngine.CommandCorrector(cmd,"!SpeakTranslate",user: user, shouldBeExact:true);
            }
        }

    }
}
