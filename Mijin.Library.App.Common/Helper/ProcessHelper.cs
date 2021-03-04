using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;


namespace Mijin.Library.Core.Common.Helper
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
                    if (process.MainModule.FileName == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
    }
}
