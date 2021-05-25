using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public class ZktDoorController : IDoorController
    {
        #region Dll Import
        [DllImport("plcommpro.dll", EntryPoint = "Connect")]
        public static extern IntPtr Connect(string Parameters);
        [DllImport("plcommpro.dll", EntryPoint = "PullLastError")]
        public static extern int PullLastError();

        [DllImport("plcommpro.dll", EntryPoint = "Disconnect")]
        public static extern void Disconnect(IntPtr h);
        [DllImport("plcommpro.dll", EntryPoint = "GetDeviceParam")]
        public static extern int GetDeviceParam(IntPtr h, ref byte buffer, int buffersize, string itemvalues);
        [DllImport("plcommpro.dll", EntryPoint = "SetDeviceParam")]
        public static extern int SetDeviceParam(IntPtr h, string itemvalues);
        [DllImport("plcommpro.dll", EntryPoint = "ControlDevice")]
        public static extern int ControlDevice(IntPtr h, int operationid, int param1, int param2, int param3, int param4, string options);

        [DllImport("plcommpro.dll", EntryPoint = "GetDeviceDataCount")]
        public static extern int GetDeviceDataCount(IntPtr h, string tablename, string filter, string options);
        [DllImport("plcommpro.dll", EntryPoint = "GetDeviceData")]
        public static extern int GetDeviceData(IntPtr h, ref byte buffer, int buffersize, string tablename, string filename, string filter, string options);

        [DllImport("plcommpro.dll", EntryPoint = "SetDeviceData")]
        public static extern int SetDeviceData(IntPtr h, string tablename, string data, string options);
        [DllImport("plcommpro.dll", EntryPoint = "GetRTLog")]
        public static extern int GetRTLog(IntPtr h, ref byte buffer, int buffersize);
        [DllImport("plcommpro.dll", EntryPoint = "SearchDevice")]
        //public static extern int SearchDevice( ref byte commtype, ref byte address, ref byte buffer);
        public static extern int SearchDevice(string commtype, string address, ref byte buffer);
        [DllImport("plcommpro.dll", EntryPoint = "ModifyIPAddress")]
        public static extern int ModifyIPAddress(string commtype, string address, string buffer);
        [DllImport("plcommpro.dll", EntryPoint = "GetDeviceFileData")]
        public static extern int GetDeviceFileData(IntPtr h, ref byte buffer, ref int buffersize, string filename, string options);


        [DllImport("plcommpro.dll", EntryPoint = "SetDeviceFileData")]
        public static extern int SetDeviceFileData(IntPtr h, string filename, ref byte buffer, int buffersize, string options);

        [DllImport("plcommpro.dll", EntryPoint = "DeleteDeviceData")]
        public static extern int DeleteDeviceData(IntPtr h, string tablename, string data, string options);
        [DllImport("plcommpro.dll", EntryPoint = "ProcessBackupData")]
        public static extern int ProcessBackupData(byte[] data, int fileLen, ref byte Buffer, int BufferSize);
        #endregion

        private IntPtr h = IntPtr.Zero;
        private string connectStr = "protocol=TCP,ipaddress={0},port={1},timeout={2},passwd=";

        private string saveIp = null;
        private Int64 savePort = 0;
        private Int64 saveTimeout = 0;

        private int deep = 0;
        private int maxDeep = 3;

        public MessageModel<bool> Connect(string ip, Int64 port, Int64 timeout)
        {
            var res = new MessageModel<bool>();
            if (IntPtr.Zero != h)
            {
                Disconnect(h);
            }
            var conStr = string.Format(connectStr, ip, port, timeout);
            h = Connect(conStr);
            if (h == IntPtr.Zero)
            {
                var errorCode = PullLastError();

                res.devMsg = "Connect device Failed! The error id is: " + errorCode;
                res.msg = "连接门控失败";
                return res;
            }
            saveIp = ip;
            savePort = port;
            saveTimeout = timeout;
            res.msg = "连接门控成功";
            res.success = true;
            return res;
        }

        /// <summary>
        /// 开门
        /// </summary>
        /// <param name="openTime">(0:, 255:Hormal-open, value range : 1~60 seconds)</param>
        /// <returns></returns>
        public MessageModel<bool> OpenDoor(Int64 openTime = 5)
        {
            var res = new MessageModel<bool>();
            var ret = ControlDevice(h, 1, 1, 1, (int)openTime, 0, "");
            if (ret >= 0)
            {
                res.msg = "开门操作成功";
                res.success = true;
            }
            else
            {
                res.msg = "开门操作失败";
                if (h != IntPtr.Zero)
                {
                    if (++deep < maxDeep)
                    { 
                        var connectRes = Connect(saveIp, savePort, saveTimeout);
                        if (connectRes.success)
                        {
                            return OpenDoor(openTime);
                        }
                    }
                }
            }
            deep = 0;
            return res;
        }
    }
}
