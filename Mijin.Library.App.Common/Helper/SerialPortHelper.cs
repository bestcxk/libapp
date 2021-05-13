using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Helpers
{
    public class SerialPortHelper
    {
        /// <summary>
        /// BCC和校验代码
        /// </summary>
        /// <param name="data">需要校验的数据包</param>
        /// <returns></returns>
        public static byte Get_CheckXor(IEnumerable<byte> data)
        {
            byte CheckCode = 0;
            foreach (var item in data)
            {
                CheckCode ^= item;
            }
            return CheckCode;
        }

        #region  16进制字符串到数组之间的相互转换
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)System.Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(System.Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
        #endregion
    }
}
