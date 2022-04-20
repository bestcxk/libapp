using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsUtil.Maps;
using Mijin.Library.App.Driver.Drivers.LibrarySIP2.Models;
using Mijin.Library.App.Driver.Drivers.LibrarySIP2.Models.JpSip2Response;
using Mijin.Library.App.Model;
using SIP2Client.Codes;
using SIP2Client.Entities;
using SIP2Client.Entities.Sip2Request;
using SIP2Client.Entities.Sip2Response;

namespace Mijin.Library.App.Driver.Drivers.LibrarySIP2
{
    /// <summary>
    /// 金盘Sip2
    /// </summary>
    public class JpSip2Client : IJpSip2Client
    {
        public Sip2Client Sip2Client { get; set; } = new Sip2Client();

        public MessageModel<string> Init(Sip2Model sip2Info)
        {
            var success = Sip2Client.Init(sip2Info);
            return new MessageModel<string>()
            {
                msg = success ? "初始化成功" : "初始化失败",
                success = success
            };
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sip2Info">初始化信息</param>
        /// <returns></returns>
        public MessageModel<string> Init(object sip2Info) => Init(sip2Info.JsonMapTo<Sip2Model>());

        /// <summary>
        /// 获取书籍信息
        /// </summary>
        /// <param name="bookIdentifier">图书条码</param>
        /// <param name="institutionId">图书馆名称</param>
        /// <returns></returns>
        public MessageModel<object> GetBookInfo(string bookIdentifier, string institutionId)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            data.msg = "获取数据失败，返回信息为空";

            //获取返回信息
            var result = Sip2Client.GetResult<JpSip2BookResponse, Sip2BookRequest> (new Sip2BookRequest()
            {
                InstitutionId = institutionId,
                BookBarcode = bookIdentifier
            });

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                bookInfo.Serial = result.Info.BookIdentifier;
                bookInfo.Title = result.Info.TitleIdentifier;
                bookInfo.Author = result.Info.Author;
                bookInfo.Callno = result.Info.PermanentLocation;
                bookInfo.CurrentLocation = result.Info.CurrentLocation;
                bookInfo.ReservationRdid = result.Info.Owner;
                bookInfo.Publisher = result.Info.BookConcern;
                bookInfo.ShuldBackDate = result.Info.DueDate;
                bookInfo.LendDate = result.Info.RecallDate;

                bookInfo.ScreenMsg = result.Info.ScreenMessage;
                bookInfo.PrintLine = result.Info.PrintLine;
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
                dic["bookInfo"] = bookInfo;
                dic["readerInfo"] = readerInfo;
                data.response = dic;
            }

            return data;
        }

        /// <summary>
        /// 获取读者信息
        /// </summary>
        /// <param name="readerIdentifier">读者证号</param>
        /// <param name="institutionId">图书馆名称</param>
        /// <returns></returns>
        public MessageModel<object> GetReaderInfo(string readerIdentifier, string institutionId)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>
            {
                msg = "获取数据失败，返回信息为空"
            };

            //获取返回信息
            var result = Sip2Client.GetResult<JpSip2ReaderResponse, Sip2ReaderRequest>(new Sip2ReaderRequest()
            {
                InstitutionId = institutionId,
                ReaderBarcode = readerIdentifier
            });

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                readerInfo.CardNo = result.Info.ReaderIdentifier;
                readerInfo.Name = result.Info.ReaderName;
                readerInfo.Owe = result.Info.ArReaRage;
                readerInfo.Moeny = result.Info.FeeAmount;
                readerInfo.ReaderCodeForChs = result.Info.CardType;
                readerInfo.HoldItems = result.Info.ChargedBook;
                readerInfo.OverdueItems = result.Info.OverdueBook;

                bookInfo.ScreenMsg = result.Info.ScreenMessage;
                bookInfo.PrintLine = result.Info.PrintLine;
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
                dic["bookInfo"] = bookInfo;
                dic["readerInfo"] = readerInfo;
                data.response = dic;
            }

            return data;
        }

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="bookIdentifier">图书条码</param>
        /// <param name="readerIdentifier">读者证号</param>
        /// <param name="institutionId">图书馆名称</param>
        /// <returns></returns>
        public MessageModel<object> LendBook(string bookIdentifier, string readerIdentifier, string institutionId)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            data.msg = "获取数据失败，返回信息为空";

            //获取返回信息
            var result = Sip2Client.GetResult<Sip2LendBookResponse, Sip2LendBookRequest>(new Sip2LendBookRequest()
            {
                BookIdentifier = bookIdentifier,
                ReaderIdentifier = readerIdentifier,
                InstitutionId =  institutionId
            });

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                bookInfo.Title = result.Info.TitleIdentifier;
                bookInfo.Serial = result.Info.BookIdentifier;
                bookInfo.ShuldBackDate = result.Info.DueDate;
                readerInfo.CardNo = result.Info.ReaderIdentifier;

                bookInfo.ScreenMsg = result.Info.ScreenMessage;
                bookInfo.PrintLine = result.Info.PrintLine;
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
                dic["bookInfo"] = bookInfo;
                dic["readerInfo"] = readerInfo;
                data.response = dic;
            }

            return data;
        }

        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="bookIdentifier">图书条码</param>
        /// <param name="institutionId">图书馆名称</param>
        /// <returns></returns>
        public MessageModel<object> BackBook(string bookIdentifier, string institutionId)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            data.msg = "获取数据失败，返回信息为空";

            //获取返回信息
            var result = Sip2Client.GetResult<Sip2ReturnBookResponse, Sip2ReturnBookRequest>(new Sip2ReturnBookRequest()
            {
                BookIdentifier = bookIdentifier,
                InstitutionId = institutionId
            });

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                readerInfo.CardNo = result.Info.ReaderIdentifier;
                bookInfo.Serial = result.Info.BookIdentifier;
                bookInfo.Title = result.Info.TitleIdentifier;
                bookInfo.PermanentLocation = result.Info.PermanentLocation;
                bookInfo.MediaType = result.Info.MediaType;

                bookInfo.ScreenMsg = result.Info.ScreenMessage;
                bookInfo.PrintLine = result.Info.PrintLine;
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
                dic["bookInfo"] = bookInfo;
                dic["readerInfo"] = readerInfo;
                data.response = dic;
            }

            return data;
        }

        /// <summary>
        /// 续借
        /// </summary>
        /// <param name="bookIdentifier">图书条码</param>
        /// <param name="readerIdentifier">读者证号</param>
        /// <param name="institutionId">图书馆名称</param>
        /// <returns></returns>
        public MessageModel<object> ReNewBook(string bookIdentifier, string readerIdentifier, string institutionId)
        {
            var dic = new Dictionary<string, object>();
            var bookInfo = new SIP2BookInfo();
            var readerInfo = new SIP2ReaderInfo();
            var data = new MessageModel<object>();
            data.msg = "获取数据失败，返回信息为空";

            //获取返回信息
            var result = Sip2Client.GetResult<Sip2ReNewResponse, Sip2ReNewRequest>(new Sip2ReNewRequest()
            {
                BookIdentifier = bookIdentifier,
                ReaderIdentifier = readerIdentifier,
                InstitutionId = institutionId
            });

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                bookInfo.Title = result.Info.TitleIdentifier;
                bookInfo.ItemHolder = result.Info.ReaderIdentifier;
                bookInfo.Serial = result.Info.BookIdentifier;
                bookInfo.ShuldBackDate = result.Info.DueDate;

                bookInfo.ScreenMsg = result.Info.ScreenMessage;
                bookInfo.PrintLine = result.Info.PrintLine;
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
                dic["bookInfo"] = bookInfo;
                dic["readerInfo"] = readerInfo;
                data.response = dic;
            }

            return data;
        }

        /// <summary>
        /// 自助办证
        /// </summary>
        /// <param name="readerRegister">自助办证</param>
        /// <returns></returns>
        public MessageModel<object> RegisterReader(Sip2ReaderRegisterRequest readerRegister)
        {
            var data = new MessageModel<object>();
            data.msg = "获取数据失败，返回信息为空";

            //获取返回信息
            var result = Sip2Client.GetResult<Sip2ReaderRegisterResponse, Sip2ReaderRegisterRequest>(readerRegister);

            //如果成功则赋值
            if (result.MessageCode.Contains("成功"))
            {
                data.msg = result.MessageCode;
                data.success = result.MessageCode.Contains("成功");
            }

            return data;
        }

        /// <summary>
        /// 自助办证(用于web进行反射调用)
        /// </summary>
        /// <param name="sip2Info">自助办证</param>
        /// <returns></returns>
        public MessageModel<object> RegisterReader(object sip2Info) => RegisterReader(sip2Info.JsonMapTo<Sip2ReaderRegisterRequest>());
    }


}
