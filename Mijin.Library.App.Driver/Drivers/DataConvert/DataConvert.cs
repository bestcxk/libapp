using System.Runtime.InteropServices;
using System.Text;
using Bing.Extensions;
using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Drivers.DataConvert
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

            epc = epc.Substring(8);

            var bytes = new byte[15];

            epc.HexStringToBytes().CopyTo(bytes, 0);

            var len = iRFID_Decode96bit(bytes);

            res.response = Encoding.ASCII.GetString(bytes).Replace("\0", "");
            res.success = true;
            res.msg = "获取成功";
            return res;
        }
    }
}
