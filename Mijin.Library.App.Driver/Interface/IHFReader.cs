using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Interface
{
    /// <summary>
    /// 高频读卡器通用接口
    /// </summary>
    public interface IHFReader
    {
        /// <summary>
        /// 初始化高频读卡器
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Init();

        /// <summary>
        /// 读指定扇区内的指定块内容
        /// </summary>
        /// <param name="sector">扇区号</param>
        /// <param name="block">块号</param>
        /// <param name="HexKey">密钥</param>
        /// <returns></returns>
        MessageModel<string> ReadBlock(long sector, long block, string HexKey = "FFFFFFFFFFFF");
        /// <summary>
        /// 读高频卡卡号
        /// </summary>
        /// <returns></returns>
        MessageModel<string> ReadCardNo();
    }

    /// <summary>
    /// 荣睿高频卡接口
    /// </summary>
    public interface IRRHFReader : IHFReader
    {
        MessageModel<string> ChangeToISO14443A();
    }

    /// <summary>
    /// 黑色读卡器高频接口
    /// </summary>
    public interface IBlackHFReader : IHFReader
    {
        /// <summary>
        /// 读高频卡指定块
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        MessageModel<string> ReadBlock(string block);
    }
}