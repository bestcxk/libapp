using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Maps;

namespace Mijin.Library.App.Driver
{
    [ComVisible(true)]
    public class WjSIP2Client : baseTcpClient, IWjSIP2Client
    {
        static WjSIP2Client()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public WjSIP2Client()
        {
        }

        public WjSIP2Client(string host, string port) : base(host, port)
        {

        }

        private static string[] CirculationStatusStr = { "分编", "入藏", "在装订", "已合并", "修补", "丢失", "剔除", "普通借出", "预约", "阅览借出", "预借", "互借", "闭架借阅", "赠送", "交换出", "调拨", "转送", "临时借出", "未知状态" };

        /// <summary>
        /// 连接socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public override MessageModel<bool> Connect(string host, string port)
        {
            return base.Connect(host, port);
        }

        /// <summary>
        /// 登录文化socket
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        public MessageModel<object> Login(string account, string pw, Int64 cp)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"93  CN{account}|CO{pw}|CP{cp}|AY1AZF77E";
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
            data.success = message.Contains("登录成功");
            data.msg = data.success ? "登录成功" : "登录失败";
            return data;
        }
        internal async override Task<string> SendAsync(string message)
        {
            if (!Connected)
            {
                throw new Exception("未连接到socket");
            }

            var tcpClient = base.GetTcpClient();
            var stream = tcpClient.GetStream();

            // 需要在字符串后面加\r\n
            var sendBytes = Encoding.GetEncoding("GBK").GetBytes(message + "\r");

            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();

            var data = new byte[1024 * 1024];
            int len = await stream.ReadAsync(data, 0, data.Length);
            return Encoding.GetEncoding("GBK").GetString(data.Take(len).ToArray());
        }

        /// <summary>
        /// 借阅书籍
        /// </summary>
        /// <param name="bookserial">书籍条码</param>
        /// <param name="readerNo">读者卡号</param>
        /// <returns></returns>
        public MessageModel<object> LendBook(string bookserial, string readerNo, string libCode)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"11YN19960212    10051419960212    100514|AO{libCode}|AA{readerNo}|AB{bookserial}|AY3AZEDB7";
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
        public MessageModel<object> BackBook(string bookserial, string libCode)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"09N{DateTime.Now.ToString("yyyyMMddHHmmss")} AP|AOzhepl|AB{bookserial}|AC|AY1AZEFAE";
            var sendStr = $@"09N{DateTime.Now.ToString("yyyyMMdd")}    08005920150303    080059AP|AO{libCode}|AB{bookserial}|AC|AY1AZEFAE";
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
        public MessageModel<object> GetReaderInfo(string readerNo = "", string identity = "", string libCode = "", string readerPw = "")
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"63001{DateTime.Now.ToString("yyyyMMddHHmmss")} 1234567890 AOzhepl|AA{readerNo}|BAAA|AD{readerPw}|AY3AZF1FA"; //HHmmss
            var sendStr = "";
            if (!readerNo.IsEmpty())
                sendStr = $@"63019{DateTime.Now.ToString("yyyyMMdd")} 102456 Y|AA{readerNo}|AO{identity}|AD{readerPw}|AY1AZEEDA";
            else
                sendStr = @$"8520190614    163548|AO{libCode}|XO{identity}|XK13|AY9AZEDE4";


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
            readerInfo.HoldItems = message.Search("AS", "|");  // ,分割多本
            readerInfo.OverdueItems = message.Search("AT", "|");// ,分割多本
            readerInfo.AllItems = message.Search("AU", "|");// ,割多本
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
        public MessageModel<object> GetBookInfo(string bookserial,string libCode)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            var sendStr = $@"17{DateTime.Now.ToString("yyyyMMddHHmmss")}AO{libCode}|AB{bookserial}|AC|AY4AZF455";

            //sendStr = @$"1720080828    AO|AB{bookserial}|AY1AZF7E3";
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
        public MessageModel<object> ContinueBook(string bookserial, string readerNo, string libCode)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            //var sendStr = $@"29|AO{libCode}|AA{readerNo}|AD|AB{bookserial}|AJ";

            var sendStr = $@"29YN{DateTime.Now.ToString("yyyyMMdd")}    100514{DateTime.Now.ToString("yyyyMMdd")}    100514|AO{libCode}|AA{readerNo}|AB{bookserial}|AY3AZEDB7";
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
        /// 获取读者信息
        /// </summary>
        /// <param name="readerInfo"></param>
        /// <returns></returns>
        public MessageModel<object> GerReaderInfo(RegiesterInfo readerInfo)
        {
            var data = new MessageModel<object>();

            if (readerInfo == null)
            {
                data.msg = "readerInfo实体不可为空";
                return data;
            }

            var sendStr = @$"91{DateTime.Now.ToString("yyyyMMddHHmmss")}    163548|AA{readerInfo.CardNo}|XO{readerInfo.Identity}|AO{readerInfo.CreateReaderLibrary}|XK{(!readerInfo.CardNo.IsEmpty() ? 0 : 1)}|AY9AZEDE4";

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

            data.success = message.Search("AC", "|") == "0";

            data.msg = data.success ? "" : "已存在重复用户";
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
            var checkRes = GerReaderInfo(readerInfo);
            if (!checkRes.success)
            {
                checkRes.msg = "已存在重复用户";
                return checkRes;

            }

            var sendStr = @$"81{DateTime.Now.ToString("yyyyMMddHHmmss")}    100514|AA{readerInfo.CardNo}|AD{readerInfo.Pw}|AE{readerInfo.Name}|AO{readerInfo.CreateReaderLibrary}|AM{readerInfo.CreateReaderLibrary}|BP{readerInfo.Phone}|BD{readerInfo.Addr}|XO{readerInfo.Identity}|XH{readerInfo.Birth}|XT02|BV{readerInfo.Moeny}|XM{(readerInfo.Sex ? "1" : "0")}|XK{readerInfo.Type}|AY3AZEDB7";

            //var testStr = "8120190522    100514|AO800021250301|AA182102|AD654321|AE533813|AM800021250301|BP18262605389|MP18262608976|BE27488522@qq.com|BD美国白宫|XO320682199504145512|XT2|BV100|XB|XH19950414|XN汉|XP|XF|XD|XE20190621|XM1|XA|XK01|AY3AZEDB7";

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
