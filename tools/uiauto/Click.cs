// 按屏幕坐标点一下鼠标 —— 给 tools\selftest-*.ps1 驱动向导用。
//
// 为什么需要它：AntdUI 的按钮不是标准 WinForms Button，UI Automation 里看不到
// （ControlType 不是 Button，也没有 InvokePattern），只能按坐标点。
//
// 为什么是独立工程：本机 Add-Type -TypeDefinition 与 csc.exe 都报
// "cannot seek in file"，只有 dotnet build 能用。
//
// 用法：click.exe <x> <y>
using System;
using System.Runtime.InteropServices;

namespace UiAuto
{
    internal static class Click
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extra);

        private const uint LeftDown = 0x0002;
        private const uint LeftUp = 0x0004;

        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("用法：click.exe <x> <y>");
                return 2;
            }

            int x, y;
            if (!int.TryParse(args[0], out x) || !int.TryParse(args[1], out y))
            {
                Console.Error.WriteLine("坐标必须是整数");
                return 2;
            }

            SetCursorPos(x, y);
            System.Threading.Thread.Sleep(80);
            mouse_event(LeftDown, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(40);
            mouse_event(LeftUp, 0, 0, 0, UIntPtr.Zero);

            Console.WriteLine("clicked " + x + "," + y);
            return 0;
        }
    }
}
