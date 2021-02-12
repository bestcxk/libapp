using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 屏幕键盘接口
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// 关闭屏幕键盘
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Close();
        /// <summary>
        /// 打开屏幕键盘
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Open();
    }
}