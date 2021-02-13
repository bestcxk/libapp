using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Model
{
    public class baseClientSettings
    {
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
        /// 每次退出清空浏览器缓存
        /// </summary>
        public bool OnExitClearWebCookie { get; set; }

        /// <summary>
        /// 开发者模式
        /// </summary>
        public bool IsDev { get; set; }

    }
}
