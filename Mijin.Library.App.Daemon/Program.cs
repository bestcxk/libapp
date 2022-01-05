using IsUtil.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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


            try
            {
                while (true)
                {
                    Thread.Sleep(10);
                    Process pro = Process.GetProcessesByName("Mijin.Library.App").FirstOrDefault();
                    if (pro == null)
                    {
                        var mPro = Process.Start(@$"Mijin.Library.App.exe");
                        Console.WriteLine("启动成功");
                        mPro.WaitForExit();
                        //Thread.Sleep(2000);
                    }
                    Console.WriteLine("守护进程执行中");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("异常");
                Console.WriteLine(e.ToString());
            }

        }
    }
}
