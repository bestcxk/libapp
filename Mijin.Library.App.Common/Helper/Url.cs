using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IsUtil.Helpers
{
    /// <summary>
    /// Url操作
    /// </summary>
    public static partial class Url
    {
        /// <summary>
        /// 合并Url
        /// </summary>
        /// <param name="urls">url片断，范例：Url.Combine( "http://a.com","b" ),返回 "http://a.com/b"</param>
        public static string Combine(params string[] urls)
        {
            return Path.Combine(urls).Replace(@"\", "/");
        }

        /// <summary>
        /// 连接Url，范例：Url.Join( "http://a.com","b=1" ),返回 "http://a.com?b=1"
        /// </summary>
        /// <param name="url">Url，范例：http://a.com</param>
        /// <param name="param">参数，范例：b=1</param>
        public static string Join(string url, string param)
        {
            return $"{GetUrl(url)}{param}";
        }

        /// <summary>
        /// 获取Url
        /// </summary>
        private static string GetUrl(string url)
        {
            if (!url.Contains("?"))
                return $"{url}?";
            if (url.EndsWith("?"))
                return url;
            if (url.EndsWith("&"))
                return url;
            return $"{url}&";
        }

        public static string GetIpPort(string str)
        {
            if (str.IsEmpty())
                return null;

            try
            {



                Regex re = new Regex(@"(((?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(:[0-9]+)?|(?:ww‌​w.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?‌​(?:[\w]*))?)");

                MatchCollection mc = re.Matches(str);//获取的是一个数组

                for (int i = 0; i < mc.Count; i++)
                {
                    Console.WriteLine(mc[i].ToString());
                }
                string ip = mc[1].ToString();

                return ip;

            }
            catch (Exception e)
            {
                return null;
            }

            //try
            //{
            //    string p = @"(http|https)://(?<domain>[^(:|/]*)";
            //    Regex reg = new Regex(p, RegexOptions.IgnoreCase);
            //    Match m = reg.Match(str);
            //    return m.Groups["domain"].Value;
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}
        }

    }
}
