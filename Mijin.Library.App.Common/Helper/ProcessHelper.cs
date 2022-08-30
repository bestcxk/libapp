using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;


namespace IsUtil.Helper
{
    public partial class ProcessHelper
    {
        /// <summary>
        /// 获取当前程序名，查看是否存在正在运行的程序实例
        /// </summary>
        public static Process CheckSameAppRunProcess()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    return process;
                    //if (process.MainModule.FileName == current.MainModule.FileName)
                    //{
                    //    return process;
                    //}
                }
            }
            return null;
        }

        public static Process StartCmd(params string[] commands)
        {
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            //cmd.StartInfo.RedirectStandardOutput = true;//接受来自调用程序的输入信息
            p.StartInfo.UseShellExecute = false;   //是否使用操作系统shell启动

            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            foreach (var command in commands)
            {
                p.StandardInput.WriteLine(command);
            }

            return p;
        }
    }
}
