using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsUtil.Helpers
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

        #region byteHEX
        /// <summary>
        /// 单个字节转字字符.
        /// </summary>
        /// <param name="ib">字节.</param>
        /// <returns>转换好的字符.</returns>
        public static string byteHEX(Byte ib)
        {
            string _str = string.Empty;
            try
            {
                char[] Digit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
                'B', 'C', 'D', 'E', 'F' };
                char[] ob = new char[2];
                ob[0] = Digit[(ib >> 4) & 0X0F];
                ob[1] = Digit[ib & 0X0F];
                _str = new string(ob);
            }
            catch (Exception)
            {
                new Exception("对不起有错。");
            }
            return _str;

        }
        #endregion
    }
}
