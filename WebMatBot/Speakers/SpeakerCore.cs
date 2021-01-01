using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebMatBot.Translate;

namespace WebMatBot
{
    public class SpeakerCore
    {
        //public static bool Speaker { get; set; } = false;
        public static Status State { get; set; } = Status.Enabled;

        public static async Task Speak(string textToSpeech, string user,bool wait = true, string speakRate = "0")
        {
            if (!await CheckStatus()) return;

            Sounds.RandomTrollSound();

            PreSpeech(user);

            textToSpeech = textToSpeech.Replace("\"", "\"\"");

            // Command to execute PS  
            ExecutePowerShell($@"Add-Type -AssemblyName System.speech;  
            $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;
            $speak.Rate = {speakRate};
            $speak.Speak(""{textToSpeech}"");"); // Embedd text  

            await AutomaticTranslator.Translate(textToSpeech);
        }

        public static void ExecutePowerShell(string command)
        {
            // create a temp file with .ps1 extension  
            var cFile = System.IO.Path.GetTempPath() + Guid.NewGuid() + ".ps1";

            //Write the .ps1  
            using (var tw = new System.IO.StreamWriter(cFile, false, Encoding.UTF8))
                tw.Write(command);
            

            // Setup the PS  
            var start =
                new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe",
                    LoadUserProfile = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"-executionpolicy bypass -File {cFile}",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                };
            var p = System.Diagnostics.Process.Start(start);
            p.StartInfo.RedirectStandardOutput = true;
            var copyProcess = Process.GetCurrentProcess();
            p.WaitForExit();
        }

        public static void ExecuteMP3File(string path)
        {
            ExecutePowerShell($@"Add-Type -AssemblyName PresentationCore;
            $mediaPlayer = New-Object System.Windows.Media.MediaPlayer;
            do{{
            $mediaPlayer.Open(""{path}"");
            $musicaDuracao = $mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            }}
            until($musicaDuracao)
            $mediaPlayer.Play();
            Start-Sleep -Milliseconds $musicaDuracao;");
        }

        public static void PreSpeech(string user)
        {
            string text = user + " diz: ";

            // Command to execute PS  
            ExecutePowerShell($@"Add-Type -AssemblyName System.speech;  
            $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;
            $speak.Speak(""{text}"");"); // Embedd text  
        }

        public static async Task<bool> CheckStatus()
        {
            if (State == Status.Disabled)
            {
                await IrcEngine.Respond("O Speaker está off... peça o streamer para acioná-lo...");
                return false;
            }
            else
                return true;
        }
        public enum Status
        {
            Disabled,
            Enabled,
            Paused,
        }
    }

    
}
