using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 系统提供方法接口
    /// </summary>
    public interface ISystemFunc
    {
        /// <summary>
        /// 获取系统中所有能使用的com口
        /// </summary>
        /// <returns></returns>
        MessageModel<string[]> GetComs();

    }
}