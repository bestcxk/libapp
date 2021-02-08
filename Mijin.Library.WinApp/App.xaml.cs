﻿using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Common.Helper;
using Mijin.Library.App.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Util.Helpers;
using Util.Logs;
using Util.Logs.Extensions;

namespace Mijin.Library.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceCollection _serviceCollection { get; }
        public IServiceProvider _serviceProvider { get; }

        public App()
        {
            _serviceCollection = new ServiceCollection();
            ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 注册主window
            services.AddSingleton<MainWindow>();
            // 注册Nlog
            services.AddNLog();
            // 注册MemoryCache
            services.AddMemoryCache();
        }

        // 使用了ioc后，只能使用该方式进行启动,把app.xaml的StartupUrl 修改成 Startup = App_OnStartup
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            // 启动窗口前检测
            this.BeforeStart();

            // 全局异常处理
            this.DispatcherUnhandledException += GlobalExceptionsFilter.OnException;
            //AppDomain.CurrentDomain.UnhandledException += TestException;
            ////Task线程内未捕获异常处理事件
            //TaskScheduler.UnobservedTaskException += TestException;

            mainWindow.Show();
        }

        // 启动程序前检测
        private void BeforeStart()
        {
            #region 检测程序是否已启动
            var prs = ProcessHelper.CheckSameAppRunProcess();
            if (prs != null)
            {
                var result = MessageBox.Show("已有一个程序实例运行，是否要关闭已经在运行的实例并启动新的实例", "提示", MessageBoxButton.YesNoCancel);

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
            }
            #endregion
        }

    }
}
