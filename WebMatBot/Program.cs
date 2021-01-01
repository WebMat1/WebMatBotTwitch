using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMatBot.Core;
using WindowsInput;
using static WebMatBotV3.Shared.Entity.Balls;

namespace WebMatBot
{
    public class Program 
    {
        static async Task Main(string[] args)
        {
            await Start();

            // to set new parameters while running
            await ListeningNewSettings();
        }

        public static async Task Start()
        {
            Task.Run(() => IrcEngine.Start()); // run the core of twitch connection in a new thread
            Task.Run(() => TasksQueueOutput.Start());
            Task.Run(() => AutomaticMessages.StartScheduled());
            Task.Run(() => AutomaticMessages.StartLiquid());
            Task.Run(() => Lights.Light.Start());
            Task.Run(() => PubSubEngine.Start());
            Task.Run(() => Games.Cannon_Store.Start());
        }

        private static async Task ListeningNewSettings()
        {
            do
            {
                try 
                {
                    var line = Console.ReadLine();
                    await SendCommand(line);
                }
                catch (Exception except)
                {
                    Console.WriteLine(except.Message);
                }
            } while (true);
        }

        public static async Task<string> SendCommand(string line)
        {

            string result = "Fail";
            if (line.ToLower().Contains("!setproject"))
            {
                Commands.ProjectLink = line.Split(" ")[1];
                result = "Projet link is :" + Commands.ProjectLink;
            }

            if (line.ToLower().Contains("!settetris"))
            {
                Commands.TetrisLink = line.Split(" ")[1];
                result = "Tetris link is: " + Commands.TetrisLink;
            }

            if (line.ToLower().Contains("!setspeaker"))
            {
                line = line.ToLower();
                switch (line.Split(" ")[1])
                {
                    case "pause":
                        SpeakerCore.State = SpeakerCore.Status.Paused;
                        break;
                    case "play":
                    case "true":
                        SpeakerCore.State = SpeakerCore.Status.Enabled;
                        break;
                    case "false":
                        SpeakerCore.State = SpeakerCore.Status.Disabled;
                        break;
                }

                result = "Speaker now is: " + SpeakerCore.State.ToString();
            }

            if (line.ToLower().Contains("!setscreen"))
            {
                switch (line.Split(" ")[1])
                {
                    case "true":
                        Screens.isActive = true;
                        break;
                    case "false":
                        Screens.isActive = false;
                        break;
                }
                result = "Screen Changer now is: " + (Screens.isActive ? "Active" : "Deactivated");
            }

            if (line.ToLower().Contains("!settroll"))
            {
                switch (line.Split(" ")[1])
                {
                    case "true":
                        Sounds.TrollisActive = true;
                        break;
                    case "false":
                        Sounds.TrollisActive = false;
                        break;
                }

                result = "Speaker Troll now is: " + (Sounds.TrollisActive ? "Active" : "Deactivated");
            }

            if (line.ToLower().Contains("!setspeaktime"))
            {
                line = line.ToLower();
                int newTime = TasksQueueOutput.TimeSleeping;

                int.TryParse(line.Split(" ")[1], out newTime);

                TasksQueueOutput.TimeSleeping = newTime;

                result = "Speaker now has time delay: " + TasksQueueOutput.TimeSleeping.ToString() + " seconds";
            }

            if (line.ToLower().Contains("!setcannon"))
            {
                switch (line.Split(" ")[1])
                {
                    case "true":
                        Games.Cannon.State = Games.Cannon.Status.Enabled;
                        break;
                    case "false":
                        Games.Cannon.State = Games.Cannon.Status.Disabled;
                        break;
                }

                result = "Cannon now is: " + (Games.Cannon.State == Games.Cannon.Status.Enabled ? "Active" : "Deactivated");
            }

            if (line.ToLower().Contains("!setiplight"))
            {
                Lights.Light.ipLight = (line.Split(" ")[1]);

                result = "Ip Light now is: " + Lights.Light.ipLight;
            }

            if (line.ToLower().Contains("!addcannon"))
            {
                var parameters = line.Split("!AddCannon")[0].Trim().Split(" ");

                if (parameters.Length == 4)
                {
                    var user = parameters[1];
                    var args = parameters[2] + " " + parameters[3];

                    await Games.Cannon.NewRedemptionGame(user, args);

                    result = "Added [" + user + "] Balls:" + parameters[2] + " and JackPot:" + parameters[3];
                }
                else result = "Fail";
            }
           
            if (line.ToLower().Contains("!cannonlist"))
            {
                await Games.Cannon.GenerateList();
            }

            if (line.ToLower().Contains("!cannonstore"))
            {
                Task.Run(() => Games.Cannon_Store.OpenMarketCommand());
            }

            if (line.ToLower().Contains("!cannonstopstore"))
            {
                Games.Cannon_Store.CloseMarketCommand();
            }

            if (line.ToLower().Contains("!addskin"))
            {
                var parameters = line.Split("!addskin")[1].Trim().Split(" ");

                if (parameters.Length == 2)
                {
                    var user = parameters[0];
                    var value = int.Parse(parameters[1]);

                    await Games.Cannon_Store.BuyBallCommand(user, value, Games.Cannon_Store.TypePayment.bits);
                }
                else result = "Fail parameters";
            }

            await WebMatBot.IrcEngine.Analizer("badges=broadcaster;user-type= :" + Parameters.User + "!" + Parameters.User + "@" + Parameters.User + ".tmi.twitch.tv PRIVMSG " + line);

            //Console.WriteLine(result);
            return result;
        }
    }
}