using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Filters;
using System;
using System.Windows;
using Util.Logs.Extensions;
using Microsoft.Extensions.Configuration;
using Mijin.Library.App.Views;
using Mijin.Library.App.Driver.Extentions;
using System.Windows.Media;
using System.Drawing;
using Mijin.Library.App.Model;
using Mijin.Library.App.Driver;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Mijin.Library.Core.Common.Helper;
using System.Linq;

namespace Mijin.Library.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceCollection _serviceCollection { get; }

        public IConfiguration Configuration { get; }

        public IServiceProvider _serviceProvider { get; }


        public App()
        {
            // 程序启动检测
            this.BeforeStart();

            _serviceCollection = new ServiceCollection();

            ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 注册主window
            services.AddSingleton<MainWindow>();
            services.AddSingleton<WebViewWindow>();
            services.AddSingleton<SettingsWindow>();
            // 注册Nlog
            services.AddNLog();
            // 注册MemoryCache
            services.AddMemoryCache();

            // 注册Driver
            services.AddDriver();

            // 注册客户端设置类
            //services.AddSingleton<ClientSettings>();
        }

        //public void Configure(IApplicationBuilder app)
        //{ 
        //     app.UseSignalR()
        //}

        // 使用了ioc后，只能使用该方式进行启动,把app.xaml的StartupUrl 修改成 Startup = App_OnStartup
        private void App_OnStartup(object sender, StartupEventArgs e)
        {

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            var webviewWindow = _serviceProvider.GetService<WebViewWindow>();
            var settings = _serviceProvider.GetService<ISystemFunc>().ClientSettings;

            WebViewWindow._webViewWindow = webviewWindow;

            // 全局异常处理
            this.DispatcherUnhandledException += GlobalExceptionsFilter.OnException;
            //AppDomain.CurrentDomain.UnhandledException += TestException;
            ////Task线程内未捕获异常处理事件
            //TaskScheduler.UnobservedTaskException += TestException;

            // 其中一个窗口触发了关闭事件则直接退出全部程序
            mainWindow.Closed += (s, e) =>
            {
                // 退出整个应用
                Environment.Exit(0);
            };
            //webviewWindow.Closed += ExitApplication;

            // 不可被关闭
            if (settings.CannotClosed)
            {
                Process.Start(@$"Mijin.Library.App.Daemon.exe");
                Console.WriteLine("启动守护进程");
            }
            else // 可以被关闭 
            {
                Process pro = Process.GetProcessesByName("Mijin.Library.App.Daemon").FirstOrDefault();
                pro?.Kill();
            }

            // 是否直接打开webview
            if (!string.IsNullOrEmpty(settings.NoSelectOpenUrl))
            {
                webviewWindow.webView.Source = new Uri(settings.NoSelectOpenUrl);
                webviewWindow.Show();
            }
            else
            {
                mainWindow.Show();
            }
        }


        // 启动程序前检测
        private void BeforeStart()
        {
            #region 检测是否管理员启动
            //var isAdmin = IsAdministrator();
            //if (!isAdmin)
            //{
            //    var res = MessageBox.Show("请以管理员权限运行！");
            //    // 退出整个应用
            //    Environment.Exit(0);
            //}
            #endregion

            #region 检测程序是否已启动
            var prs = ProcessHelper.CheckSameAppRunProcess();
            if (prs != null)
            {
                var result = MessageBox.Show("已有一个程序实例运行，是否要关闭所有实例", "提示", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        prs.Kill();
                        prs.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        // 需要在注册Nlog后启动，否则就注释掉
                        //ex.Log(Log.GetLog().Caption("BeforeStart"));
                    }
                }
                MessageBox.Show("关闭成功，请重新启动", "提示", MessageBoxButton.OK);
                Environment.Exit(0);

            }
            #endregion

            #region 检查是否安装了webview2
            string version = null;
            try
            {
                // 注册表查询webview2 runtime 版本
                version = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}")?.GetValue("pv")?.ToString();
            }
            catch (Exception)
            {
                version = null;
            }

            // 未安装wenview2 runtime
            if (version == null)
            {
                var result = MessageBox.Show("检测到未安装必要环境，不安装将导致应用无法正常运行，是否现在开始安装？", "提示", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    // 本地安装包路径
                    string path = @"./DependInstaller/MicrosoftEdgeWebview2Setup.exe";

                    // 如果文件不存在，则直接默认浏览器打开网页提示下载
                    if (File.Exists(path))
                    {
                        Process.Start(path);
                    }
                    else
                    {
                        result = MessageBox.Show("未找到本地安装包，将打开指定网页进行下载，请选择 “常青版引导程序”，是否现在下载？", "提示", MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            //调用系统默认的浏览器   
                            Process.Start("explorer.exe", "https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/#download-section");
                            //Task.Delay(200).GetAwaiter().GetResult();
                        }

                    }
                }
                // 退出整个应用
                Environment.Exit(0);
            }

            #endregion
        }

        /// <summary>
        /// 是否管理员权限运行
        /// </summary>
        /// <returns></returns>
        private bool IsAdministrator()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
