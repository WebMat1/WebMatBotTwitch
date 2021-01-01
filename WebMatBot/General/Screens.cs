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

        public static async Task VSCode()
        {
            if (!await CheckStatus())
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F1);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task Browser()
        {

            if (!await CheckStatus())
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F3);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task VS()
        {

            if (!await CheckStatus())
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F2);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return ;
        }
        public static async Task Kitchen()
        {

            if (!await CheckStatus())
                return;

            InputSimulator input = new InputSimulator();
            input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F6);
            input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            return;
        }
        public static async Task Chat()
        {

            if (!await CheckStatus())
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

        private static async Task<bool> CheckStatus()
        {
            if (!isActive)
                await IrcEngine.Respond("A mudança de tela está desativada, peça o streamer para ativá-la.");

            return isActive;
        }

    }
}
