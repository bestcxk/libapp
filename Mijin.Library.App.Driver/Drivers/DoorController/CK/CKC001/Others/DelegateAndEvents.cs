using PublicAPI.CKC001.Connected;
using PublicAPI.CKC001.MessageObj;
using PublicAPI.CKC001.MessageObj.MsgObj;
using PublicAPI.CKC001.MessageObj.Notify;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PublicAPI.CKC001.Others
{
    public delegate void delegateMessageReceived(MsgObjBase msg); //接收消息
    public delegate void delegateDisconnected(string msg);//断开
    public delegate void delegateConnected();//连接

    public delegate void delegateTcpDisconnected(string str);//断开连接
    //public delegate void delegateTcpClientConnected(Others.SocketObject client);
    public delegate void delegateTcpClientConnected(ClientObject client);
    internal delegate void delegateInternalTcpClientConnected(PublicAPI.CKC001.Connected.communication.TypeTcpClient client);//连接

    //上报
    public delegate void delegateNotifyDeviceOnline(Notify_DevOnline msg);  //委托  设备上线
    public delegate void delegateNotifyLock(Notify_Lock msg); //锁反馈
    public delegate void delegateNotifyHF(Notify_HFCard msg); //高频卡
    public delegate void deleageNotifyFinger(Notify_Finger msg);//指静脉
    public delegate void delegateNotifyRQCode(Notify_QRCode msg);//二维码
    public delegate void delegateNotifyHumiture(Notify_Humiture msg);//温湿度
    public delegate void delegateNotifyRFID(Notify_RFID msg); //rfid
    public delegate void delegateNotifyHeartBeat(Notify_HeartBeat msg);//心跳
    public delegate void delegateNotifyLight(Notify_Light msg);//灯控
    public delegate void delegateNotifyAbnormal(Notify_Error_Alarm_Abnormal msg);//故障
    public delegate void delegateNotifyContainerNum(Notify_ContainerNum msg);//
    public delegate void delegateNotifyLog(Notify_Log msg);//日志

    public delegate void delegateUDPMulticast(PublicAPI.CKC001.MessageObj.UdpObj.UdpObjBase udp,IPAddress address);//udp广播

}
