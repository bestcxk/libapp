
using PublicAPI.CKC001.Others;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.CKC001.MessageObj.MsgObj
{
    public class MsgObjBase
    {
        /// <summary>
        /// 头标志 2字节
        /// </summary>
        private byte[] headFlag;
        /// <summary>
        /// 地址 1字节
        /// </summary>
        private byte addressNum;
        private byte[] serialNum;//序列号 4字节
        private eCmdType cmdType; //命令类型
        private byte cmdTag;    //命令tag
        private eReturnResult ereturn;//返回值
        private byte[] cmdLenByte;//命令长度
        private byte[] cmdData;//命令数据
        private byte[] crc;//crc
        private byte[] frameData;//帧数据

        internal byte[] HeadFlag { get => headFlag; set => headFlag = value; }
        internal byte AddressNum { get => addressNum; set => addressNum = value; }
        internal byte[] SerialNum { get => serialNum; set => serialNum = value; }
        internal eCmdType CmdType { get => cmdType; set => cmdType = value; }
        internal byte CmdTag { get => cmdTag; set => cmdTag = value; }
        internal eReturnResult eReturn { get => ereturn; set => ereturn = value; }
        //public ushort CmdLen { get => cmdLen; set => cmdLen = value; }
        internal byte[] CmdLenByte { get => cmdLenByte; set => cmdLenByte = value; }
        internal byte[] CmdData { get => cmdData; set => cmdData = value; }
        internal byte[] Crc { get => crc; set => crc = value; }
        internal byte[] FrameData { get => frameData; set => frameData = value; }

        public MsgObjBase()  //消息基础
        {
            headFlag = new byte[2] { 0x17, 0x99 };//头部
            addressNum = 0x00;//地址
            cmdData = null;
            ereturn = eReturnResult.NonResponse;
            serialNum = new byte[4];//序列号
        }
        public MsgObjBase(byte[] receivedData) //接收数据
        {

            frameData = receivedData;//帧数据
            int flag = 0;
            headFlag = new byte[2] { receivedData[flag], receivedData[flag + 1] };
            flag += 2;
            addressNum = receivedData[flag++];
            serialNum = new byte[4] { receivedData[flag], receivedData[flag + 1], receivedData[flag + 2], receivedData[flag + 3] };
            flag += 4;
            cmdType = (eCmdType)receivedData[flag++];
            cmdTag = receivedData[flag++];
            ereturn = (eReturnResult)receivedData[flag++];
            cmdLenByte = new byte[2] { receivedData[flag], receivedData[flag + 1] };
            flag += 2;
            ushort len = DataConverts.Bytes_To_Ushort(cmdLenByte);
            if (len > 0)
            {
                //if (receivedData.Length == 1460)
                //{

                //}
                cmdData = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    //var str = DataConverts.Bytes_To_HexStr(receivedData);
                    cmdData[i] = receivedData[flag + i];
                }
                //Array.Copy(receivedData, flag, cmdData, 0, len);

            }
            else
                cmdData = null;
            flag += len;
            crc = new byte[2] { receivedData[flag], receivedData[flag + 1] };

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        internal virtual void SendPacked() { }

        /// <summary>
        /// 把属性打包成数组
        /// </summary>
        /// <returns></returns>
        internal byte[] CmdToBytes()
        {
            SendPacked();
            List<byte> tempCmd = new List<byte>();
            tempCmd.AddRange(headFlag);
            tempCmd.Add(addressNum);
            tempCmd.AddRange(serialNum);
            tempCmd.Add((byte)cmdType);
            tempCmd.Add(cmdTag);
            if (cmdData == null)
            {
                tempCmd.AddRange(new byte[2] { 0x00, 0x00 });
            }
            else
            {
                tempCmd.AddRange(DataConverts.Int_To_Bytes((ushort)cmdData.Length));
                tempCmd.AddRange(cmdData);
            }
            byte[] tempBytes = new byte[tempCmd.Count];
            frameData = new byte[tempCmd.Count + 2];
            for (int i = 0; i < tempCmd.Count; i++)
            {
                tempBytes[i] = frameData[i] = tempCmd[i];
            }
            byte[] tempCrc = DataConverts.Make_CRC16(tempBytes);
            frameData[frameData.Length - 2] = tempCrc[0];
            frameData[frameData.Length - 1] = tempCrc[1];
            return frameData;
        }
        internal bool CheckCRC()
        {
            if (frameData.Length < 13)
                return false;
            byte[] tempData = DataConverts.ReadBytes(frameData, 0, frameData.Length - 2);
            byte[] temp = DataConverts.Make_CRC16(tempData);
            if (temp[0] == crc[0] && temp[1] == crc[1])
            {
                return true;
            }
            return false;
        }

        internal string ToKey()
        {
            byte[] temp = new byte[7] { addressNum, serialNum[0], serialNum[1], serialNum[2], serialNum[3], (byte)cmdType, cmdTag };
            return DataConverts.Bytes_To_HexStr(temp);
        }
    }
}
