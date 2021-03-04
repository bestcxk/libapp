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
        /// <param name="com">com口号，只用传com口的号码</param>
        /// <returns></returns>
        MessageModel<string> ReadHFCardNo(int com);
        /// <summary>
        /// 读身份证信息
        /// </summary>
        /// <returns></returns>
        MessageModel<IdentityInfo> ReadIdentity();
    }
}