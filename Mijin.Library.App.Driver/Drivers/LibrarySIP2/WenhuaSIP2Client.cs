using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Maps;
using Bing.Extensions;

namespace Mijin.Library.App.Driver
{
    [ComVisible(true)]
    public class WenhuaSIP2Client : baseTcpClient, IWenhuaSIP2Client
    {
        public WenhuaSIP2Client()
        {
        }

        public WenhuaSIP2Client(string host, string port) : base(host, port)
        {
        }

        private static string[] CirculationStatusStr =
        {
            "分编", "入藏", "在装订", "已合并", "修补", "丢失", "剔除", "普通借出", "预约", "阅览借出", "预借", "互借", "闭架借阅", "赠送", "交换出", "调拨",
            "转送", "临时借出", "未知状态"
        };


        public string account { get; set; }

        public string pw { get; set; }


        /// <summary>
        /// 登录文化socket
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public MessageModel<object> Login(string account, string pw)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"93  CN{account}|CO{pw}|CP|AY1AZF77E";
            string message = null;

            try
            {
                message = SendAsync(sendStr, true).Result;
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

            data.success = message.Search("94", "|")?.ToInt() == 1;
            data.msg = data.success ? "登录成功" : "登录失败";

            if (data.success)
            {
                this.account = account;
                this.pw = pw;
            }

            return data;
        }

        internal async Task<string> SendAsync(string message, bool isLogin)
        {
            int retry = 0;

            if (host.IsEmpty() || port.IsEmpty())
                throw new ArgumentNullException(@$"{nameof(host)}_{nameof(port)}");

            // 登陆接口只执行一次
            if (isLogin) retry = 2;

            while (++retry < 3)
            {
                try
                {
                    if (!Connected)
                    {
                        throw new Exception("未连接到socket");
                    }

                    var tcpClient = base.GetTcpClient();
                    var stream = tcpClient.GetStream();

                    // 需要在字符串后面加\r\n
                    var sendBytes = UTF8Encoding.UTF8.GetBytes(message + "\r\n");

                    stream.Write(sendBytes, 0, sendBytes.Length);
                    stream.Flush();

                    var data = new byte[65536];
                    int len = await stream.ReadAsync(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(data.Take(len).ToArray());
                }
                catch (Exception e)
                {
                    if (e.Message == "未连接到socket")
                        throw;

                    ReConnect();
                }
            }

            throw new Exception(@$"超过最大重试次数{retry}");
        }

        internal override Task<string> SendAsync(string message) => SendAsync(message, false);

        public void ReConnect()
        {
            var connectRes = Connect(host, port);

            if (!connectRes.success) return;

            Login(account, pw);
        }

        /// <summary>
        /// 借阅书籍
        /// </summary>
        /// <param name="bookserial">书籍条码</param>
        /// <param name="readerNo">读者卡号</param>
        /// <returns></returns>
        public MessageModel<object> LendBook(string bookserial, string readerNo)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr =
                $@"11YN{DateTime.Now.ToString("yyyyMMddHHmmss")}   {DateTime.Now.ToString("yyyyMMddHHmmss")}AOzhepl|AA{readerNo}|AB{bookserial}|AC|AY|BOY|BIN|AY4AZE86F";

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

            data.success = message.Search("12", "YYY")?.ToInt() == 1;


            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.Serial = bookserial;
            bookInfo.ShuldBackDate = message.Search("AH", "|");
            readerInfo.CardNo = readerNo;

            bookInfo.ScreenMsg = message.Search("AF", "|")?.Replace(bookserial, "");
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
        public MessageModel<object> BackBook(string bookserial)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"09N{DateTime.Now.ToString("yyyyMMddHHmmss")} AP|AOzhepl|AB{bookserial}|AC|AY1AZEFAE";
            var sendStr =
                $@"09N{DateTime.Now.ToString("yyyyMMdd")}    08005920150303    080059AP|AOzhepl|AB{bookserial}|AC|AY1AZEFAE";
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

            data.success = message.Search("10", "YYN")?.ToInt() == 1;


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

            return data;
        }

        /// <summary>
        /// 查询读者信息
        /// </summary>
        /// <param name="readerNo"></param>
        /// <param name="readerPw">可为空，可做校验读者密码是否正确，读者登录使用</param>
        /// <returns></returns>
        public MessageModel<object> GetReaderInfo(string readerNo, string readerPw = null)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"63001{DateTime.Now.ToString("yyyyMMddHHmmss")} 1234567890 AOzhepl|AA{readerNo}|BAAA|AD{readerPw}|AY3AZF1FA"; //HHmmss
            var sendStr =
                $@"63001{DateTime.Now.ToString("yyyyMMdd")}    081303Y         AOzhepl|AA{readerNo}|AD{readerPw}|AY3AZF1FA";
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
            readerInfo.Enddate = message.Search("XD", "|");
            readerInfo.HoldItems = message.Search("AS", "|"); // ,分割多本
            readerInfo.OverdueItems = message.Search("AT", "|"); // ,分割多本
            readerInfo.AllItems = message.Search("AU", "|"); // ,割多本
            readerInfo.ReaderCodeForChs = message.Search("AI", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|");
            bookInfo.PrintLine = message.Search("AG", "|");

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
            var sendStr = $@"17{DateTime.Now.ToString("yyyyMMddHHmmss")}AO|AB{bookserial}|AC|AY4AZF455";

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

            bookInfo.CirculationStatus = CirculationStatusStr[message.Search("18", "0001").ToInt()];
            bookInfo.ItemHolder = message.Search("AA", "|");
            bookInfo.Serial = bookserial;
            bookInfo.Title = message.Search("AJ", "|");
            bookInfo.Author = message.Search("AW", "|");
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
            bookInfo.LendDate = message.Search("CJ", "|");

            bookInfo.ScreenMsg = message.Search("AF", "|");
            bookInfo.PrintLine = message.Search("AG", "|");

            data.success = !bookInfo.ScreenMsg.Contains("不存在");
            data.msg = data.success ? "查询书籍信息成功" : bookInfo.ScreenMsg ?? "未知错误";

            dic["bookInfo"] = bookInfo;
            dic["readerInfo"] = readerInfo;
            data.response = dic;

            return data;
        }

        /// <summary>
        /// 续借
        /// </summary>
        /// <param name="bookserial"></param>
        /// <param name="readerNo"></param>
        /// <returns></returns>
        public MessageModel<object> ContinueBook(string bookserial, string readerNo)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"29AO|AA{readerNo}|AD|AB{bookserial}|AJ";

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
        /// 自助办证
        /// </summary>
        /// <param name="readerInfo"></param>
        /// <returns></returns>
        public MessageModel<object> RegiesterReader(RegiesterInfo readerInfo)
        {
            var data = new MessageModel<object>();

            if (readerInfo == null)
            {
                data.msg = "readerInfo实体不可为空";
                return data;
            }

            var sendStr =
                @$"81{DateTime.Now.ToString("yyyyMMddHHmmss")}    144920AOYB|AA{readerInfo.CardNo}|AD{readerInfo.Pw}|AE{readerInfo.Name}|AM{readerInfo.CreateReaderLibrary}|BP{readerInfo.Phone}|BD{readerInfo.Addr}|XO{readerInfo.Identity}|XT{readerInfo.Type}|BV{readerInfo.Moeny}|XM{(readerInfo.Sex ? "1" : "0")}|XK01|AY1AZB92E";

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