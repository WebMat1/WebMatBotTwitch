using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMatBot
{
    public class Translate
    {
        public static int CharacterCounter { get; set; }
        private static int LimitCharacters = 16000; //limite de caracteres diarios.
        private static TranslationClient client = TranslationClient.CreateFromApiKey(Parameters.GoogleTranslateApiKey);

        public static async Task<string> TranslateText(string stringRaw, bool respond)
        {
            string stringToTranslate;
            Languages? src, trg;
            if (!GetLanguages(stringRaw, out src, out trg, out stringToTranslate))
                return stringRaw;

            return await TranslateCore(stringToTranslate, respond, trg.Value, src);
            
        }

        public static async Task<string> TranslateCore(string textToTranslate,bool respond, Languages Trg, Languages? Src = null)
        {
            //checagem de limites
            if (!CheckLimits(textToTranslate)) return "Limite de tradução gratuita diária estourado.";

            var response = await client.TranslateTextAsync(
                text: textToTranslate,
                targetLanguage: Trg.ToString(),  
                sourceLanguage: (Src == null) ? null : Src.ToString());  

            //Console.WriteLine(response.TranslatedText);

            if (respond) await IrcEngine.Respond(response.TranslatedText);

            return response.TranslatedText;
        }

        private static bool CheckLimits(string str)
        {
            CharacterCounter += str.Length;

            return (CharacterCounter <= LimitCharacters);
        }

        public enum Languages
        {
            pt,
            en,
            ru,
            de,
            fr, 
            it, 
            ar, 
            el,
            ja, 
            zh, 
            es,
        }

        public static bool GetLanguages(string stringRaw, out Languages? Source, out Languages? Target, out string Message)
        {
            stringRaw = stringRaw.ToLower().Trim();
            var parts = stringRaw.Split(" ");
            var partsin = parts[0].Split("-");

            string src, trg;

            if (partsin.Length > 1)
            {
                src = partsin[0];
                trg = partsin[1];
            }
            else
            {
                src = null;
                trg = partsin[0];
            }

            Message = "";
            object Src;
            object Trg;

            Enum.TryParse(typeof(Languages), src, out Src);
            Enum.TryParse(typeof(Languages), trg, out Trg);

            if (Trg != null)
            {
                Source = (Languages?)Src;
                Target = (Languages)Trg;

                parts[0] = "";
                Message = string.Join(" ",parts);
                return true;

            }
            else
            {
                Source = null;
                Target = null;
                return false;
            }
        }
    }
}
