using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.Track
{
    public class Track : ITrack
    {
        //初始化 板卡
        FA_IO_CALLBACK2 fun;
        IntPtr handle = IntPtr.Zero;
        ushort com;
        public event Action<WebViewSendModel<TrackModel>> OnTrack;
        public MessageModel<bool> Connect(Int64 comPort)
        {
            var res = new MessageModel<bool>();
            com = (ushort)comPort;
            IOBERROR ero = FAIO.iob_board_init(com, 0);// 初始化板卡

            if (ero == IOBERROR.FAIO_ERROR_SUCCESS)
            {
                fun = new FA_IO_CALLBACK2(OnCALLBACK);
                FAIO.iob_board_setcallback2(com, fun, this.handle);// 设置回调函数

                res.msg = "连接成功";
                res.success = true;
                return res;
            }
            res.msg = $@"{ero}-{Enum.GetName(ero)}";
            return res;
        }

        public MessageModel<bool> Output(Int64 no, bool on = false)
        {
            var res = new MessageModel<bool>();
            if (com == 0)
            {
                res.msg = "未连接上设备，请先连接设备[Track]";
                return res;
            }
            IOBERROR ero = FAIO.iob_write_outbit(com, (byte)no, on);
            if (ero == IOBERROR.FAIO_ERROR_SUCCESS)
            {
                res.msg = "操作成功";
                res.success = true;
                return res;
            }
            res.msg = "操作失败";
            return res;
        }

        // 回调函数
        private void OnCALLBACK(IntPtr p, ushort comno, Byte inputportstatus, Byte outputportstatus, Byte inputportstatus_last, Byte outputportstatus_last, Byte framenum)
        {
            // 多卡的时候需要注意判断comno，以确定是否响应的是相应的卡
            //if( 测试卡端口号 != comno)return; // 如果不是测试卡(本例以界面输入的端口为测试卡端口)，则不执行测试卡的代码。


            if (inputportstatus != inputportstatus_last)// 输入端变化
            {
                // 输入端口1的沿变化（低电平触发卡）
                if ((inputportstatus_last & 0x01) == 0x01 &&
                    (inputportstatus & 0x01) == 0x00)// 输入端口1 从有信号（低电平)->无信号（高电平）
                {
                    OnTrack.Invoke(new WebViewSendModel<TrackModel>()
                    {
                        method = nameof(OnTrack),
                        response = new TrackModel()
                        {
                            InOutType = TrackInOutMenu.In,
                            TrackLevel = TrackLevel.High,
                            Relay = 1
                        },
                        success = true,
                        msg = "获取成功"
                    });
                }
                else if ((inputportstatus_last & 0x01) == 0x00 &&
                         (inputportstatus & 0x01) == 0x01)// 输入端口1 从无信号（高电平）->有信号（低电平)
                {
                    OnTrack.Invoke(new WebViewSendModel<TrackModel>()
                    {
                        method = nameof(OnTrack),
                        response = new TrackModel()
                        {
                            InOutType = TrackInOutMenu.In,
                            TrackLevel = TrackLevel.Low,
                            Relay = 1
                        },
                        success = true,
                        msg = "获取成功"
                    });
                }
                // 输入端口1的沿变化（低电平触发卡）
                else if ((inputportstatus_last & 0x02) == 0x02 &&
                    (inputportstatus & 0x02) == 0x00)// 输入端口1 从有信号（低电平)->无信号（高电平）
                {
                    OnTrack.Invoke(new WebViewSendModel<TrackModel>()
                    {
                        method = nameof(OnTrack),
                        response = new TrackModel()
                        {
                            InOutType = TrackInOutMenu.In,
                            TrackLevel = TrackLevel.High,
                            Relay = 2
                        },
                        success = true,
                        msg = "获取成功"
                    });
                }
                else if ((inputportstatus_last & 0x02) == 0x00 &&
                         (inputportstatus & 0x02) == 0x02)// 输入端口1 从无信号（高电平）->有信号（低电平)
                {
                    OnTrack.Invoke(new WebViewSendModel<TrackModel>()
                    {
                        method = nameof(OnTrack),
                        response = new TrackModel()
                        {
                            InOutType = TrackInOutMenu.In,
                            TrackLevel = TrackLevel.Low,
                            Relay = 2
                        },
                        success = true,
                        msg = "获取成功"
                    });
                }
            }

            //if (outputportstatus != outputportstatus_last) // 输出端变化
            //{

            //    // 输出端口1的沿变化（低电平触发卡）
            //    if ((outputportstatus_last & 0x01) == 0x01 &&
            //        (outputportstatus & 0x01) == 0x00)// 输出端口1 从有信号（低电平)->无信号（高电平）
            //    {

            //    }
            //    else if ((outputportstatus_last & 0x01) == 0x00 &&
            //            (outputportstatus & 0x01) == 0x01)// 输出端口1 从无信号（高电平）->有信号（低电平)
            //    {

            //    }
            //}
        }
    }

    public enum TrackInOutMenu
    {
        Out, In
    }
    public enum TrackLevel
    {
        Low, High
    }
    public class TrackModel
    {
        /// <summary>
        /// 输出端/输入端
        /// </summary>
        public TrackInOutMenu InOutType { get; set; }
        /// <summary>
        /// 低电平/高电平
        /// </summary>
        public TrackLevel TrackLevel { get; set; }
        /// <summary>
        /// 继电器号 目前只有1 2，对应光耦1 2
        /// </summary>
        public int Relay { get; set; }


    }
}
