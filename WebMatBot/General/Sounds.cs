using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMatBot
{
    public class Sounds
    {
        public static bool TrollisActive { get; set; } = false;

        public static void RandomTrollSound()
        {
            if (!CheckStatus())
                return;

            Random rdm = new Random();
            var files = GetTrollFiles();

            var index = rdm.Next(files.Length);

            SpeakerCore.ExecuteMP3File(files[index]);
            //Task.Delay(500);
        }

        private static bool CheckStatus()
        {
            return TrollisActive;
        }

        private static string[] GetTrollFiles()
        {
            var targetDirectory = @Directory.GetCurrentDirectory() + @"\Sounds\Troll";

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            return fileEntries;
        }

        private static string[] GetGeneralFiles()
        {
            var targetDirectory = @Directory.GetCurrentDirectory() + @"\Sounds\General";

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            return fileEntries;
        }

        private static string[] GetPartyFiles()
        {
            var targetDirectory = @Directory.GetCurrentDirectory() + @"\Sounds\Party";

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            return fileEntries;
        }


        public static void Piada()
        {
            var files = GetGeneralFiles();
            var piada = files.Where(q=>q.Contains("rimshot.mp3")).FirstOrDefault();
            if (piada != null) SpeakerCore.ExecuteMP3File(piada);
        }

        public static void Clap()
        {
            var files = GetGeneralFiles();
            var piada = files.Where(q => q.Contains("aplausos.mp3")).FirstOrDefault();
            if (piada != null) SpeakerCore.ExecuteMP3File(piada);
        }

        public static void Xandao()
        {
            var files = GetGeneralFiles();
            var piada = files.Where(q => q.Contains("xandao.mp3")).FirstOrDefault();
            if (piada != null) SpeakerCore.ExecuteMP3File(piada);
        }

        public static void RandomPartySound()
        {
            Random rdm = new Random();
            var files = GetPartyFiles();

            var index = rdm.Next(files.Length);

            SpeakerCore.ExecuteMP3File(files[index]);
        }

    }
}
