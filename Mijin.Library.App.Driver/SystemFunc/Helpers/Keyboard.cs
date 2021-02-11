using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.src.SystemFunc.Helpers
{
    /// <summary>
    /// 使用前需要设置屏幕宽高
    /// </summary>
    public class Keyboard
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        private static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // 屏幕尺寸
        public static int Height { get; set; } = 1080;
        public static int Width { get; set; } = 1920;

        public static object Open()
        {
            try
            {
                Process.Start(@"C:\WINDOWS\system32\osk.exe");
                IntPtr intptr = IntPtr.Zero;
                while (IntPtr.Zero == intptr)
                {
                    System.Threading.Thread.Sleep(100);
                    intptr = FindWindow(null, "屏幕键盘");
                }

                int keyWidth = (int)(Width * 0.8);
                int keyHeight = (int)(keyWidth * 0.33);

                // 设置软键盘的显示位置，底部居中
                int posX = (Width - keyWidth) / 2;
                int posY = (Width - keyHeight);

                //设定键盘显示位置
                MoveWindow(intptr, posX, posY, keyWidth, keyHeight, true);

                //设置软键盘到前端显示
                //SetForegroundWindow(intptr);

                return true;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static void Close()
        {
            foreach (Process p in System.Diagnostics.Process.GetProcessesByName("osk"))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit();
                }
                catch (Exception exp)
                {

                }
            }
        }
    }
}
