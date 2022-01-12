
using Bing.Helpers;
using Bing.IO;
using Bing.Text;
using IsUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Common.Helper
{
    public class EncryptApp
    {

        private static Computer computer = null;

        private static readonly object _lock = new object();

        public static Computer Computer
        {
            get
            {
                if (computer == null)
                {
                    lock (_lock)
                    {
                        if (computer == null)
                        {
                            lock (_lock)
                            {
                                computer = new Computer();
                            }
                        }
                    }
                }


                return computer;
            }
            private set
            {
            }
        }


        public string GetEncryptStr()
        {
            var infoStr = @$"{Computer.CpuID}{Computer.DiskID}{Computer.SystemType}{Computer.TotalPhysicalMemory}";

            var str = Encrypt.Sha256(infoStr).Substring(0, 16).ToUpper();

            var key = InsertFormat(str, 4, "-");

            return key;
        }

        public string GetDeEncryptStr(string encryptStr)
        {
            if (encryptStr.IsEmpty())
            {
                return "";
            }

            var str = "";
            var sha = Encrypt.Sha256(encryptStr);
            for (int i = 0; i < 16; i++)
            {
                str += sha[16].ToString();
                sha = Encrypt.Sha256(sha);
            }


            return InsertFormat(str, 4, "-").ToUpper();

        }
        /// <summary>
        /// 每隔n个字符插入一个字符
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="interval">间隔字符数</param>
        /// <param name="value">待插入值</param>
        /// <returns>返回新生成字符串</returns>
        private static string InsertFormat(string input, int interval, string value)
        {
            for (int i = interval; i < input.Length; i += interval + 1)
                input = input.Insert(i, value);
            return input;
        }
    }
}
