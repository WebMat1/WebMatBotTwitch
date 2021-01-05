using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI;
using YeelightAPI.Models.ColorFlow;

namespace WebMatBot.Lights
{
    public class Light
    {
        public enum Status
        {
            Enabled,
            Disabled
        }

        private static Status _State = Status.Disabled;
        public static Status State { get => _State;
            set
            {
                _State = value;
                if (_State == Status.Enabled)
                    Start();
            }
        }

        public static string ipLight = "192.168.0.14";
        private static Device device { get; set; } 

        public static async Task Start()
        {
            try
            {
                if (State == Status.Enabled)
                {
                    device = new Device(ipLight, autoConnect: false);
                    Console.WriteLine(device.FirmwareVersion);
                    if (device.IsConnected)
                    {
                        await device.TurnOn();
                        await Random("");
                    }
                    else
                        await device.Connect();
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static async Task Random(string user)
        {
            if (!await CheckStatus(user))
                return;

            int r = 0, g = 0, b = 0;

            while (r == 0 && b == 0 && g == 0)
            {
                Random random = new Random();
                r = random.Next(0, 255);
                g = random.Next(0, 255);
                b = random.Next(0, 255);
            }

            await SetRGBColor($"rgb({r},{g},{b})", user);
        }

        public static async Task Command(string cmd, string user)
        {
            if (!await CheckStatus(user))
                return;

            bool founded = false;

            cmd = cmd.Trim().Replace("\r\n", "");
            if (cmd.StartsWith("rgb"))
            {
                await SetRGBColor(cmd, user);
                founded = true;
            }


            switch (cmd)
            {
                case "blue":
                    await SetRGBColor("rgb(0,0,255)", user);
                    founded = true;
                    break;
                case "red":
                    await SetRGBColor("rgb(255,0,0)", user);
                    founded = true;
                    break;
                case "green":
                    await SetRGBColor("rgb(0,255,0)", user);
                    founded = true;
                    break;
                case "yellow":
                    await SetRGBColor("rgb(255,255,0)", user);
                    founded = true;
                    break;
                case "white":
                    await SetRGBColor("rgb(255,255,255)", user);
                    founded = true;
                    break;
                case "pink":
                    await SetRGBColor("rgb(255,0,155)", user);
                    founded = true;
                    break;
                case "orange":
                    await SetRGBColor("rgb(255,155,0)", user);
                    founded = true;
                    break;
                case "purple":
                    await SetRGBColor("rgb(127,0,255)", user);
                    founded = true;
                    break;
                case "random":
                    await Random(user);
                    founded = true;
                    break;
            }

            //não encontrou nenhum comando
            if (!founded)
            {
                await IrcEngine.CommandCorrector(cmd, "!Light",user:user, shouldBeExact: true);
            }
        }

        private static async Task SetRGBColor(string rawSTR, string user)
        {
            try
            {
                if (!device.IsConnected)
                    return;

                //trabalhar a string para remover dados
                string[] rgb = rawSTR.Split("rgb")[1].Replace("(", "").Replace(")", "").Split(",");
                int r = int.Parse(rgb[0]);
                int g = int.Parse(rgb[1]);
                int b = int.Parse(rgb[2]);

                if (r > 255 || r < 0 || g > 255 || g < 0 || b > 255 || b < 0 || (r == 0 && g == 0 && b == 0))
                    throw new Exception("Parametro(s) inválido(s)");

                await device.SetRGBColor(r, g, b);
            }
            catch (Exception ex)
            {
                await IrcEngine.CommandCorrector(rawSTR, "!Light",user: user, shouldBeExact: true);
            }
        }

        public static async Task StartLightFlow(string user)
        {
            if (!await CheckStatus(user))
                return;

            if (device.IsConnected)
            {
                Random rand = new Random();
                ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.Restore);
                flow.Add(new ColorFlowRGBExpression(27, 149, 211, 100, rand.Next(200, 450))); // color : red / brightness : 1% / duration : 500
                flow.Add(new ColorFlowRGBExpression(206, 95, 26, 100, rand.Next(200, 450))); // color : green / brightness : 100% / duration : 500
                flow.Add(new ColorFlowRGBExpression(31, 88, 162, 100, rand.Next(200, 450))); // color : blue / brightness : 50% / duration : 500
                flow.Add(new ColorFlowRGBExpression(213, 122, 25, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(104, 62, 139, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(232, 178, 20, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(163, 65, 141, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(253, 236, 16, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(200, 46, 129, 100, rand.Next(200, 450)));
                flow.Add(new ColorFlowRGBExpression(129, 178, 61, 100, rand.Next(200, 450)));

                await device.StartColorFlow(flow); // start
            }
        }

        public static async Task StartLightFlowAnthem(string user)
        {
            if (!await CheckStatus(user))
                return;

            if (device.IsConnected)
            {
                Random rand = new Random();
                ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.Restore);
                flow.Add(new ColorFlowRGBExpression(1, 0, 173, 100, rand.Next(1300, 5000))); // blue screen of death
                flow.Add(new ColorFlowRGBExpression(118, 185, 0, 100, rand.Next(200, 450))); // nvidia's green
                flow.Add(new ColorFlowRGBExpression(222, 0, 49, 100, rand.Next(200, 450))); // amd's red
                await device.StartColorFlow(flow); // start
            }
        }

        public static async Task StopLightFlow()
        {
            if (device.IsConnected)
            {
                await Light.device.StopColorFlow();
            }
        }

        public static async Task Stop()
        {
            if (device.IsConnected)
                await device.TurnOff();
        }

        public static async Task<bool> CheckStatus(string user)
        {
            if (State == Status.Enabled)
                return true;
            else
                await IrcEngine.Respond("As alterações de luzes estão desabilitadas, por favor peça o streamer para ativa-las...", user);

            return false;
        }
    }
}
