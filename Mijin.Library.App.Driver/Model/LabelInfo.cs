using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 标签信息Model
    /// </summary>
    public class LabelInfo
    {
        public LabelInfo()
        {
        }

        public LabelInfo(LogBaseEpcInfo logBaseEpcInfo)
        {
            Epc = logBaseEpcInfo.Epc;
            Tid = logBaseEpcInfo.Tid;
            AntId = logBaseEpcInfo.AntId;
            Rssi = logBaseEpcInfo.Rssi;
        }

        public string Epc { get; set; }
        public string Tid { get; set; }
        /// <summary>
        /// 读到标签的天线Id
        /// </summary>
        public byte AntId { get; set; }
        /// <summary>
        /// 标签 Rssi 值
        /// </summary>
        public byte Rssi { get; set; }
    }
}
