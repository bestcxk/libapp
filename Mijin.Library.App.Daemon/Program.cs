using Mijin.Library.Core.Common.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mijin.Library.App.Daemon
{
    /// <summary>
    /// wpf 守护进程
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // 若守护程序已经在运行，则不启动
            if (ProcessHelper.CheckSameAppRunProcess() != null)
            {
                return;
            }

            while (true)
            {
                Task.Delay(500).GetAwaiter().GetResult();
                Process pro = Process.GetProcessesByName("Mijin.Library.App").FirstOrDefault();
                if (pro == null)
                {
                    Process.Start(@$"Mijin.Library.App.exe");
                    Console.WriteLine("启动成功");
                }
                Console.WriteLine("守护进程执行中");
            }
        }
    }
}
