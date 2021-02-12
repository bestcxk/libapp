﻿using Microsoft.Web.WebView2.Core;
using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mijin.Library.App.Model;
using Util.Helpers;
using Util.Logs.Extensions;
using Util.Logs;
using Util.Maps;

namespace Mijin.Library.App.Views
{
    /// <summary>
    /// WebView.xaml 的交互逻辑
    /// </summary>
    public partial class WebView : Window
    {
        private readonly IDriverHandle _driverHandle;
        private readonly ClientSettings _clientSettings;

        #region 构造函数
        public WebView(IDriverHandle driverHandle,ClientSettings clientSettings)
        {
            _driverHandle = driverHandle;
            _clientSettings = clientSettings;


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
            await this.webView.EnsureCoreWebView2Async(null);
            this.webView.CoreWebView2.WebMessageReceived += WebMessageReceived;

            // 注册事件
            _driverHandle.lockStatusEvent += LockStatusEvent;
            _driverHandle.OnTagEpcLog += OnTagEpcLog;
            _driverHandle.OnPeopleInOut += OnPeopleInOut;

            // 窗口关闭时
            this.Closed += (s, e) =>
            {
                // 清空Cookies
                if (_clientSettings.OnExitClearWebCookie)
                    this.webView.CoreWebView2.CookieManager.DeleteAllCookies();

                // 退出整个应用
                Environment.Exit(0);
            };

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
                this.WindowState = System.Windows.WindowState.Normal;
                this.WindowStyle = System.Windows.WindowStyle.None;
            }

            // 全屏
            if (_clientSettings.WindowWidth == 0 && _clientSettings.WindowHeight == 0)
            {
                // 显示标题栏
                if (_clientSettings.ShowWindowTitleBar)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else  // 不显示标题栏
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                    this.WindowStyle = System.Windows.WindowStyle.None;
                    this.Topmost = true;
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
            }
            // 窗口显示在屏幕中央
            Rect workArea = SystemParameters.WorkArea;
            Left = (workArea.Width - this.Width) / 2 + workArea.Left;
            Top = (workArea.Height - this.Height) / 2 + workArea.Top;
            // 显示到最前端
            this.Topmost = true;
            #endregion

            if (!string.IsNullOrEmpty(_clientSettings.NoSelectOpenUrl))
            {
                this.webView.Source = new Uri(_clientSettings.NoSelectOpenUrl);
            }
        }
        #endregion

        #region Driver 模块事件

        #region 通道门人员进出事件处理
        private void OnPeopleInOut(PeopleInOut obj)
        {
            var result = new SendMessageModel<object>()
            {
                success = true,
                msg = "获取成功",
                response = obj
            };

            Send(result);

        }
        #endregion
        #region 标签事件处理 
        private void OnTagEpcLog(LabelInfo obj)
        {
            var result = new SendMessageModel<object>()
            {
                success = true,
                msg = "获取成功",
                response = obj
            };

            Send(result);


        }
        #endregion
        #region 锁孔板事件处理
        private void LockStatusEvent(List<bool> obj)
        {
            var result = new SendMessageModel<object>()
            {
                success = true,
                msg = "获取成功",
                response = obj
            };

            Send(result);
        }
        #endregion

        #endregion

        #region 前端 web 接收事件
        /// <summary>
        /// 接收前端信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs receivedEvent)
        {
            var result = new SendMessageModel<object>();
            try
            {
                // 获取请求字符串
                string reqStr = receivedEvent.WebMessageAsJson;

                // 检查请求字符串
                if (string.IsNullOrEmpty(reqStr))
                {
                    result.msg = "无法序列化成对象";
                    result.devMsg = @$"请求字符串 ： [{reqStr}]";
                    Send(result);
                    return;
                }

                // 请求model序列化, 一定要序列化成 object[]
                var req = Util.Json.ToObject<ReqMessageModel<object[]>>(reqStr);
                // 请求method guid 赋值
                result.method = req.method;
                result.guid = result.guid;

                string interfaceName = req.method.Split('.')[0]; // 执行的接口名
                string methodName = req.method.Split('.')[1];    // 执行的方法
                object[] parameters = req.parameters;            // 执行参数处理
                // 把请求参数中的 JArray 转换为 List<List<string> ,否则会匹配不到方法
                for (int i = 0; i < parameters?.Length; i++)
                {
                    var item = parameters[i];
                    if (item.GetType().Name == "JArray")
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
        }
        #endregion

        #region 发送信息给前端页面(Send)
        /// <summary>
        /// 发送信息给前端页面
        /// </summary>
        /// <param name="obj"></param>
        private void Send(object obj)
        {
            this.webView.CoreWebView2.PostWebMessageAsString(Json.ToJson(obj));
        }
        #endregion

        #region web前端 发送/接收 类
        /// <summary>
        /// web 前端发送类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class SendMessageModel<T> : MessageModel<T>
        {
            public SendMessageModel()
            {
            }

            public SendMessageModel(baseMessageModel @base) : base(@base)
            {
            }
            /// <summary>
            /// 执行方法 sample : ISIP2Client.Connect
            /// </summary>
            public string method { get; set; }
            /// <summary>
            /// guid，确保唯一性
            /// </summary>
            public string guid { get; set; }
        }

        /// <summary>
        /// web 前端接收类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ReqMessageModel<T> : baseMessageModel
        {
            public ReqMessageModel()
            {
            }

            public ReqMessageModel(baseMessageModel @base) : base(@base)
            {
            }

            
            /// <summary>
            /// 执行方法 sample : ISIP2Client.Connect
            /// </summary>
            public string method { get; set; }
            /// <summary>
            /// 请求参数
            /// </summary>
            public object[] parameters { get; set; }
            /// <summary>
            /// guid，确保唯一性
            /// </summary>
            public string guid { get; set; }
            
        }
        #endregion
    }
}
