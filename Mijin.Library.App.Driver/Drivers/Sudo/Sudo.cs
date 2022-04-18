using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using Mijin.Library.App.Driver.Drivers.Sudo;
using Mijin.Library.App.Model;
using Util.Logs;
using Util.Logs.Extensions;

namespace Mijin.Library.App.Driver
{
    public class Sudo : ISudo
    {
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

            return res;
        }
        public MessageModel<string> Read_SSC()
        {
            var res = new MessageModel<string>();
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
                    var data = Encoding.Default.GetString(bytes)?.Split("|")[1];
                    res.response = data;
                    res.success = true;
                    res.msg = "获取成功";
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

            return res;
        }
    }
}
