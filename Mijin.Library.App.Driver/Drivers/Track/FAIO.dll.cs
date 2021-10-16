/*//////////////////////////////////////////////////////////////////////////
*功能：FAIO板卡API函数
*作者：gcnhiexn
*时间：2015-2-22 20:38:25
*版本：3.17.1122.1450
*适用环境：支持Unicode;支持多线程操作;支持win x64和x86操作系统;支持VB,VB.net,VC,C#的调用(详见例程);

*时间：2017-3-3 14:37:11
*版本：5.17.3.314
*更新： 1，优化处理速度
		2，增加脉冲发生器的4个函数iob_board_pulser_xxxxx

*时间：2017-8-27 14:10:28
*版本：5.17.8.2714
*更新： 1，修复bitno越界问题。
		2，增加iob_board_waitportevent函数，用于支持不方便创建事件的编程语言(如:Labview)的调用.
		
*时间：2017-11-29 15:50:29
*版本：6.17.11.2915
*更新： 1，提升处理速度.
		2，修复输入端的电平错误(输入端有信号，软件得到的输入状态是0).
		3，增加输入输出端上次状态值，更加方便得到FAIO端口的变化.
//////////////////////////////////////////////////////////////////////////*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;//使用线程
using Microsoft.Win32.SafeHandles;

namespace Mijin.Library.App.Driver
{
    public enum FAIO_DELAYOFF
    {
        FAIO_delayOff_0_1 = 0x12,   //0.1s  //iob_set_delayOff  secondevery
        FAIO_delayOff_0_5 = 0x13,   //0.5s  //iob_set_delayOff  secondevery
        FAIO_delayOff_1_0 = 0x14,   //1.0s  //iob_set_delayOff  secondevery
    }


    //定义回调函数  委托
    public delegate void FA_IO_CALLBACK(IntPtr p, ushort comno, Byte inportstatus, Byte outportstatus, Byte framenum);
    public delegate void FA_IO_CALLBACK2(IntPtr p, ushort comno, Byte inportstatus, Byte outportstatus, Byte inportstatus_last, Byte outportstatus_last, Byte framenum);

    public enum IOBERROR
    {
        FAIO_ERROR_SUCCESS = 0,  // 成功
        FAIO_ERROR_OVERMAXBORADNUM = -1, // comno 的值超出最大board数 0≤comno≤15
        FAIO_ERROR_INITFAILED = -2, // board打开失败 //串口不存在
        FAIO_ERROR_WRONGBOARD = -3, // board打开失败 //板卡异常 //次品卡或者非该软件专用卡
        FAIO_ERROR_BOARDNOTINIT = -4, // board没有初始化,即没有调用iob_board_init函数
        FAIO_ERROR_BOARDDISCONNECT = -5, // board没有连接或者已经断开连接
        FAIO_ERROR_WRONGPARAM_delayOff = -6, // iob_set_delayOff() 中的secondevery 参数不正确
        FAIO_ERROR_READTIMEOUT = -7, // iob_read_ 读操作超时
        FAIO_ERROR_INVALIDHANDLE = -8, // iob_board_setportevent中的hPortEvent 为无效句柄

        FAIO_ERROR_WAITCOMMEVENT = -9,  // COM端的WaitCommEvent 错误
        FAIO_ERROR_RECEIVE = -10, // COM端的Receive 错误
        FAIO_ERROR_WRONGRECVBYTESIZE = -11, // COM端的WrongRecvByteSize 错误
        FAIO_ERROR_WRITE = -12, // COM端的WriteERROR 错误

        FAIO_ERROR_PULSERCREATEFAIL = -13, // 脉冲发生器创建失败
        FAIO_ERROR_PULSERINVALID = -14, // 脉冲发生器未创建

        FAIO_ERROR_WAITTIMEOUT = -15, // iob_board_waitportevent 操作超时
        FAIO_ERROR_OVERMAXBITNUM = -16, // bitno 的值超出最大bitno数 0≤bitno≤8
        FAIO_ERROR_BITNUMVALUEWRONG = -17, // bitno 的值不正确，即iob_read_inbit，iob_read_outbit，iob_write_outbit中的bitno 不能等于0
    };//板卡的错误类型



    public class FAIO
    {
        //////////////////////////////////////////////////////////////////////////
        //输出端的1代表ON(闭合),输入端的1代表有信号								//
        //COM口编号数最大为16 即0≤comno≤15									    //
        //////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////
        ///////////////////////////初始化函数/////////////////////////////////////
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_read_getlasterror(ushort comno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_init(ushort comno, Byte outputstatus);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_close(ushort comno, bool bOutputKeep);

        ///////////////////////////操作函数///////////////////////////////////////
        [DllImport("FAIO.dll")]
        public static extern int iob_read_inport(ushort comno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_outport(ushort comno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_inport_last(ushort comno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_outport_last(ushort comno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_write_outport(ushort comno, Byte outputstatus);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_inbit(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_outbit(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_inbit_last(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern int iob_read_outbit_last(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_write_outbit(ushort comno, Byte bitno, bool bOn);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_write_delayOff(ushort comno, FAIO_DELAYOFF secondevery, Byte secondmulty);

        ///////////////////////////设置函数///////////////////////////////////////
        // 操作函数中的bitno编号数最大为8 即0＜bitno≤8	 （bitno = 0时会返回错误代码FAIO_ERROR_BITNUMVALUEWRONG）
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_setcallback(IntPtr p, ushort comno, FA_IO_CALLBACK callbackfunstatus);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_setcallback2(ushort comno, FA_IO_CALLBACK2 callbackfun2status, IntPtr p);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_setoutport(ushort comno, bool bAllOn);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_setportevent(ushort comno, SafeWaitHandle hPortEvent);

        ///////////////////////////等待函数///////////////////////////////////////
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_waitportevent(ushort comno, UInt32 dwWaitTime);

        ////////////////////////////获取卡信息////////////////////////////////////
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_getboardinfo(ushort comno, String version, String cardtype);

        ////////////////////////////脉冲发生器//////////////////////////////////////////////
        // 脉冲发生器中bitno编号数最大为8 即0≤bitno≤8	 （bitno = 0 所有bit）
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_pulser_create(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_pulser_destory(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_pulser_terminate(ushort comno, Byte bitno);
        [DllImport("FAIO.dll")]
        public static extern IOBERROR iob_board_pulser_pulse(ushort comno,
                                                            Byte bitno,
                                                            bool bPositive,                 //true:正向脉冲。 false:负向脉冲
                                                            UInt32 dwCount,         //脉冲个数
                                                            UInt32 dwTimePositive,  //正向脉冲的持续时间(详见文档)
                                                            UInt32 dwTimeNegative,  //负向脉冲的持续时间(详见文档)
                                                            UInt32 dwTimeFirst);//第一次脉冲的前奏时间(详见文档)
    }
}