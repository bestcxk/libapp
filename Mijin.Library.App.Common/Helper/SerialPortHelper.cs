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
    }
}
