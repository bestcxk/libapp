using autoreplyprint.cs;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Maps;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 茗又小票打印机
    /// </summary>
    public class MyPosPrint : IPosPrint
    {
        // 小票打印机句柄
        private static UIntPtr _handle = UIntPtr.Zero;

        /// <summary>
        /// 自动检测com口,，并初始化
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Init()
        {
            var data = new MessageModel<bool>();
            var coms = SerialPort.GetPortNames();

            if (coms.Length <= 0)
            {
                data.msg = "com口数量为0，无法初始化";
                return data;
            }

            for (int i = 0; i < coms.Length; i++)
            {
                _handle = AutoReplyPrint.CP_Port_OpenCom(
                        coms[i], 115200,
                        AutoReplyPrint.CP_ComDataBits_8,
                        AutoReplyPrint.CP_ComParity_NoParity,
                        AutoReplyPrint.CP_ComStopBits_One,
                        AutoReplyPrint.CP_ComFlowControl_XonXoff,
                        0);

                if (AutoReplyPrint.CP_BlackMark_FullCutBlackMarkPaper(_handle) != 0)
                {
                    data.success = true;
                    return data;
                }
            }
            _handle = UIntPtr.Zero;
            data.msg = "未找到打票机com口，无法初始化";
            return data;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        public MessageModel<bool> Print(PrintInfo print)
        {
            var data = new MessageModel<bool>();

            if (print == null)
            {
                data.msg = "参数类型不可为空";
                return data;
            }

            if (_handle == UIntPtr.Zero) // 未创建句柄，则进行初始化
            {
                var res = Init();
                if (res.success == false)
                {
                    data.msg = res.msg;
                    return data;
                }
            }

            print.CreateTime = DateTime.Now; // 小票创建时间

            // 身份证信息处理
            if (!string.IsNullOrWhiteSpace(print.UserIdentity) && print.UserIdentity.Length == 18)
            {
                string temp = print.UserIdentity.Substring(print.UserIdentity.Length / 2, 4);
                print.UserIdentity = print.UserIdentity.Replace(temp, "****");
            }

            // 卡号信息处理
            if (!string.IsNullOrWhiteSpace(print.UserCardNo) && print.UserCardNo.Length == 18)
            {
                string temp = print.UserCardNo.Substring(print.UserCardNo.Length / 2, 4);
                print.UserCardNo = print.UserCardNo.Replace(temp, "****");
            }

            // 借阅信息
            AutoReplyPrint.CP_Pos_PrintText(_handle, "-----------借阅信息-----------" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"操作类型：{print.Action.ToString()}" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"操作时间：{print.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}" + "\r\n");

            AutoReplyPrint.CP_Pos_PrintText(_handle, "-----------用户信息-----------" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"用户名  ：{print.UserName ?? ""}" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"卡号    ：{print.UserCardNo ?? ""}" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"身份证  ：{print.UserIdentity ?? ""}" + "\r\n");

            AutoReplyPrint.CP_Pos_PrintText(_handle, "-----------书籍信息-----------" + "\r\n");
            AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍数量：{print.BookInfos?.Count}" + "\r\n" + (print.BookInfos.Count > 1 ? "\r\n" : ""));
            print.BookInfos?.ForEach(book =>
            {
                AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍名称：{book.Title ?? ""}" + "\r\n");

                // 以前使用的是UHF，现在改成条形码
                //if (!string.IsNullOrWhiteSpace(book.UHF))
                //    AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍UHF:{book.UHF}" + "\r\n");

                if (!string.IsNullOrWhiteSpace(book.Serial))
                    AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍条码:{book.Serial}" + "\r\n");

                //if(!string.IsNullOrWhiteSpace(book.Serial))
                //AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍编号：{book.Serial}" + "\r\n");

                if (print.Action != ActionType.归还)
                    AutoReplyPrint.CP_Pos_PrintText(_handle, $@"应还时间：{book.ShouldBackTime.ToString("yyyy-MM-dd HH:mm:ss")}" + "\r\n");

                // 一维码
                AutoReplyPrint.CP_Pos_PrintText(_handle, $@"书籍ISBN：" + "\r\n");
                if (book.Isbn != null)
                {
                    AutoReplyPrint.CP_Pos_SetBarcodeUnitWidth(_handle, 3);
                    AutoReplyPrint.CP_Pos_SetBarcodeHeight(_handle, 60);
                    AutoReplyPrint.CP_Pos_SetBarcodeReadableTextFontType(_handle, 0);
                    AutoReplyPrint.CP_Pos_PrintBarcode(_handle, AutoReplyPrint.CP_Pos_BarcodeType_EAN13, book.Isbn);
                }
                if (print.BookInfos?.Count > 1)
                    AutoReplyPrint.CP_Pos_PrintText(_handle, "\r\n");
            });
            //AutoReplyPrint.CP_Pos_FeedLine(_handle, 4);
            int result = AutoReplyPrint.CP_Pos_FeedLine(_handle, 4);
            AutoReplyPrint.CP_BlackMark_FullCutBlackMarkPaper(_handle);
            if (result == 0)
            {
                data.success = false;
                data.msg = "打印失败，未知原因，请联系管理员";
                return data;
            }
            data.success = true;
            data.msg = "打印成功";
            return data;
        }

        /// <summary>
        /// 打印(用于web进行反射调用)
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        public MessageModel<bool> Print(object print)
        {
            return Print(print.JsonMapTo<PrintInfo>());
        }
    }
}
