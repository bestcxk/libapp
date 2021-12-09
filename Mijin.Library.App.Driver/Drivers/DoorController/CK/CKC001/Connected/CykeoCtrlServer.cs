using Newtonsoft.Json.Linq;
using PublicAPI.CKC001.MessageObj;
using PublicAPI.CKC001.MessageObj.MsgObj;
using PublicAPI.CKC001.MessageObj.Notify;
using PublicAPI.CKC001.Others;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PublicAPI.CKC001.Connected
{
    /// <summary>
    /// 
    /// </summary>
    public class CykeoCtrlServer
    {
        public event Action<Notify_Lock> dNotifyLock;
        public event Action<Notify_Light> dNotifyLight;
        public event Action<Notify_Finger> dNotifyFinger;
        public event Action<Notify_HFCard> dNotifyHF;
        public event Action<Notify_QRCode> dNotifyRQCode;
        public event Action<Notify_RFID> dNotifyRFID;

        public event Action<Notify_Error_Alarm_Abnormal> dNotifyAbnormal;
        public event Action<Notify_HeartBeat> dHeartBeat;
        public event Action<Notify_DevOnline> dNotifyDeviceOnline;
        public event Action<Notify_Humiture> dNotifyHumiture;
        public event Action<Notify_ContainerNum> dNotifyContainerNum;
        public event Action<Notify_Log> dNotifyLog;

        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        private SimpleTcp.SimpleTcpServer server;
        private Dictionary<string, ClientInfo> clients = new Dictionary<string, ClientInfo>();
        private uint _port = 5460;
        private CancellationTokenSource cPackage = new CancellationTokenSource();
        public CykeoCtrlServer(uint port = 5460)
        {
            _port = port;
            server = new SimpleTcp.SimpleTcpServer($"0.0.0.0:{_port}");
            server.Settings.OnCustSplitMessage = new Func<SimpleTcp.SimpleTcpRingBuffer, byte[]>(SplitPackages);
            server.Settings.IsSpilt = true;
            server.Settings.StreamBufferSize = 1024 * 1024 * 10;
            server.Events.ClientConnected += OnClientConnected;
            server.Events.ClientDisconnected += OnClientDisconnected;
            server.Events.DataReceived += OnDataReceived;
        }

        public bool OnStart()
        {
            try
            {
                server.Start();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void OnStop()
        {
            server.Stop();
            //Logs.WorkLog($"服务停止");
        }

        /// <summary>
        /// 发送消息至指定客户端
        /// </summary>
        /// <param name="ipPort"></param>
        /// <param name="msg"></param>
        public void SendAsync(string ipPort, MsgObjBase msg)
        {
            if (clients.ContainsKey(ipPort))
            {
                var tmp = clients[ipPort].SerialNum;
                msg.SerialNum = tmp == null ? new byte[4] : tmp;
                msg.AddressNum = clients[ipPort].AddressNum;

            }
            msg.SendPacked();
            //发送消息
            server.SendAsync(ipPort, msg.CmdToBytes());

            //异步推送日志信息
            dNotifyLog?.Invoke(new Notify_Log(msg, ipPort, false));

        }

        /// <summary>
        /// 发送消息至所有客户端
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(MsgObjBase msg)
        {
            //var clientList = server.GetClients().ToList();
            foreach (var item in clients)
            {
                var tmp = item.Value.SerialNum;
                msg.SerialNum = tmp == null ? new byte[4] : tmp;
                msg.AddressNum = item.Value.AddressNum;
                msg.SendPacked();
                //异步推送日志信息
                dNotifyLog?.Invoke(new Notify_Log(msg, item.Key, false));
                //发送消息
                server.SendAsync(item.Key, msg.CmdToBytes());
            }



        }

        private void OnDataReceived(object sender, SimpleTcp.DataReceivedEventArgs e)
        {
            //CRC校验
            var client = e.IpPort;
            var msg = new MsgObjBase(e.Data);
            if (msg.CheckCRC())
            {
                if (msg.CmdType == eCmdType.HartBeat)
                {
                    SendAsync(client, new MsgObj_HeartBeat());
                }

                switch (msg.CmdType)
                {
                    case eCmdType.Lock:
                        dNotifyLock?.Invoke(new MessageObj.Notify.Notify_Lock(msg, client)
                        {
                            getLockStatus = msg.CmdData,
                        });
                        break;
                    case eCmdType.LED:
                        dNotifyLight?.Invoke(new MessageObj.Notify.Notify_Light(msg, client)
                        {

                        });
                        break;
                    case eCmdType.Finger:
                        MessageObj.Notify.Notify_Finger notify_Finger = new Notify_Finger(msg, client);
                        switch (msg.CmdTag)
                        {
                            case 0x02:
                                notify_Finger.getNotifyVerifyResult = new FingerChildrenVerify_1_N()
                                {
                                    setUserID = DataConverts.ReadBytes(msg.CmdData, 0, 6),
                                    setFingerID = msg.CmdData[6],
                                    setVerifyResult = msg.CmdData[7],
                                };
                                break;
                            case 0x04:
                                FingerChildrenAckGatherResult tempFingerGather = new FingerChildrenAckGatherResult();
                                tempFingerGather.setGatherCount = msg.CmdData[0];
                                tempFingerGather.setResult = msg.CmdData[1];
                                if (msg.CmdData[0] == 0x03 && msg.CmdData[1] == 0x00)
                                {
                                    tempFingerGather.setFingerTemplatesByte = DataConverts.ReadBytes(msg.CmdData, 2, msg.CmdData.Length - 2);
                                }
                                notify_Finger.getNotifyGatherResult = tempFingerGather;

                                this.SendAsync(client, new MsgObj_Finger_GatherTemplatesACK()
                                {
                                    setAckCollectCount = msg.CmdData[0],
                                });
                                break;
                        }
                        dNotifyFinger?.Invoke(notify_Finger); ;
                        break;
                    case eCmdType.HFCard:
                        dNotifyHF?.Invoke(new Notify_HFCard(msg, client)
                        {
                            getIDNumByte = (msg.CmdTag == 0x01) ? msg.CmdData : null,
                            getUserDataByte = (msg.CmdTag == 0x02) ? msg.CmdData : null,
                        });
                        break;
                    case eCmdType.QRCode:
                        dNotifyRQCode?.Invoke(new Notify_QRCode(msg, client)
                        {
                            setRQCodeByte = msg.CmdData,
                        }); ;
                        break;
                    case eCmdType.RFID:
                        Notify_RFID notify_RFID = new Notify_RFID(msg, client) { };
                        if (msg.CmdData != null)
                        {
                            //RfidReport tags = null;
                            switch ((eRFID)msg.CmdTag)
                            {
                                case eRFID.NotifyReadData:
                                    string jsonStr = DataConverts.Bytes_To_ASCII(msg.CmdData);
                                    if (!string.IsNullOrEmpty(jsonStr))
                                    {
                                        notify_RFID.AllTags = Newtonsoft.Json.JsonConvert.DeserializeObject<RfidReport>(jsonStr);
                                    }
                                    else
                                    {
                                        notify_RFID.AllTags = new RfidReport();
                                    }

                                    break;
                                case eRFID.GetAllTags:
                                    string allStr = DataConverts.Bytes_To_ASCII(msg.CmdData);
                                    if (!string.IsNullOrEmpty(allStr))
                                    {
                                        notify_RFID.AllTags = Newtonsoft.Json.JsonConvert.DeserializeObject<RfidReport>(allStr);
                                    }
                                    else
                                    {
                                        notify_RFID.AllTags = new RfidReport();
                                    }
                                    break;
                                default:
                                    break;
                            }


                            dNotifyLog?.Invoke(new Notify_Log(msg, client));
                        }
                        dNotifyRFID?.Invoke(notify_RFID);
                        break;
                    case eCmdType.Warnning:
                        dNotifyAbnormal?.Invoke(new Notify_Error_Alarm_Abnormal(msg, client)
                        {

                        });
                        //异步推送日志信息
                        dNotifyLog?.Invoke(new Notify_Log(msg, client));
                        break;
                    case eCmdType.HartBeat:
                        //this.SendAsync(new MsgObj_HeartBeat());
                        dHeartBeat?.Invoke(new Notify_HeartBeat(msg, client)
                        {
                        });
                        break;
                    case eCmdType.Online:
                        {
                            if (clients.ContainsKey(client))
                            {
                                clients[client].SerialNum = msg.SerialNum;
                                clients[client].AddressNum = msg.AddressNum;
                            }
                            dNotifyDeviceOnline?.Invoke(new Notify_DevOnline(msg, client)
                            {
                                setDevAddress = msg.AddressNum,
                                setDevIPByte = DataConverts.ReadBytes(msg.CmdData, 0, 4),
                                setMacByte = DataConverts.ReadBytes(msg.CmdData, 4, 6),
                                setDevType = DataConverts.ReadBytes(msg.CmdData, 10, msg.CmdData.Length - 10),
                            });
                            break;
                        }
                    case eCmdType.Humiture:
                        Notify_Humiture notify_Humiture = new Notify_Humiture(msg, client) { };
                        switch ((eHumiture)msg.CmdTag)
                        {
                            case eHumiture.NotifyHumiture:
                                ushort tempUshort1 = DataConverts.Bytes_To_Ushort(new byte[2] { msg.CmdData[0], msg.CmdData[1] });
                                notify_Humiture.setTemperature = Math.Round(((double)tempUshort1 / 10), 1);
                                tempUshort1 = DataConverts.Bytes_To_Ushort(new byte[2] { msg.CmdData[2], msg.CmdData[3] });
                                notify_Humiture.setHumidity = Math.Round(((double)tempUshort1 / 10), 1);
                                break;
                        }
                        dNotifyHumiture?.Invoke(notify_Humiture); ;
                        break;
                    case eCmdType.Address:
                        if (clients.ContainsKey(client))
                        {
                            clients[client].AddressNum = msg.AddressNum;
                        }
                        dNotifyContainerNum?.Invoke(new Notify_ContainerNum(msg, client)
                        {
                            getNewAddress = msg.AddressNum,
                        });
                        break;
                }

            }
            else
            {
                Trace.WriteLine($"CRC校验失败,{DataConverts.Bytes_To_HexStr(msg.FrameData)}");
            }

        }

        private void OnClientDisconnected(object sender, SimpleTcp.ClientDisconnectedEventArgs e)
        {
            if (clients.ContainsKey(e.IpPort))
            {
                clients.Remove(e.IpPort);
            }
            ClientDisconnected?.Invoke(e.IpPort);
            //Logs.WorkLog($"客户端断开，{e.IpPort}");

        }

        private void OnClientConnected(object sender, SimpleTcp.ClientConnectedEventArgs e)
        {
            if (!clients.ContainsKey(e.IpPort))
            {
                clients.Add(e.IpPort, new ClientInfo());
            }
            ClientConnected?.Invoke(e.IpPort);
        }

        private byte[] SplitPackages(SimpleTcp.SimpleTcpRingBuffer ring)
        {
            if (ring.DataCount < 14)
            {
                Trace.WriteLine("无数据，不做处理");
                return new byte[] { };
            }
            else
            {
                while (ring[0] != 0x16 && ring[1] != 0x98)
                {
                    ring.Clear(1);
                }

                ushort frameLen = DataConverts.Bytes_To_Ushort(new byte[2] { ring[10], ring[11] });
                int allLen = 14 + frameLen;
                if (allLen < 605535)
                {
                    if (ring.DataCount < allLen)
                    {
                        Trace.WriteLine("半帧数据，等待接收完整");
                        return new byte[] { };
                    }
                    else
                    {
                        byte[] temp = new byte[allLen];
                        ring.ReadFromRingBuffer(temp, 0, allLen);
                        ring.Clear(allLen);
                        return temp;
                    }
                }
                else
                {
                    Trace.WriteLine($"帧长度异常 {allLen}，抛弃数据");
                    return new byte[] { };
                }
            }
        }
    }

}

public class ClientInfo
{
    private ReaderWriterLockSlim rLock = new ReaderWriterLockSlim();
    public byte AddressNum { get; set; }
    public byte[] SerialNum { get; set; }
    public List<byte> Buffer { get; private set; } = new List<byte>();

    public byte[] GetBuffer(byte[] buffer)
    {
        lock (Buffer)
        {
            Buffer.AddRange(buffer);

            if (Buffer.Count < 14)
            {
                Trace.WriteLine("无数据，不做处理");
                return new byte[] { };
            }
            else
            {
                while (Buffer[0] != 0x16 && Buffer[1] != 0x98)
                {
                    Buffer.RemoveAt(0);
                }

                ushort frameLen = DataConverts.Bytes_To_Ushort(new byte[2] { Buffer[10], Buffer[11] });
                int allLen = 14 + frameLen;
                if (allLen < 605535)
                {
                    if (Buffer.Count < allLen)
                    {
                        //Trace.WriteLine("半帧数据，等待接收完整");
                        return new byte[] { };
                    }
                    else
                    {

                        var temp = Buffer.Take(allLen).ToArray();
                        Buffer.RemoveRange(0, allLen);
                        return temp;
                    }
                }
                else
                {
                    //Trace.WriteLine($"帧长度异常 {allLen}，抛弃数据");
                    return new byte[] { };
                }
            }
        }
    }
}



