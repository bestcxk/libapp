using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsUtil
{
    /// <summary>
    /// String 类扩展
    /// </summary>
    public static partial class Extensions
    {
        #region Search(搜索字符串中间内容)
        /// <summary>
        /// 搜索字符串中间内容
        /// </summary>
        /// <param name="source">完整的内容</param>
        /// <param name="left">左边的内容</param>
        /// <param name="right">右边的内容</param>
        /// <returns></returns>
        public static string Search(this string source, string left, string right)  //获取搜索到的数目  
        {
            try
            {
                int n1, n2;
                n1 = source.IndexOf(left, 0);   //开始位置  
                if (n1 < 0) return null;
                n1 += left.Length;

                n2 = source.IndexOf(right, n1);               //结束位置    

                if (n2 < 0)
                    return null;

                return source.Substring(n1, n2 - n1);   //取搜索的条数，用结束的位置-开始的位置,并返回   
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion
    }
}
