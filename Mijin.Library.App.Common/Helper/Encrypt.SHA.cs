using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IsUtil.Helpers
{
    /// <summary>
    /// SHA加密类
    /// </summary>
    public static partial class Encrypt
    {
        #region SHA

        #region SHA256加密
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data">加密的数据</param>
        /// <returns>加密后的数据</returns>
        public static string SHA256Encrypt(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
        #endregion

        #region SHA256摘要
        /// <summary>
        /// SHA256文件内容摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>加密后的字符串</returns>
        /// 用法 => string str = SHA256Encrypt.AbstractFile(@"D:\副本.rar");
        public static string SHA256AbstractFile(Stream stream)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] retVal = sha256.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// SHA256文件内容摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>加密后的字符串</returns>
        public static string SHA256AbstractFile(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                return SHA256AbstractFile(file);
            }
        }
        #endregion

        #endregion
    }
}
