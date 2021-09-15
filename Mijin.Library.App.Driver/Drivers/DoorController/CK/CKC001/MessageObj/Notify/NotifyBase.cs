
using PublicAPI.CKC001.Connected;
using PublicAPI.CKC001.MessageObj.MsgObj;
using PublicAPI.CKC001.Others;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.CKC001.MessageObj.Notify
{
    public class NotifyBase
    {
        /// <summary>
        /// 【所有上报都附带的】结果上报
        /// </summary>
        public eReturnResult getReturnResult { get; internal set; }
        /// <summary>
        /// 【所有上报都附带的】设备IP用于标识设备
        /// </summary>
        public string getDevIp { get; internal set; }
        /// <summary>
        /// 【所有上报都附带的】设备序列号用于标识设备
        /// </summary>
        public string getDevSN { get; internal set; }
        /// <summary>
        /// 【所有上报都附带的】返回的命令种类标识
        /// </summary>
        public eCmdType getCmdType { get; internal set; }
        /// <summary>
        /// 【所有上报都附带的】上报返回的具体命令
        /// </summary>
        public byte getCmdTag { get; internal set; }
        /// <summary>
        /// 【所有上报都附带的】上报返回的具体数据
        /// </summary>
        public string getFrameData { get; internal set; }

        /*数据区内容
         */
        public byte[] getDataArea;// { get; internal set; }

        protected void NotifyAdditiveAttributeA(MsgObjBase msg, string ip)
        {
            getReturnResult = msg.eReturn;
            getCmdType = msg.CmdType;
            getCmdTag = msg.CmdTag;
            getFrameData = DataConverts.Bytes_To_HexStr(msg.FrameData);
            getDevSN = DataConverts.Bytes_To_HexStr(msg.SerialNum);
            getDevIp = ip;
            // Array.Copy(msg.CmdData, getDataArea,msg.CmdData.Length);
            //getDataArea = (byte[])msg.CmdData.Clone();
            //System.Runtime.InteropServices.Marshal.Copy((IntPtr)msg.CmdData,0,getDataArea,msg.CmdData.Length);
        }
    }
}
