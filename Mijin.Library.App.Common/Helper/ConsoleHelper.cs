using System;

namespace Util.Helpers
{
    public static class ConsoleHelper
    {
        static void Write(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = currentForeColor;
        }

        /// <summary>
        /// 打印错误信息
        /// </summary>
        /// <param name="str">待打印的字符串</param>
        /// <param name="color">想要打印的颜色</param>
        public static void ErrorLine(this string str, ConsoleColor color = ConsoleColor.Red)
        {
            Write(str, color);
        }

        /// <summary>
        /// 打印警告信息
        /// </summary>
        /// <param name="str">待打印的字符串</param>
        /// <param name="color">想要打印的颜色</param>
        public static void WarningLine(this string str, ConsoleColor color = ConsoleColor.Yellow)
        {
            Write(str, color);
        }
        /// <summary>
        /// 打印正常信息
        /// </summary>
        /// <param name="str">待打印的字符串</param>
        /// <param name="color">想要打印的颜色</param>
        public static void InfoLine(this string str, ConsoleColor color = ConsoleColor.White)
        {
            Write(str, color);
        }
        /// <summary>
        /// 打印成功的信息
        /// </summary>
        /// <param name="str">待打印的字符串</param>
        /// <param name="color">想要打印的颜色</param>
        public static void SuccessLine(this string str, ConsoleColor color = ConsoleColor.Green)
        {
            Write(str, color);
        }

    }
}
