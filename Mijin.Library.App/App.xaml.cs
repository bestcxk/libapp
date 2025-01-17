﻿using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Filters;
using System;
using System.Collections.Generic;
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
using System.Linq;
using IsUtil.Helper;
using Util.Logs;
using Mijin.Library.App.Setting;
using Prism.DryIoc;
using Prism.Ioc;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Bing.IO;
using System.Text;
using Mijin.Library.App.Authorization;
using Mijin.Library.App.Common.EncryptApplocation;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Bing.Extensions;
using Bing.Helpers;
using Exceptionless.Utility;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http.Extensions;
using Mijin.Library.App.Common.Domain;
using Mijin.Library.App.Driver.Services.Network;
using Newtonsoft.Json;

namespace Mijin.Library.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public IServiceCollection _serviceCollection { get; } = new ServiceCollection();

        public EncryptApp app { get; set; } = new EncryptApp();

        public App()
        {
            // 程序启动检测
            this.BeforeStart();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainWindow>();
            containerRegistry.RegisterSingleton<WebViewWindow>();
            containerRegistry.RegisterSingleton<SettingWindow>();
            containerRegistry.RegisterSingleton<AuthWindow>();
        }


        protected override void OnInitialized()
        {
#if !DEBUG
            if (app.IsAuth())
                App_OnStartup();
#else
            App_OnStartup();
#endif

            base.OnInitialized();
        }

        protected override Window CreateShell()
        {
            // 非开发环境
#if (!DEBUG)
            if (!app.IsAuth())
                return Container.Resolve<AuthWindow>();
            else
                return Container.Resolve<MainWindow>();
#else
            var res = Container.Resolve<MainWindow>();
            return res;
#endif
        }

        protected override IContainerExtension CreateContainerExtension()
        {

            ConfigureServices(_serviceCollection);
            var res = new DryIocContainerExtension(new Container(CreateContainerRules())
                .WithDependencyInjectionAdapter(_serviceCollection));

            return res;

        }

        private void ConfigureServices(IServiceCollection services)
        {

            //services.AddSingleton<MainWindow>();
            //services.AddSingleton<WebViewWindow>();
            //services.AddSingleton<SettingWindow>();
            //services.AddSingleton<AuthWindow>();

            // 注册Nlog
            services.AddNLog();
            // 注册MemoryCache
            services.AddMemoryCache();

            // 注册Driver
            services.AddDriver();

        }

        // 使用了ioc后，只能使用该方式进行启动,把app.xaml的StartupUrl 修改成 Startup = App_OnStartup
        private void App_OnStartup()
        {

            Task.Run(() =>
            {
                #region Test
                {

                    //try
                    //{
                    //    Console.WriteLine("Start Test");
                    //    var tuchang = new TuChuangSIP2Client();

                    //    var connectRes = tuchang.Connect("172.16.0.11", "60002",true);
                    //    Console.WriteLine(JsonConvert.SerializeObject(connectRes));

                    //    var res = tuchang.AdminLogin("zzjh1", "ilas");
                    //      Console.WriteLine(JsonConvert.SerializeObject(res));

                    //    res = tuchang.GetBookInfo("36080000229434");

                    //    Console.WriteLine(JsonConvert.SerializeObject(res));

                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}



                }
                #endregion
            });





            var mainWindow = Container.Resolve<MainWindow>();
            var webviewWindow = Container.Resolve<WebViewWindow>();
            try
            {
                Container.Resolve<ICamera>().GetOne();

            }
            catch (Exception e)
            {
            }
            var settings = Container.Resolve<ISystemFunc>().ClientSettings;

            WebViewWindow._webViewWindow = webviewWindow;

            // 全局异常处理
            this.DispatcherUnhandledException += GlobalExceptionsFilter.OnException;

            var clseEvent = () =>
            {
                if (!mainWindow.IsVisible && !webviewWindow.IsVisible &&
                    WebViewWindow._doorViewWindow?.IsVisible != true)
                {
                    AppStatic.CloseRfid();
                    // 退出整个应用
                    Environment.Exit(0);
                }
            };

            // 其中一个窗口触发了关闭事件则直接退出全部程序
            mainWindow.Closed += (s, e) => { clseEvent.Invoke(); };

            // 其中一个窗口触发了关闭事件则直接退出全部程序
            webviewWindow.Closed += (s, e) => { clseEvent.Invoke(); };

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
                webviewWindow.OpenUrl = settings.NoSelectOpenUrl;
                webviewWindow.Show();
            }
            else
            {
                mainWindow.Show();
            }

            // 直接打开门禁页面
            if (settings.DoorControllerStart)
            {
                webviewWindow.ShowDoorViewBtn(null, null);
            }

            if (!settings.DisibleProxy)
                Container.Resolve<INetWorkTranspondService>().StartOrUpdateListen(settings);
            var settingWindow = Container.Resolve<SettingWindow>();
            settingWindow.OnSettingsChange += async () =>
            {
                var network = Container.Resolve<INetWorkTranspondService>();
                var settings = new ClientSettings();
                if (!settings.DisibleProxy)
                    await network
                        .StartOrUpdateListen(Container.Resolve<ISystemFunc>().ClientSettings);
                else
                {
                    await network.ClearAllListen();
                }
            };
        }

        /// <summary>
        /// 启动程序前检测
        /// </summary>
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
                version = Registry.LocalMachine
                    .OpenSubKey(
                        @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}")
                    ?.GetValue("pv")?.ToString();
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
                        result = MessageBox.Show("未找到本地安装包，将打开指定网页进行下载，请选择 “常青版引导程序”，是否现在下载？", "提示",
                            MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            //调用系统默认的浏览器   
                            Process.Start("explorer.exe",
                                "https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/#download-section");
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
            System.Security.Principal.WindowsPrincipal principal =
                new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}