using Bing.Extensions;
using Mijin.Library.App.Driver.Drivers.Sudo;
using Mijin.Library.App.Model;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Util.Logs;
using Util.Logs.Extensions;

namespace Mijin.Library.App.Driver
{

    public class SudoQrcode
    {

        public string OriginCode { get; set; }

        public string DecodeBase64Code { get; set; }

        public UserInfo UserInfo { get; set; }



    }

    public class UserInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string idcard { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// base64图片
        /// </summary>
        public string photo { get; set; }
    }


    public class Sudo : ISudo
    {
        private readonly ISystemFunc _systemFunc;
        
        object lockobj = new object();

        static bool isFirst = true;

        int reTry = 1;

        private bool isWathing = false;
        private bool isNormalQrCode = false;

        public event Action<WebViewSendModel<SudoQrcode>> OnSudoQrcode;


        public Sudo(ISystemFunc systemFunc)
        {
            _systemFunc = systemFunc;

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(500);
                        if (!isWathing)
                            continue;


                        var res = ReadQrcode();

                        if (!res.success)
                            continue;

                        if (res.response.IsEmpty())
                            continue;

                        if (!isNormalQrCode)
                        {
                            var code = res.response.Replace("|", "+"); // 

                            var data = SudoEncrypt.AesDecrypt(code, "tecsun1234567890", Encoding.UTF8);

                            var sudoQrcode = new SudoQrcode();


                            sudoQrcode.DecodeBase64Code = data;
                            sudoQrcode.OriginCode = res.response;

                            try
                            {
                                sudoQrcode.UserInfo = JsonConvert.DeserializeObject<UserInfo>(sudoQrcode.DecodeBase64Code);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            OnSudoQrcode?.Invoke(new WebViewSendModel<SudoQrcode>()
                            {
                                method = nameof(OnSudoQrcode),
                                response = sudoQrcode,
                                success = true,
                                msg = "获取成功"
                            });
                        }
                        else
                        {
                            OnSudoQrcode?.Invoke(new WebViewSendModel<SudoQrcode>()
                            {
                                method = nameof(OnSudoQrcode),
                                response = new SudoQrcode()
                                {
                                    OriginCode = res.response
                                },
                                success = true,
                                msg = "获取成功"
                            });
                        }
                        

                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(@$"Sudo_Task：{e}");
                    }

                }
            });
        }

        static Sudo()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private bool isInit = false;

        public MessageModel<string> Connect(Int64 port, Int64 baud)
        {
            var res = new MessageModel<string>();

            if (isInit)
            {
                res.msg = "已初始化，无需再次初始化";
                res.success = true;
                return res;
            }


            try
            {
                SodoWinSDKHandle.Sodo_InitSerialPort((int)port, (int)baud);
                SodoWinSDKHandle.Sodo_Stop();
                int ret = SodoWinSDKHandle.Sodo_Start();
                if (ret == (int)STATUS_CODE.BASE_SUCCESS)
                {
                    res.success = true;
                    res.msg = "操作成功";

                    isInit = true;
                }
            }
            catch (Exception e)
            {
                e.Log(Log.GetLog());
                res.success = false;
                res.msg = "操作失败";
            }


            return res;
        }

        public MessageModel<IdentityInfo> ReadIdentity()
        {
            var res = new MessageModel<IdentityInfo>();

            int i = 0;
            while (i++ <= reTry && !res.success)
            {
                SodoWinSDKHandle.Sodo_Start();
                try
                {
                    TRADE_SFZ_OP_PARAM ptrTradeRecord = new TRADE_SFZ_OP_PARAM();
                    ptrTradeRecord.cmdop1 = 0x03;
                    ptrTradeRecord.cmdop2 = 0x00;

                    int ret = SodoWinSDKHandle.Sodo_Sfz_Process(ref ptrTradeRecord);

                    if (ret == (int)STATUS_CODE.BASE_SUCCESS)
                    {
                        //  showMsg("支付成功！")
                        try
                        {
                            var reader = new IdentityInfo();

                            ID2Parser parser = new ID2Parser(ptrTradeRecord.recvBuf, 3);
                            ID2Txt idText = parser.ParseText();

                            try
                            {
                                var bytes = parser.ParsePic();
                                using var stream = new MemoryStream(bytes);
                                var bitmap = new Bitmap(stream);

                                reader.FacePicBase64 = bitmap.ToBase64String(ImageFormat.Jpeg);
                            }
                            catch (Exception e)
                            {
                                e.Log(Log.GetLog());
                            }

                            reader.Country = idText.mNational;
                            reader.Name = idText.mName;
                            reader.Identity = idText.mID2Num;
                            reader.Birth = @$"{idText.mBirthYear}-{idText.mBirthMonth}-{idText.mBirthDay}";
                            reader.Addr = idText.mAddress;
                            reader.Sex = idText.mGender;

                            res.success = true;
                            res.response = reader;
                            res.msg = "获取成功";
                            return res;
                        }
                        catch (Exception e)
                        {
                            e.Log(Log.GetLog());
                            res.msg = "身份证解析失败";
                        }
                    }
                    else
                    {
                        res.msg = "身份证读取失败";
                    }
                }
                catch (Exception e)
                {
                    e.Log(Log.GetLog());
                    res.msg = "身份证解析失败";
                }

                if (res.success)
                    return res;

                try
                {
                    var identityRes = Read_SSC(true);
                    if (identityRes.success)
                    {
                        res.response = new IdentityInfo()
                        {
                            Identity = identityRes?.response?.Split("|")[0],
                            Name = identityRes?.response?.Split("|")[1],
                            Birth = new DateTime(1900, 1, 1).ToString("yyyy-M-d dddd")
                        };

                        res.success = identityRes.success;
                        res.msg = "读取身份证成功";
                    }
                    return res;
                }
                catch (Exception e)
                {
                }
            }

            return res;
        }

        public MessageModel<string> Read_SSC(bool getAllStr = false)
        {
            var res = new MessageModel<string>();

            int i = 0;
            while (i++ <= reTry && !res.success)
            {
                if (_systemFunc.ClientSettings.SudoPSAMMode)
                {

                    if (isFirst)
                    {
                        isFirst = false;
                        var identityRes = ReadIdentity();
                        if (identityRes.success)
                        {
                            res.success = identityRes.success;
                            res.msg = identityRes.msg;
                            res.response = identityRes?.response?.Identity;
                            return res;
                        }
                    }

                    SodoWinSDKHandle.Sodo_Start();
                    STR_SB_INFO ptrTradeRecord = new STR_SB_INFO();
                    int ret = SodoWinSDKHandle.Sodo_SB_Process(ref ptrTradeRecord);
                    if (ret == (int)STATUS_CODE.BASE_SUCCESS)
                    {
                        res.success = true;

                        var str = Encoding.GetEncoding("GB18030")
                            .GetString(ptrTradeRecord.recvBuf, 0, ptrTradeRecord.lenrecv);

                        res.response = (getAllStr ? str : str.Split("|").First())?.Replace("\0", "");
                        res.msg = "获取成功";
                        return res;
                    }
                    else
                    {
                        res.msg = "读取失败";
                    }
                }
                else
                {
                    SodoWinSDKHandle.Sodo_Stop();
                    var bytes = new byte[1024];
                    var code = TSCardDriver.iInitParms(0, null, null, bytes);

                    if (code == 0)
                    {
                        bytes = new byte[1024 * 1024];
                        try
                        {
                            code = TSCardDriver.iReadCardBasExt(3, bytes);
                        }
                        catch (Exception e)
                        {
                            res.msg = e.ToString();
                            return res;
                        }

                        if (code == 0)
                        {
                            var str = Encoding.GetEncoding("GB18030").GetString(bytes);
                            var data = getAllStr ? str : str.Split("|").First();
                            res.response = data?.Replace("\0", "");
                            res.success = true;
                            res.msg = "获取成功";
                            return res;
                        }
                        else
                        {
                            res.msg = "读社保卡失败";
                        }
                    }
                    else
                    {
                        res.msg = "初始化参数失败";
                    }
                }
            }


            return res;
        }

        class QrcodeType
        {
            public string cardType { get; set; }
            public string Id { get; set; }
        }

        public MessageModel<string> ReadQrcode()
        {
            var res = new MessageModel<string>();

            STR_TRANS_OP_PARAM ptrTradeRecord = new STR_TRANS_OP_PARAM();
            ptrTradeRecord.transType = 0x60;
            ptrTradeRecord.transTime = new byte[] { 0x012, 0x12, 0x12 };
            ptrTradeRecord.transDate = new byte[] { 0x20, 0x12, 0x12 };
            ptrTradeRecord.transNo = new byte[] { 0x11, 0x22, 0x33, 0x44 };
            ptrTradeRecord.transAmt = 1;
            int ret = SodoWinSDKHandle.Sodo_RequestCardAndQrcode(ref ptrTradeRecord);
            if (ret == (int)STATUS_CODE.BASE_SUCCESS)
            {

                var originData = Encoding.ASCII.GetString(
                    ptrTradeRecord.transOutBuf,
                    0, ptrTradeRecord.lenTransOutbuf);

                var qrcode = JsonConvert.DeserializeObject<QrcodeType>(originData);

                if (qrcode.Id.IsEmpty())
                {
                    res.msg = "获取失败";
                    return res;
                }

                if(qrcode.cardType == "1")
                {
                    res.response = qrcode.Id.Substring(6, 8);
                }
                else
                {
                    res.response = qrcode.Id.DecodeBase64();
                }
                res.success = true;
                res.msg = "获取成功";
                return res;
            }
            else
            {
                res.msg = "获取失败";
            }

            return res;
        }


        public MessageModel<string> StartWatchQrcode(bool normalCode=false)
        {
            isWathing = true;
            isNormalQrCode = normalCode;

            return new MessageModel<string>()
            {
                msg = "操作成功",
                success = true
            };
        }

        public MessageModel<string> StartWatchQrcode()
        {
            return StartWatchQrcode();
        }

        public MessageModel<string> StopWatchQrcode()
        {
            isWathing = false;
            isNormalQrCode = false;
            return new MessageModel<string>()
            {
                msg = "操作成功",
                success = true
            };
        }
        
        public MessageModel<string> ReadinternalInfo()
        {
            MessageModel<string> res = new MessageModel<string>();
                lock (lockobj)
                {
                    SodoWinSDKHandle.Sodo_Start();
                    try
                    {
                        IntPtr returnInfo2 = Marshal.AllocHGlobal(1024);
                        IntPtr inbuf2 = Marshal.AllocHGlobal(1024);
                        var ret = SodoWinSDKHandle.Sodo_SB_Psam_Process_GetAllInfo(ref returnInfo2, ref inbuf2, 2);
                        if (ret == (int)STATUS_CODE.BASE_SUCCESS)
                        {
                            res.response = Marshal.PtrToStringAnsi(returnInfo2).Trim();
                            res.success = true;
                            res.msg = "获取成功";
                        }
                        else
                        {
                            res.msg = "读取失败";
                        }
                    }
                    catch (Exception e)
                    {
                        e.Log(Log.GetLog());
                    }
                }

                return res;
        }
        
        public MessageModel<string> SecretServiceCheck(string SecretStr)
        {
            var res = new MessageModel<string>();
            lock (lockobj)
            {
                try
                {
                    var returnInfo = Marshal.AllocHGlobal(1024);
                    var inbuf = Marshal.StringToHGlobalAnsi(SecretStr);
                    var ret = SodoWinSDKHandle.Sodo_SB_Psam_Process_GetAllInfo(ref returnInfo, ref inbuf, 3);
                    if (ret == (int)STATUS_CODE.BASE_SUCCESS)
                    {
                        res.response = Marshal.PtrToStringAnsi(returnInfo).Trim();
                        res.success = true;
                        res.msg = "获取成功";
                    }
                    else
                    {
                        res.msg = "密服验证失败";
                    }
                }
                catch (Exception e)
                {
                    e.Log(Log.GetLog());
                }


                return res;
            }
        }


        public void Close()
        {
            SodoWinSDKHandle.Sodo_Stop();
        }
    }
}