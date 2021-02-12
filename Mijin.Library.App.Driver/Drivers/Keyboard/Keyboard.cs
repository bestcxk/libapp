using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public class Keyboard : IKeyboard
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

        /// <summary>
        /// 打开屏幕键盘
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Open()
        {
            var result = new MessageModel<bool>();
            try
            {
                Process.Start(@"C:\WINDOWS\system32\osk.exe");
                IntPtr intptr = IntPtr.Zero;
                while (IntPtr.Zero == intptr)
                {
                    System.Threading.Thread.Sleep(100);
                    intptr = FindWindow(null, "屏幕键盘");
                }

                //int keyWidth = (int)(Width * 0.8);
                //int keyHeight = (int)(keyWidth * 0.33);

                //// 设置软键盘的显示位置，底部居中
                //int posX = (Width - keyWidth) / 2;
                //int posY = (Width - keyHeight);

                ////设定键盘显示位置
                //MoveWindow(intptr, posX, posY, keyWidth, keyHeight, true);

                //设置软键盘到前端显示
                //SetForegroundWindow(intptr);

                result.success = true;
            }
            catch (Exception e)
            {
                result.success = false;
                result.devMsg = e.ToString();
            }
            result.msg = "打开屏幕键盘" + (result.success ? "成功" : "失败");
            return result;

        }

        /// <summary>
        /// 关闭屏幕键盘
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Close()
        {
            var result = new MessageModel<bool>();
            foreach (Process p in System.Diagnostics.Process.GetProcessesByName("osk"))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit();
                    result.success = true;
                }
                catch (Exception exp)
                {
                    result.success = false;
                    result.devMsg = exp.ToString();
                }
            }
            result.msg = "关闭屏幕键盘" + (result.success ? "成功" : "失败");
            return result;
        }
    }
}
