using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Model
{
    public class baseClientSettings
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 后台管理打开的URL 
        /// </summary>
        public string LibraryManageUrl { get; set; }
        /// <summary>
        /// 自助借阅打开的URL
        /// </summary>
        public string ReaderActionUrl { get; set; }

        /// <summary>
        /// 此字段若有值，则跳过选择窗口直接打开页面窗口
        /// </summary>
        public string NoSelectOpenUrl { get; set; }

        /// <summary>
        /// 窗口高
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// 窗口宽
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// 显示窗口标题栏
        /// </summary>
        public bool ShowWindowTitleBar { get; set; } = true;

        /// <summary>
        /// 可否手动调整窗口宽高
        /// </summary>
        public bool CanResize { get; set; } = true;

        /// <summary>
        /// 门禁Url
        /// </summary>
        public List<string> DoorUrls { get; set; }

        /// <summary>
        /// 每次退出清空Cookie
        /// </summary>
        public ClearWebViewCacheModeEnum OnExitClearWebCache { get; set; } = ClearWebViewCacheModeEnum.Default;

        /// <summary>
        /// 开发者模式
        /// </summary>
        public bool IsDev { get; set; }

        /// <summary>
        /// 窗口顶置
        /// </summary>
        public bool WindowOverhead { get; set; }

        /// <summary>
        /// 窗口被关闭后自动重启
        /// </summary>
        public bool CannotClosed { get; set; }

        /// <summary>
        /// 开机自启
        /// </summary>
        public bool FollowSystemRun { get; set; }

        /// <summary>
        /// 显示标题栏多项按钮
        /// </summary>
        public bool ShowTitleBarBtns { get; set; }

        /// <summary>
        /// 高频读卡器
        /// </summary>
        public HFReaderEnum HFReader { get; set; } = 0;

        /// <summary>
        /// 超高频模块事件名使用旧事件名
        /// </summary>
        public bool UHFEventIsOldName { get; set; }

        /// <summary>
        /// 高频读卡器发送原始卡号
        /// </summary>
        public bool HFOriginalCard { get; set; }

        public bool IsM513IdentityReader { get; set; }

        public Title Titles { get; set; } = new Title();

        public int CameraIndex { get; set; } = 0;

        public string LabelConvertUrl { get; set; }

        public string DoorControllerUrl { get; set; }

    }

    /// <summary>
    /// webview清楚缓存类型
    /// </summary>
    public enum ClearWebViewCacheModeEnum
    {
        /// <summary>
        /// 不清空
        /// </summary>
        Default = 0,
        /// <summary>
        /// 清空cookie
        /// </summary>
        Cookie = 1,
        /// <summary>
        /// 清空LocalState
        /// </summary>
        LocalState = 2,
        /// <summary>
        /// 删除webview 下的Default文件
        /// </summary>
        DefaultCache = 3,
        /// <summary>
        /// 删除整个webview 的Cache文件
        /// </summary>
        AllCache = 4,
    }

    public enum HFReaderEnum
    {
        [Description("黑色读卡器")]
        BlackReader = 0,
        [Description("荣睿高频读卡器")]
        RRHFReader = 1
    }

    public class Title
    {
        public string App { get; set; } = "图书管理系统";
        public string Manager { get; set; } = "后台管理系统";
        public string Terminal { get; set; } = "自助借阅";
    }

}
