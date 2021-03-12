using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 系统提供方法接口
    /// </summary>
    public interface ISystemFunc
    {
        /// <summary>
        /// 图书馆设置
        /// </summary>
        public LibrarySettings LibrarySettings { get; set; }

        /// <summary>
        /// 客户端设置
        /// </summary>
        public ClientSettings ClientSettings { get; set; }
        /// <summary>
        /// 获取系统中所有能使用的com口
        /// </summary>
        /// <returns></returns>
        MessageModel<string[]> GetComs();

        /// <summary>
        /// 配置图书馆设置
        /// </summary>
        /// <param name="librarySettings"></param>
        /// <returns></returns>
        MessageModel<int> SetLibrarySettings(LibrarySettings librarySettings);

    }
}