using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 身份证读卡器接口
    /// </summary>
    public interface IdentityReader
    {
        /// <summary>
        /// 读高频卡卡号(身份证读卡器不一定包含该功能)
        /// </summary>
        /// <returns></returns>
        MessageModel<string> ReadHFCardNo();
        /// <summary>
        /// 读身份证信息
        /// </summary>
        /// <returns></returns>
        MessageModel<IdentityInfo> ReadIdentity();
    }
}