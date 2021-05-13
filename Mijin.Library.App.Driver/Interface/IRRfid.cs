using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IRRfid
    {
        /// <summary>
        /// 自动连接设备
        /// </summary>
        /// <returns></returns>
        MessageModel<string> AutoOpenComPort();
        /// <summary>
        /// 新的扫描
        /// NewScan(3,2) 即为 从第三块区开始读，读2个区的数据，即为读3 4 区的数据
        /// NewScan(3,5) 即为 从第三块区开始读，读5个区的数据，即为读3 4 5 6 7 区的数据
        /// </summary>
        /// <param name="startBlockNum">起始块地址</param>
        /// <param name="blockCount">连续读取的数据块数</param>
        /// <returns></returns>
        MessageModel<List<ScanDataModel>> NewScan(long startBlockNum, long blockCount);

        /// <summary>
        /// 设置高频标签EAS
        /// </summary>
        /// <param name="uid">标签Uid</param>
        /// <param name="enabled">使能EAS报警</param>
        /// <returns></returns>
        MessageModel<string> SetEAS(List<Int64> uid, bool enabled);
    }
}