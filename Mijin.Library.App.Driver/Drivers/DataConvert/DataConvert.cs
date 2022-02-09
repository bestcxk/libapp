using Bing.Extensions;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public class DataConvert : IDataConvert
    {
        [DllImport("EncPro.dll")]
        public static extern int iRFID_Decode96bit(byte[] bar);

        // 云海电子 - 系统完善,自助借还系统对接图创 超高频
        public MessageModel<string> RFID_Decode96bit(string epc)
        {
            var res = new MessageModel<string>();
            if (epc.IsEmpty() || epc.Length != 24)
            {
                res.msg = "epc长度不正确";
                return res;
            };

            epc = epc.Substring(8, epc.Length - 1);

            var bytes = new byte[15];
            Encoding.ASCII.GetBytes(epc).CopyTo(bytes, 0);

            var len = iRFID_Decode96bit(bytes);

            res.response = Encoding.ASCII.GetString(bytes);
            res.success = true;
            res.msg = "获取成功";
            return res;
        }
    }
}
