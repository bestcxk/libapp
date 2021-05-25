using Microsoft.Web.WebView2.Core;
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

namespace Mijin.Library.App.Views
{
    /// <summary>
    /// WebViewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebViewWindow : MetroWindow
    {
        private readonly IDriverHandle _driverHandle;
        private readonly ClientSettings _clientSettings;
        public static WebViewWindow _doorViewWindow { get; set; } = null;
        public static WebViewWindow _webViewWindow { get; set; } = null;
        public ISystemFunc _systemFunc { get; }
        public string openUrl { get; set; }

        static private bool hasRegisterEvent = false;

        #region 构造函数
        public WebViewWindow(IDriverHandle driverHandle, ISystemFunc systemFunc)
        {
            _driverHandle = driverHandle;
            _systemFunc = systemFunc;
            _clientSettings = systemFunc.ClientSettings;
            InitializeComponent();
            InitializeAsync(); // 初始化



        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        async void InitializeAsync()
        {
            // 窗口关闭时
            this.Closed += async (s, e) =>
            {
                #region 退出时清空浏览器缓存
                try
                {
                    if ((!_doorViewWindow.IsNull() && _doorViewWindow.IsVisible) || _webViewWindow.IsVisible)
                    {
                        return;
                    }
                    // 获取webview 进程id 并杀掉，否则无法删除文件
                    var process = Process.GetProcessById((int)this.webView.CoreWebView2.BrowserProcessId);
                    process.Kill();
                    process.WaitForExit();
                    //_webViewWindow.webView.Dispose();
                    //GC.Collect();

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
                Environment.Exit(0);
            };

            await this.webView.EnsureCoreWebView2Async(null);
            this.webView.CoreWebView2.WebMessageReceived += WebMessageReceived;

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
                this.WindowState = System.Windows.WindowState.Normal;//还原窗口（非最小化和最大化）
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
                    this.WindowState = WindowState.Maximized;
                    this.ShowTitleBar = true;  // 不显示标题栏
                }
                else  // 不显示标题栏
                {
                    this.WindowState = System.Windows.WindowState.Normal;//还原窗口（非最小化和最大化）
                    this.WindowStyle = System.Windows.WindowStyle.None; //仅工作区可见，不显示标题栏和边框
                    this.ResizeMode = System.Windows.ResizeMode.NoResize;//不显示最大化和最小化按钮
                    this.ShowTitleBar = false;  // 不显示标题栏

                    this.Left = 0.0;
                    this.Top = 0.0;
                    this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                    this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
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

            // 直达webView模式
            if (!string.IsNullOrEmpty(_clientSettings.NoSelectOpenUrl))
            {
                openUrl = _clientSettings.NoSelectOpenUrl;
            }

            #region 当前路由处理
            if (!string.IsNullOrWhiteSpace(openUrl) && openUrl.Substring(0, 2) == @$".\")
            {
                this.openUrl = @$"{Path.Combine(Environment.CurrentDirectory, openUrl.Substring(2)).Replace(@$"\",@$"/")}";
            }
            this.webView.Source = new Uri(this.openUrl);
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

        #region webview 接收事件
        /// <summary>
        /// 接收前端信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs receivedEvent)
        {
            var result = new WebViewSendModel<object>();
            // 获取请求字符串
            string reqStr = receivedEvent.WebMessageAsJson;

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
                    string methodName = req.method.Split('.')[1];    // 执行的方法
                    object[] parameters = req.@params;            // 执行参数处理
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
                    // 设置日志等级为 警告
                    log.Warn();
                    // 设置内容为 WebMessageReceived捕获异常
                    log.Content("WebMessageReceived捕获异常");
                    // 写入日志
                    ex.Log(log);

                    // 设置devMsg
                    result.devMsg = ex.ToString();
                    result.msg = "操作异常";
                    result.success = false;
                }

                // 发送信息给前端页面
                Send(result);
            });
        }
        #endregion

        #region 发送信息给前端页面(Send)
        /// <summary>
        /// 发送信息给前端页面
        /// </summary>
        /// <param name="obj"></param>
        private void Send(dynamic obj, bool isEvent = false)
        {
            try
            {
                // 事件处理
                if (isEvent)
                {
                    if (obj.status == 1001)
                        this.Dispatcher.Invoke(new Action(() => _doorViewWindow.webView.CoreWebView2.PostWebMessageAsString(Json.ToJson(obj))));
                    else
                        this.Dispatcher.Invoke(new Action(() => _webViewWindow.webView.CoreWebView2.PostWebMessageAsString(Json.ToJson(obj))));
                }
                else
                    this.Dispatcher.Invoke(new Action(() => this.webView.CoreWebView2.PostWebMessageAsString(Json.ToJson(obj))));
            }
            catch (Exception)
            {
            }
        }
        #endregion

        public void ShowDoorViewBtn(object sender, RoutedEventArgs e)
        {
            if (_doorViewWindow == null)
            {
                _doorViewWindow = new WebViewWindow(_driverHandle, _systemFunc);
                var url = this._clientSettings.LibraryManageUrl + (this._clientSettings.LibraryManageUrl.Last() == '/' ? "door" : "/door");
                _doorViewWindow.openUrl = url;
                _doorViewWindow.Title = "通道门";
            }
            _doorViewWindow.Show();
        }
    }
}
