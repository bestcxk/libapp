using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsUtil.Maps;
using System.Text.RegularExpressions;
using IsUtil;
using Bing.Text;

namespace Mijin.Library.App.Driver
{
    public class TuChuangSIP2Client : baseTcpClient, ITuChuangSIP2Client
    {
        private static string[] CirculationStatusStr =
        {
            "分编", "入藏", "在装订", "已合并", "修补", "丢失", "剔除", "普通借出", "预约", "阅览借出", "预借", "互借", "闭架借阅", "赠送", "交换出", "调拨",
            "转送", "临时借出", "未知状态"
        };

        private string Sip2Key = "";
        string account = null;
        string password = null;

        public TuChuangSIP2Client()
        {
            Console.WriteLine("client初始化");

            this.OnTcpClientConnected += () =>
            {
                this.AdminLogin(account, password);
            };


        }

        public TuChuangSIP2Client(string host, string port) : base(host, port)
        {

        }

        internal async override Task<string> SendAsync(string message)
        {
            int retry = 0;

            if (host?.Any() == false || port?.Any() == false)
                throw new ArgumentNullException(@$"{nameof(host)}_{nameof(port)}");

            //
            //&& !message.StartsWith("9300"
            if (!message.StartsWith("9900") && !message.StartsWith("9300"))
                StatusCheck();


            Console.WriteLine(@$"发送: {message}");

            try
            {
                var res = await base.SendAsync(message, "gbk", "gbk", "\r");
                Console.WriteLine(res);
                return res;
            }
            catch (Exception e)
            {

                if (e.ToString().Contains("您的主机中的软件中止了一个已建立的连接。"))
                {
                    ReConnect();
                    return await SendAsync(message);
                }
                throw;

            }
            //}

            throw new Exception(@$"超过最大重试次数{retry}");
        }

        public MessageModel<bool> ReConnect() => Connect(host, port, true);


        public MessageModel<object> LendBook(string bookserial, string readerNo, string libraryAccount)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr =
                $@"11YN19960212   10051419960212   100514AO|AA{readerNo}|AB{bookserial}|CN{libraryAccount}|AC|AY3AZEDB7|";
            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = $"{e.ToString()}:{sendStr}";
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }

            


            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.Serial = bookserial;
            bookInfo.ShuldBackDate = message.Search("AH", "|");
            readerInfo.CardNo = readerNo;

            bookInfo.ScreenMsg = message.Search("AF", "|")?.Replace(bookserial, newValue: "");
            data.success = bookInfo.ScreenMsg.Contains("成功");
            bookInfo.PrintLine = message.Search("AG", "|");

            data.msg = data.success ? "借阅成功" : bookInfo.ScreenMsg ?? "未知错误";


            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }

        /// <summary>
        /// 归还书籍
        /// </summary>
        /// <param name="bookserial">书籍条码</param>
        /// <returns></returns>
        public MessageModel<object> BackBook(string bookserial, string libraryAccount)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"09N{DateTime.Now.ToString("yyyyMMddHHmmss")} AP|AOzhepl|AB{bookserial}|AC|AY1AZEFAE";
            var sendStr =
                $@"09N{DateTime.Now.ToString("yyyyMMdd")}    08005920150303    080059AP|AO|AB{bookserial}|AC|CN{libraryAccount}|BIN|AY1AZF0CC|";
            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(@$"归还返回字符串：{message}");


            data.success = message.Search("10", "YNN")?.ToInt() == 1;


            readerInfo.CardNo = message.Search("AA", "|");
            bookInfo.Serial = message.Search("AB", "|");
            bookInfo.LendDate = message.Search("CJ", "|");
            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.Status = "02" == message.Search("ST", "|") ? "在馆" : "借出"; // 02在馆 / 03借出
            bookInfo.PermanentLocation = message.Search("AQ", "|");
            bookInfo.MediaType = message.Search("CK", "|");
            bookInfo.CirculationStatus = message.Search("CT", "|");
            readerInfo.Owe = message.Search("CF", "|");
            bookInfo.Callno = message.Search("KC", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|")?.Replace(bookserial, "");
            bookInfo.PrintLine = message.Search("AG", "|");

            data.msg = data.success ? "归还书籍成功" : bookInfo.ScreenMsg ?? "未知错误";


            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            Console.WriteLine(@$"{ data.msg}   {data.success}");

            return data;
        }

        public MessageModel<object> CheckIdentity(string identity,string userType)
        {
            var data = new MessageModel<object>();
            var sendStr = $@"91{DateTime.Now.ToString("yyyyMMdd")}    081303Y         AO|XO{identity}|XK1|XT{userType}|AY1AZF4A6|";
            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                data.success = false;
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                data.success = false;
                return data;
            }

            data.success = message.Search("AC", "|") == "0";
            if (!data.success)
            {
                data.msg = "该身份证已被注册";
                return data;
            }

            data.msg = "查重成功";
            return data;
        }


        // 判断是否身份证
        public bool IsIdCard(string idCard)
        {
            Regex objReg = new Regex(@"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
            return objReg.IsMatch(idCard);
        }

        /// <summary>
        /// 查询读者信息
        /// </summary>
        /// <param name="readerNo"></param>
        /// <param name="readerPw">可为空，可做校验读者密码是否正确，读者登录使用</param>
        /// <returns></returns>
        public MessageModel<object> GetReaderInfo(string readerNo = null, string readerPw = null)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            Regex objReg = new Regex(@"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
            var getCardNoByIdCard = "";
            if (readerPw?.Any() != true) readerPw = null;
            var sendStr = "";
            string message = null;

            if (IsIdCard(readerNo))
            {
                sendStr =
                    $@"85{DateTime.Now.ToString("yyyyMMdd")}    081303Y         AO|XO{readerNo}|AD|AY1AZF4A6|XK13|";
                //try
                //{
                //    message = SendAsync(sendStr).Result;
                //}
                //catch (Exception e)
                //{
                //    data.msg = e.ToString();
                //    return data;
                //}

                //if (message == null)
                //{
                //    data.msg = "获取数据失败，返回信息为空";
                //    return data;
                //}

                //getCardNoByIdCard = message.Search("OX", "]]"); //获取返回的第一个读者证号
                //sendStr =
                //    $@"63000{DateTime.Now.ToString("yyyyMMdd")}    081303Y         AO|AA{getCardNoByIdCard}|AD{readerPw}|AY1AZF4A6|";
            }
            else
            {
                sendStr =
                    $@"63000{DateTime.Now.ToString("yyyyMMdd")}    081303Y         AO|AA{readerNo}|AD{readerPw}|AY1AZF4A6|";
            }
            Console.WriteLine(@$"GetReaderInfo_sendStr:{sendStr}");


            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }

            var hasReaderValid = "Y" == message.Search("BL", "|");
            var readerPwValid = "Y" == message.Search("CQ", "|");

            // 需要验证用户密码
            if (readerPw != null)
            {
                data.success = hasReaderValid && readerPwValid;
                data.msg = data.success ? "查询成功" : "用户密码错误";
            }
            // 只验证用户是否存在
            else
            {
                data.success = hasReaderValid;
                data.msg = data.success ? "查询成功" : "用户不存在";
            }

            readerInfo.CardNo = message.Search("AA", "|");
            readerInfo.Name = message.Search("AE", "|");
            readerInfo.Owe = message.Search("CF", "|");
            readerInfo.Moeny = message.Search("BV", "|");
            readerInfo.Prepay = message.Search("JE", "|");
            readerInfo.Depositrate = message.Search("XR", "|");
            readerInfo.Loanedvalue = message.Search("XC", "|");
            readerInfo.HoldItemsLimit = message.Search("BZ", "|");
            readerInfo.ReaderType = message.Search("XT", "|");
            readerInfo.Identity = message.Search("OX", "|");
            readerInfo.Enddate = message.Search("XD", "|");

            var books = message.Split("|AS");
            var addBooks = new List<string>();
            if (books.Length > 1)
            {
                for (int i = 1; i < books.Length; i++)
                {
                    var book = books[i];
                    // 最后一个AS的处理
                    if (books.Length == i + 1)
                    {
                        addBooks.Add(string.Join("", book.Take(book.IndexOf('|'))));
                    }
                    else
                        addBooks.Add(book);
                }
            }

            readerInfo.HoldItems = string.Join(",", addBooks); // ,分割多本


            readerInfo.OverdueItems = message.Search("AT", "|"); // ,分割多本
            readerInfo.AllItems = message.Search("AU", "|"); // ,割多本
            readerInfo.ReaderCodeForChs = message.Search(left: "AI", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|") + $@"发送字符串：{sendStr}" + $@"接收字符串：{message}";
            bookInfo.PrintLine = message.Search("AG", right: "|");

            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }

        /// <summary>
        /// 查询书籍信息
        /// </summary>
        /// <param name="bookserial"></param>
        /// <returns></returns>
        public MessageModel<object> GetBookInfo(string bookserial)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"17{DateTime.Now.ToString("yyyyMMdd")}    {DateTime.Now.ToString("HHmmss")}AOzzjh1|AB{bookserial}|ACilas|AY{Sip2Key}|";

            var sendStr = @$"1720230418    160609AO044120|AB{bookserial}|AC|AY4AZF3E6";

            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }
            //data.devMsg = message;
            Console.WriteLine($@"发送字符串：{sendStr}");
            Console.WriteLine($@"返回字符串：{message}");

            bookInfo.CirculationStatus = CirculationStatusStr[message.Search("18", "0001").ToInt()];
            bookInfo.ItemHolder = message.Search("AA", "|");
            bookInfo.Serial = bookserial;
            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.Author = message.Search("CF", "|");
            bookInfo.Isbn = message.Search("AK", "|");
            bookInfo.MediaType = message.Search("CK", "|");
            bookInfo.ItemProperties = message.Search("BV", "|");
            bookInfo.Callno = message.Search("KC", "|");
            bookInfo.PermanentLocation = message.Search("AQ", "|");
            bookInfo.CurrentLocation = message.Search("AP", "|");
            bookInfo.ReservationRdid = message.Search("BG", "|");
            bookInfo.Publisher = message.Search("PB", "|");
            bookInfo.Subject = message.Search("SJ", "|");
            bookInfo.Page = message.Search("PG", "|");
            bookInfo.ShelfNo = message.Search("KP", "|");
            bookInfo.ShuldBackDate = message.Search("AH", "|");
            bookInfo.LendDate = message.Search("CM", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|");
            bookInfo.PrintLine = message.Search("AG", "|");

            data.success = !bookInfo.ScreenMsg.Contains("不存在");
            data.msg = data.success ? "查询书籍信息成功" : bookInfo.ScreenMsg ?? "未知错误";

            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }

        public MessageModel<object> StatusCheck()
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"9900522.00AY1AZFCA1";

            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }


            bookInfo.ScreenMsg = "";

            data.msg = bookInfo.ScreenMsg;
            data.success = false;


            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }


        public MessageModel<object> AdminLogin(string admin, string pw, string libraryAccount = null)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = "9300CNzzjhs|COilas|CP11|AY1AZF660";

            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }
            Console.WriteLine($@"adminLogin发送字符串:{sendStr}");
            Console.WriteLine($@"adminLogin返回信息:{message}");

            Sip2Key = message.Substring(8);
            Console.WriteLine($@"获取到的AY:{Sip2Key}");
            StatusCheck();


            if (message.Search("94", "AF") == "1")
            {
                account = admin;
                password = pw;
            }


            bookInfo.ScreenMsg = "";

            data.msg = bookInfo.ScreenMsg;
            data.success = false;


            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }


        void AutoLogin()
        {
            if (!account.IsNullOrEmpty())
            {
                Console.WriteLine("AutoLogin");
                AdminLogin(account, password);
            }
        }

        /// <summary>
        /// 续借
        /// </summary>
        /// <param name="bookserial"></param>
        /// <param name="readerNo"></param>
        /// <returns></returns>
        public MessageModel<object> ContinueBook(string bookserial, string readerNo, string libraryAccount)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"29AO|AA{readerNo}|AD|AB{bookserial}|AC|CN{libraryAccount}|AY3AZEDB7|";

            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }

            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }


            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.ItemHolder = message.Search("AA", "|");
            bookInfo.Serial = bookserial;
            bookInfo.ShuldBackDate = message.Search("AH", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|");
            bookInfo.PrintLine = message.Search("AG", "|");

            data.msg = bookInfo.ScreenMsg;
            data.success = data.msg.Contains("成功"); // 成功

            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="bookserial"></param>
        /// <param name="readerNo"></param>
        /// <returns></returns>
        public MessageModel<object> RegiesterReader(RegiesterInfo readerInfo)
        {
            var data = new MessageModel<object>();
            Regex objReg = new Regex(@"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
            if (readerInfo == null)
            {
                data.msg = "readerInfo实体不可为空";
                return data;
            }

            if (!readerInfo.Identity.IsNullOrEmpty())
            {
                data = CheckIdentity(readerInfo.Identity,readerInfo.Type);
                if (!data.success)
                {
                    return data;
                }
            }

            var sendStr =
                @$"81YN20080828    15294220080828    152942AO|AA{readerInfo.CardNo}|AD{readerInfo.Pw}|AE{readerInfo.Name}|AM{readerInfo.CreateReaderLibrary}|BP{readerInfo.Phone}|BD{readerInfo.Addr}|XO{readerInfo.Identity}|XT{readerInfo.Type}|BV{readerInfo.Moeny}|XM{(readerInfo.Sex ? "M" : "F")}|XK01|AY1AZB92E";

            string message = null;
            try
            {
                message = SendAsync(sendStr).Result;
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }



            if (message == null)
            {
                data.msg = "获取数据失败，返回信息为空";
                return data;
            }

            try
            {
                data.msg = message.Search("AF", "|");
            }
            catch (Exception)
            {
                data.msg = message.Substring(message.IndexOf("AF"), message.Length - 1);
            }

            if (string.IsNullOrEmpty(data.msg))
            {
                data.msg = message.Substring(message.IndexOf("AF"), message.Length - message.IndexOf("AF"));
            }
            data.success = data.msg?.Contains("成功") ?? false; // 成功
            //data.msg += "发送str:" + sendStr;

            return data;
        }

        /// <summary>
        /// 自助办证(用于web进行反射调用)
        /// </summary>
        /// <param name="readerInfo"></param>
        /// <returns></returns>
        public MessageModel<object> RegiesterReader(object readerInfo)
        {
            return RegiesterReader(readerInfo.JsonMapTo<RegiesterInfo>());
        }
    }
}