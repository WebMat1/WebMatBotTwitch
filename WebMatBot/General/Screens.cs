using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace WebMatBot
{
    public class Screens
    {
        public static bool isActive { get; set; } = false; //inactivated

        public static async Task VSCode(string user)
        {
            if (!await CheckStatus(user))
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F1);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task Browser(string user)
        {

            if (!await CheckStatus(user))
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F3);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task VS(string user)
        {

            if (!await CheckStatus(user))
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F2);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task Kitchen(string user)
        {

            if (!await CheckStatus(user))
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F6);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return;
        }
        public static async Task Chat(string user)
        {

            if (!await CheckStatus(user))
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F7);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return;
        }
        public static async Task Cannon()
        {
            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F12);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
        }

        private static async Task<bool> CheckStatus(string user)
        {
            if (!isActive)
                await IrcEngine.Respond("A mudança de tela está desativada, peça o streamer para ativá-la.", user);

            return isActive;
        }

    }
}
