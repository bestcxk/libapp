using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// RFID接口
    /// </summary>
    public interface IRfid
    {
        /// <summary>
        /// 读到标签事件
        /// </summary>
        event Action<WebViewSendModel<LabelInfo>> OnReadLabel;

        /// <summary>
        /// 232自动连接读写器
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Auto232Connect();
        /// <summary>
        /// 连接读写器
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        MessageModel<bool> Connect(string mode, string conStr, Int64 timeOutMs = 1500);
        /// <summary>
        /// 获取当前读写器频段
        /// </summary>
        /// <returns></returns>
        MessageModel<FreqRange> GetFreqRange();
        /// <summary>
        /// 获取功率
        /// </summary>
        /// <returns></returns>
        MessageModel<Dictionary<byte, byte>> GetPower();
        /// <summary>
        /// 开启读
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Read();
        /// <summary>
        /// 开启指定天线读
        /// </summary>
        /// <param name="antIds"></param>
        /// <returns></returns>
        MessageModel<bool> ReadByAntId(List<string> antIdStrs);
        /// <summary>
        /// 只读一次，读到标签后返回
        /// </summary>
        /// <returns></returns>
        MessageModel<LabelInfo> ReadOnce();
        /// <summary>
        /// 开启指定天线，读到标签后返回
        /// </summary>
        /// <param name="antIds"></param>
        /// <returns></returns>
        MessageModel<LabelInfo> ReadOnceByAntId(List<string> antIdStrs);
        /// <summary>
        /// 设置频段
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        MessageModel<bool> SetFreqRange(string index);
        /// <summary>
        /// 设置GPO
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        MessageModel<bool> SetGpo(Dictionary<string, byte> dic);

        /// <summary>
        /// 设置功率
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        MessageModel<Dictionary<byte, byte>> SetPower(Dictionary<byte, byte> dt);

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="gpiAction"></param>
        /// <returns></returns>
        MessageModel<bool> StartInventory(GpiAction gpiAction = GpiAction.Default);
        /// <summary>
        /// 停止读
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Stop();
        /// <summary>
        /// 停止盘点
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> StopInventory();
        /// <summary>
        /// 写标签
        /// </summary>
        /// <param name="area">0， 保留区； 1， EPC 区； 2， TID 区； 3， 用户数据区</param>
        /// <param name="startAddr">开始写地址</param>
        /// <param name="data">写入数据</param>
        /// <param name="baseTid">指定Tid</param>
        /// <param name="password">访问密码</param>
        /// <param name="timeOut">超时次数</param>
        /// <returns></returns>
        MessageModel<bool> WriteLabel(Int64 area, Int64 startAddr, string data, string baseTid = null, string password = "00000000", Int64 timeOut = 3);
    }
}
