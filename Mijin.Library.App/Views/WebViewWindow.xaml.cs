﻿using Microsoft.Web.WebView2.Core;
using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mijin.Library.App.Model;
using IsUtil.Helpers;
using Util.Logs.Extensions;
using Util.Logs;
using IsUtil.Maps;
using MahApps.Metro.Controls;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using IsUtil;
using System.Reflection;
using System.Windows.Input;
using Bing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Driver.Services.Network;
using Enum = IsUtil.Helpers.Enum;

namespace Mijin.Library.App.Views
{
    /// <summary>
    /// WebViewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebViewWindow : MetroWindow
    {
        private readonly IDriverHandle _driverHandle;
        private readonly IServiceProvider _serviceProvider;
        private readonly INetWorkTranspondService _netWorkTranspondService;
        private readonly ClientSettings _clientSettings;
        public static WebViewWindow _doorViewWindow { get; set; } = null;
        public static WebViewWindow _webViewWindow { get; set; } = null;
        public ISystemFunc _systemFunc { get; }


        bool? isDoorWindow = null;


        private string openUrl;

        public string OpenUrl
        {
            get
            {
                var url = _netWorkTranspondService.GetVisitUrl(openUrl);
                return url;
            }
            set => openUrl = value;
        }

        static private bool hasRegisterEvent = false;

        static private string logColor = "32"; // 32 或者 34

        #region 构造函数

        public WebViewWindow(IDriverHandle driverHandle, ISystemFunc systemFunc, IServiceProvider serviceProvider,
            INetWorkTranspondService netWorkTranspondService)
        {
            _driverHandle = driverHandle;
            _netWorkTranspondService = netWorkTranspondService;
            _systemFunc = systemFunc;
            _serviceProvider = serviceProvider;
            _clientSettings = systemFunc.ClientSettings;
            InitializeComponent();
            InitializeAsync(); // 初始化

            AppStatic.Services = serviceProvider;

            this.versionLabel.Content = @$"版本号：{Assembly.GetExecutingAssembly().GetName().Version}";

            Title = _clientSettings.Title?.App ?? "图书管理系统";
            DisableWPFTabletSupport();

            //this.Activated += WebViewWindow_Activated;


            //this.WindowState = WindowState.Normal;

            this.StateChanged += (s, e) =>
            {
                Send(new WebViewSendModel<string>()
                {
                    success = true,
                    response = Enum.GetName<WindowState>(this.WindowState),
                    method = "OnWindowStateChanged",
                    status = isDoorWindow == true ? 1001 : 200

                }, true);
            };
        }

        #endregion

        ~WebViewWindow()
        {
        }

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        async void InitializeAsync()
        {
            // 窗口关闭时
            this.Closed += async (s, e) =>
            {
                AppStatic.CloseRfid();

                #region 退出时清空浏览器缓存

                try
                {
                    if ((!_doorViewWindow.IsNull() && _doorViewWindow.IsVisible) || _webViewWindow.IsVisible)
                    {
                        return;
                    }

                    // 获取webview 进程id 并杀掉，否则无法删除文件
                    if (_clientSettings.OnExitClearWebCache != ClearWebViewCacheModeEnum.Default)
                    {
                        var process = Process.GetProcessById((int)this.webView.CoreWebView2.BrowserProcessId);
                        process.Kill();
                        process.WaitForExit();
                    }

                    //等待500毫秒等待文件被释放
                    await Task.Delay(500);
                    if (true) // 最多等待3秒
                    {
                        switch (_clientSettings.OnExitClearWebCache)
                        {
                            case ClearWebViewCacheModeEnum.Cookie: // 清空cookie
                                this.webView.CoreWebView2.CookieManager.DeleteAllCookies();
                                break;
                            case ClearWebViewCacheModeEnum.LocalState: // 清空LocalState
                                {
                                    var dirPath = @"./Mijin.Library.App.exe.WebView2/EBWebView/Default/Local Storage";
                                    var FilePath = @"./Mijin.Library.App.exe.WebView2/EBWebView/Local Storage";

                                    if (Directory.Exists(dirPath))
                                    {
                                        FileHelper.DeleteFolder(dirPath);
                                        FileHelper.FileDel(FilePath);
                                    }
                                }
                                break;
                            case ClearWebViewCacheModeEnum.DefaultCache: // 删除webview 下的Default文件
                                {
                                    var dirPath = @"./Mijin.Library.App.exe.WebView2/EBWebView/Default";
                                    if (Directory.Exists(dirPath))
                                    {
                                        FileHelper.DeleteFolder(dirPath);
                                    }
                                }
                                break;
                            case ClearWebViewCacheModeEnum.AllCache: // 删除整个webview 的Cache文件
                                {
                                    var dirPath = @"Mijin.Library.App.exe.WebView2";
                                    if (Directory.Exists(dirPath))
                                    {
                                        FileHelper.DeleteFolder(dirPath);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Log(Log.GetLog().Caption("退出时清空浏览器缓存"));
                }

                //ReStartApp();

                #endregion

                // 退出整个应用
                // Environment.Exit(0);
            };

            try
            {
                await this.webView.EnsureCoreWebView2Async(null);
            }
            catch (Exception)
            {
                MessageBox.Show("Webview2 初始化失败！，检测Webview2运行时是否安装正确", "错误");
                Environment.Exit(0);
            }
            this.webView.CoreWebView2.WebMessageReceived += WebMessageReceived;
            #region 窗口设置

            // 不可调整窗口宽高
            if (!_clientSettings.CanResize)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
            }

            // 标题栏设置
            if (!_clientSettings.ShowWindowTitleBar)
            {
                this.ShowTitleBar = false;
                //this.WindowState = System.Windows.WindowState.Normal;
                //this.WindowStyle = System.Windows.WindowStyle.None;
            }
            else
            {
                // 不显示标题栏
                this.WindowState = System.Windows.WindowState.Normal; //还原窗口（非最小化和最大化）
                this.WindowStyle = System.Windows.WindowStyle.ThreeDBorderWindow; //仅工作区可见，不显示标题栏和边框
            }

            //窗口顶置
            if (_clientSettings.WindowOverhead)
            {
                this.Topmost = true;
            }

            // 全屏
            if (_clientSettings.WindowWidth == 0 && _clientSettings.WindowHeight == 0)
            {
                // 显示标题栏
                if (_clientSettings.ShowWindowTitleBar)
                {
                    this.ShowTitleBar = true; // 不显示标题栏
                    this.WindowState = WindowState.Maximized;
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                else // 不显示标题栏
                {
                    this.WindowState = WindowState.Maximized;
                    this.WindowState = WindowState.Normal; //还原窗口（非最小化和最大化）
                    this.WindowStyle = WindowStyle.None; //仅工作区可见，不显示标题栏和边框
                    this.ResizeMode = ResizeMode.NoResize; //不显示最大化和最小化按钮
                    this.ShowTitleBar = false; // 不显示标题栏


                    this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                    this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                    this.Left = 0;
                    this.Top = 0;

                }
            }
            else
            {
                this.Width = _clientSettings.WindowWidth;
                this.Height = _clientSettings.WindowHeight;

                // 窗口显示在屏幕中央
                Rect workArea = SystemParameters.WorkArea;
                Left = (workArea.Width - this.Width) / 2 + workArea.Left;
                Top = (workArea.Height - this.Height) / 2 + workArea.Top;
            }

            // 显示到最前端
            //this.Topmost = true;

            #endregion

            // 注册事件
            if (!hasRegisterEvent)
            {
                hasRegisterEvent = true;
                _driverHandle.OnDriverEvent += OnDriverEvent;
            }

            // 非 开发 模式
            if (!_clientSettings.IsDev)
            {
                // 使能 dev tools
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                // 取消右键菜单
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            }


            // 直达webView模式
            if (!string.IsNullOrEmpty(_clientSettings.NoSelectOpenUrl))
            {
                OpenUrl = _clientSettings.NoSelectOpenUrl;
            }



            #region 当前路由处理

            if (!string.IsNullOrWhiteSpace(OpenUrl) && OpenUrl.Substring(0, 2) == @$".\")
            {
                this.OpenUrl =
                    @$"{Path.Combine(Environment.CurrentDirectory, OpenUrl.Substring(2)).Replace(@$"\", @$"/")}";
            }

            this.webView.Source = new Uri(this.OpenUrl);

            #endregion


            if (!_clientSettings.ShowTitleBarBtns)
            {
                this.doorBtn.Visibility = Visibility.Hidden;
            }

            this.Show();
        }

        #endregion

        #region Driver 模块事件 发送

        public void OnDriverEvent(object obj)
        {
            Send(obj, true);
        }

        #endregion


        private object lockObj = new object();

        #region webview 接收事件

        /// <summary>
        /// 接收前端信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs receivedEvent)
        {
            var result = new WebViewSendModel<object>();
            // 获取请求字符串
            string reqStr = receivedEvent.WebMessageAsJson;
            object[] parameters = null;

            // lock (lockObj)
            // {
            Task.Run(() =>
            {
                try
                {
                    // 检查请求字符串
                    if (string.IsNullOrEmpty(reqStr))
                    {
                        result.msg = "无法序列化成对象";
                        result.devMsg = @$"请求字符串 ： [{reqStr}]";
                        Send(result);
                        return;
                    }

                    // 请求model序列化, 一定要序列化成 object[]
                    var req = IsUtil.Json.ToObject<ReqMessageModel<object[]>>(reqStr);
                    // 请求method guid 赋值
                    result.method = req.method;
                    result.guid = req.guid;

                    string interfaceName = req.method.Split('.')[0]; // 执行的接口名
                    string methodName = req.method.Split('.')[1]; // 执行的方法
                    parameters = req.@params; // 执行参数处理
                    // 把请求参数中的 JArray 转换为 List<List<string> ,否则会匹配不到方法


                    for (int i = 0; i < parameters?.Length; i++)
                    {
                        var item = parameters[i];
                        if (!item.IsNull() && item.GetType().Name == "JArray")
                            item = item.JsonMapTo<List<string>>();
                    }


                    // 反射执行指定方法并赋值
                    var resultObj = _driverHandle.Invoke(interfaceName, methodName, parameters);

                    // 赋值
                    result.devMsg = resultObj?.devMsg;
                    result.msg = resultObj?.msg;
                    result.status = resultObj?.status ?? 0;
                    result.success = resultObj?.success ?? false;
                    result.response = resultObj?.response;
                }
                catch (Exception ex)
                {
                    ILog log = Log.GetLog();
                    log.Content("WebMessageReceived捕获异常");
                    // 设置日志等级为 警告
                    log.Warn();
                    // 设置内容为 WebMessageReceived捕获异常
                    // 写入日志
                    ex.Log(log);

                    // 设置devMsg
                    result.devMsg = ex.ToString();
                    result.msg = "操作异常";
                    result.success = false;
                }

                // 发送信息给前端页面
                Send(result, false, reqStr, Json.ToJson(parameters));
            });
            // }
        }

        #endregion

        #region 发送信息给前端页面(Send)

        /// <summary>
        /// 发送信息给前端页面
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isEvent"></param>
        /// <param name="reqStr"></param>
        /// <param name="para"></param>
        private void Send(dynamic obj, bool isEvent = false, string reqStr = "", string para = "")
        {
            // 事件处理
            if (isEvent)
            {
                if (obj.status == 1001)
                    this.Dispatcher.Invoke(new Action(() =>
                        _doorViewWindow?.webView?.CoreWebView2?.PostWebMessageAsString(Json.ToJson(obj))));
                else
                    this.Dispatcher.Invoke(new Action(() =>
                        _webViewWindow?.webView?.CoreWebView2?.PostWebMessageAsString(Json.ToJson(obj))));
            }
            else
                this.Dispatcher.Invoke(new Action(() =>
                    this.webView?.CoreWebView2?.PostWebMessageAsString(Json.ToJson(obj))));


            // 屏蔽黑名单进info日志
            if (!string.IsNullOrWhiteSpace(obj.method) &&
                _driverHandle.BlackListLogMethod.Contains((string)obj.method))
                return;

            // 日志信息记录
            RequestLogInfo loginfo = new RequestLogInfo();
            loginfo.reqStr = reqStr;
            loginfo.para = para;
            loginfo.rtSuccess = obj.success;
            loginfo.rtMsg = obj.msg;
            loginfo.devMsg = obj.devMsg;

            string webLog = "";
            if (isEvent)
            {
                loginfo.eventName = obj.method;

                // 摄像头事件则不传输 rtData
                if (loginfo.eventName != nameof(ICamera.OnCameraGetImage))
                    loginfo.rtData = Json.ToJson(obj.response);

                webLog = loginfo.WriteEvent();
            }
            else
            {
                loginfo.method = obj.method;
                loginfo.rtData = Json.ToJson(obj.response);
                webLog = loginfo.WriteActionLog();
            }

            webLog = @$"console.debug('\x1B[" + logColor + @$"m%s\x1B[0m', '{webLog}')".Replace("\r\n", "\\n");

            if (logColor == "32")
                logColor = "34";
            else
                logColor = "32";

            if (obj.status == 1001)
                this.Dispatcher.Invoke(
                    new Action(() => _doorViewWindow.webView.CoreWebView2.ExecuteScriptAsync(webLog)));
            else
                this.Dispatcher.Invoke(new Action(() =>
                    _webViewWindow?.webView?.CoreWebView2?.ExecuteScriptAsync(webLog)));
        }

        #endregion

        public void ShowDoorViewBtn(object sender, RoutedEventArgs e)
        {
            if (_doorViewWindow == null)
            {
                _doorViewWindow =
                    new WebViewWindow(_driverHandle, _systemFunc, _serviceProvider, _netWorkTranspondService);
                _doorViewWindow.isDoorWindow = true;
                var url = this._clientSettings.LibraryManageUrl +
                          (this._clientSettings.LibraryManageUrl.Last() == '/' ? "doorInfo" : "/doorInfo");
                //_doorViewWindow.openUrl = _netWorkTranspondService.GetVisitUrl(this._clientSettings.DoorControllerUrl.IsEmpty() ? url : this._clientSettings.DoorControllerUrl);
                _doorViewWindow.OpenUrl = this._clientSettings.DoorControllerUrl.IsEmpty()
                    ? url
                    : this._clientSettings.DoorControllerUrl;

                _doorViewWindow.Title = "通道门";
                var clseEvent = () =>
                {
                    var mainWindow = _serviceProvider.GetService<MainWindow>();
                    if (!mainWindow.IsVisible && !_webViewWindow.IsVisible && _doorViewWindow?.IsVisible != true)
                    {
                        // 退出整个应用
                        Environment.Exit(0);
                    }
                };


                _doorViewWindow.Closed += (s, e) => { clseEvent?.Invoke(); };

            }

            try
            {
                _doorViewWindow.Show();
            }
            catch (Exception)
            {
                _doorViewWindow = null;
                ShowDoorViewBtn(null, null);
            }
        }

        // 修复 win7 触屏 
        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                    }
                }
            }
        }

        private class RequestLogInfo
        {
            private ILog log;
            public string reqStr { get; set; }
            public string title { get; set; }
            public string method { get; set; }
            public string para { get; set; }
            public string rtData { get; set; }
            public string eventName { get; set; }
            public string rtMsg { get; set; }
            public bool rtSuccess { get; set; }

            public string devMsg { get; set; }

            public RequestLogInfo()
            {
                log = Log.GetLog();
            }

            private void Write(string text)
            {
                log.Content(text);
                log.Info();
            }

            public virtual string WriteActionLog(string reqStr, string title, string method, string para, string rtData,
                bool rtSuccess, string rtMsg, string devMsg)
            {
                string text =
                    $" 请求字符串　：{reqStr} \r\n 标题　　　　：{title} \r\n 请求方法　　：{method} \r\n 请求参数　　：{para} \r\n 返回数据　　：{rtData} \r\n 返回成功状态：{rtSuccess} \r\n 返回信息　　：{rtMsg} \r\n ";
                Write(text);
                return text;
            }

            public virtual string WriteActionLog()
            {
                string text =
                    $" 请求字符串　：{reqStr} \r\n 标题　　　　：{title} \r\n 请求方法　　：{method} \r\n 请求参数　　：{para} \r\n 返回数据　　：{rtData} \r\n 返回成功状态：{rtSuccess} \r\n 返回信息　　：{rtMsg} \r\n 调试信息　　：{devMsg} \r\n ";
                Write(text);
                return text;
            }

            public virtual string WriteEvent(string title, string eventName, string rtData, bool rtSuccess,
                string rtMsg)
            {
                string text =
                    $" 标题　　　　：{title} \r\n 事件名称　　：{eventName} \r\n 返回数据　　：{rtData} \r\n 返回成功状态：{rtSuccess} \r\n 返回信息　　：{rtMsg} \r\n ";
                Write(text);
                return text;
            }

            public virtual string WriteEvent()
            {
                string text =
                    $" 标题　　　　：{title} \r\n 事件名称　　：{eventName} \r\n 返回数据　　：{rtData} \r\n 返回成功状态：{rtSuccess} \r\n 返回信息　　：{rtMsg} \r\n ";
                Write(text);
                return text;
            }
        }
    }
}