using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Reader
{
    public interface IHFReader
    {
        /// <summary>
        /// 初始化高频读卡器
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Init();
        /// <summary>
        /// 读高频卡指定块
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MessageModel<string> ReadBlock(string block);
        /// <summary>
        /// 读高频卡卡号
        /// </summary>
        /// <returns></returns>
        MessageModel<string> ReadCardNo();
    }
}