using System;
using Mijin.Library.App.Driver.Drivers.LibrarySIP2;
using SIP2Client.Codes;
using SIP2Client.Entities;
using SIP2Client.Entities.Sip2Request;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    /// <summary>
    /// 金盘sip2测试
    /// </summary>
    public class JpSip2ClientTest
    {
        #region 参数

        private JpSip2Client _jpSip2Client = new()
        {
            Sip2Client = new Sip2Client()
        };
        /// <summary>
        /// 读者条码
        /// </summary>
        private string readerCode { get; set; } = "431000213972";
        /// <summary>
        /// 图书条码
        /// </summary>
        private string bookCode { get; set; } = "431202540693";
        /// <summary>
        /// 馆代码
        /// </summary>
        private string AO = "";
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            _jpSip2Client.Init(new Sip2Model() { Ip = "192.168.0.159", Port = "6789", Encoding = "gbk" });
        }

        #endregion

        /// <summary>
        /// 查询图书信息
        /// </summary>
        [Fact]
        public void GetBookInfo_Test()
        {
            Init();
            var result = _jpSip2Client.GetBookInfo(bookCode, AO);
            Assert.Contains("成功", result.msg);
        }

        /// <summary>
        /// 查询读者信息
        /// </summary>
        [Fact]
        public void GetReaderInfo_Test()
        {
            Init();
            var result = _jpSip2Client.GetReaderInfo(readerCode, AO);
            Assert.Contains("成功", result.msg);
        }

        /// <summary>
        /// 借书
        /// </summary>
        [Fact]
        public void LendBook_Test()
        {
            Init();
            var result = _jpSip2Client.LendBook(bookCode, "TEST011", AO);
            Assert.Contains("成功", result.msg);
        }

        /// <summary>
        /// 还书
        /// </summary>
        [Fact]
        public void BackBook_Test()
        {
            Init();
            var result = _jpSip2Client.BackBook(bookCode, AO);
            Assert.Contains("成功", result.msg);
        }

        /// <summary>
        /// 续借
        /// </summary>
        [Fact]
        public void ReNewBook_Test()
        {
            Init();
            var result = _jpSip2Client.ReNewBook(bookCode, readerCode, AO);
            Assert.Contains("成功", result.msg);
        }

        /// <summary>
        /// 自助办证
        /// </summary>
        [Fact]
        public void RegisterReader_Test()
        {
            Init();

            var readerRegister = new Sip2ReaderRegisterRequest()
            {
                InstitutionId = AO,
                ReaderIdentifier = "TEST016",
                ReaderPassword = "",
                ReaderName = "金盘测试12",
                ReaderOpenAccountLibrary = AO,
                CertificateDeposit = "1000",
                DateOfBirth = DateTime.Now.ToString("yyyyMMdd"),
                OperationMode = "01",
                ReaderType = "读者"
            };

            var result = _jpSip2Client.RegisterReader(readerRegister);
            Assert.Contains("成功", result.msg);
        }
    }
}
