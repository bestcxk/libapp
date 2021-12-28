using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CH340;
using IsUtil;
using Extensions = Util.Extensions;
using Util.Logs.Extensions;
using Util.Logs;

namespace Mijin.Library.App.Driver
{
    public class M513Reader : IdentityReader
    {
        IdCard ic_Card = new IdCard();
        private string com = null;

        #region Dll

        #region VID和PID连接com口

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct HANDLE
        {
            public int unused;
        }

        #endregion

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern void CH9326CloseDevice(HANDLE hDeviceHandle); //关闭那啥

        //[DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        //static extern HANDLE CH9326OpenDevice(ushort USB_VID, ushort USB_PID);//通过VID和PID打开设备
        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326SetRate(HANDLE hDeviceHandle, byte ucRate, byte ucCheck, byte ucStop, byte ucData, byte ucInterval); //设置波特率

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326InitThreadData(HANDLE hDeviceHandle); //初始化线程

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern ulong CH9326GetBufferLen(HANDLE hDeviceHandle, ref ushort InputReportLen, ref ushort OutputReportLen); //返还硬件能够接受的长度


        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326ReadData(HANDLE hDeviceHandle, ref byte[] buff, ref int ReadLen, HANDLE hd); //读数据

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern HANDLE CH9326OpenDevice(ushort USB_VID, ushort USB_PID);


        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326ReadThreadData(HANDLE hDeviceHandle, ref byte[] buff, ref int ReadLen); //读数据


        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326WriteData(HANDLE hDeviceHandle, byte[] buff, int ReadLen, HANDLE hd); //写数据

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern void CH9326ClearThreadData(HANDLE hDeviceHandle); //设备句柄

        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326ReadData(HANDLE hDeviceHandle, ref char[] ReadBuffer, ref int pReadLen, HANDLE hEventObject);


        [DllImport("CH9326DLL.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern bool CH9326GetAttributes(HANDLE hDeviceHandle, ref ushort pVID, ref ushort pPID, ref ushort pVer); //返还读卡器VID、PID


        [DllImport("SuperReader.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern HANDLE CH9326OpenDevices(ushort USB_VID, ushort USB_PID); //返还读卡器VID、PID

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int FindCard(); //寻卡

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SelectCard(); //选卡

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int ReadBaseMsg(byte[] pucManaInfo, byte[] phone); //返还读卡器VID、PID

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int ReadBaseMsg2(byte[] pucManaInfo); //返还读卡器VID、PID


        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern byte[] mCharToBcd(byte[] mychar); //返还读卡器VID、PID

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int Come(); //返还读卡器VID、PID


        [DllImport("WltRS.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int GetBmp(byte[] Wlt_File, int intf); //读取追加地址信息0x90成功 0x91无数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int PasswordVerification(int ReadLen, byte[] buff); //读数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int ReadICSan(int ReadLen, byte[] buff); //读数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int WriteSector(int ReadLen, byte[] buff); //写数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SetUpAntenna(); //读取追加地址信息0x90成功 0x91无数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int OPenAntenna(); //读取追加地址信息0x90成功 0x91无数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SetUpAgreement(); //读取追加地址信息0x90成功 0x91无数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SetUID(byte[] pucManaInfo); //读取追加地址信息0x90成功 0x91无数据

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SetUpICAgreement(); //设置IC卡协议


        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int SetICUID(byte[] pucManaInfo); //设置IC卡协议


        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int Fingerprint(byte[] pucManaInfo, byte[] pucManaInfoa, byte[] pucManaInfoaa); //返还读卡器VID、PID

        [DllImport("SuperReader.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        static extern int ReadSAM_ID(byte[] pucManaInfo); //SAM号

        #endregion

        private bool BeforeCheck()
        {
            if (!Extensions.IsEmpty(com)) return true;
            string[] a = ic_Card.RetunCom();
            foreach (var item in a)
            {
                ic_Card.OpenCom(item);
                string C = ic_Card.OFFAntenna();
                if (C == "0")
                {
                    com = item;
                    break;
                }
                else
                {
                    ic_Card.ClosCom();
                }
            }

            return !Extensions.IsEmpty(com);
        }

        public MessageModel<IdentityInfo> ReadIdentity()
        {
            var res = new MessageModel<IdentityInfo>();
            IdentityInfo user = new IdentityInfo();

            if (!BeforeCheck())
            {
                res.msg = "连接设备失败";
                return res;
            }

            byte[] uid = ic_Card.ReadUserID();

            if (uid.Length == 1 || uid.Length == 3)
            {
                res.msg = "未连接设备";
                return res;
            }
            else if (uid.Length == 2)
            {
                res.msg = "请放置身份证卡";
                return res;
            }
            else if (uid.Length == 8)
            {
                string status = ic_Card.ReadUserOnekey(); //设置身份证模式
                Console.WriteLine("设置模式状态" + status);
                if (status == "0")
                {
                    byte[] by = ic_Card.DuKa();
                    Console.WriteLine("身份证数据长度" + by.Length);
                    if (by.Length < 1295)
                    {
                        return res;
                    }

                    byte[] type = new byte[2];
                    byte[] image = new byte[1024];
                    byte[] userby = new byte[256];
                    Array.Copy(by, 14, userby, 0, 256); //获取文字信息字节
                    Array.Copy(by, 270, image, 0, 1024); //获取图片字节
                    Array.Copy(by, 262, type, 0, 2);
                    string asa = Encoding.Unicode.GetString(type);
                    if (asa == "J")
                    {
                        user = RetunGATUser(userby);
                    }
                    else if (asa == "I")
                    {
                        user = RetunForeignUser(userby);
                    }
                    else
                    {
                        user = RetunUser(userby);
                    }


                    string FileLoad = "zp.wlt"; //.jpg-.doc   -.xls
                    FileStream fs = new FileStream(FileLoad, FileMode.Create, FileAccess.Write); //创建图片存放路径
                    BinaryWriter bw = new BinaryWriter(fs);
                    int i = 1;
                    while (i < image.Length)
                    {
                        bw.Write(image);
                        i++;
                    }

                    bw.Close();

                    try
                    {
                        if (File.Exists("zp.wlt"))
                        {
                            string path; int b = 1;
                            path = ("zp.wlt");
                            byte[] byteArray = Encoding.Default.GetBytes(path);
                            int aa = GetBmp(byteArray, b);

                            if (File.Exists("zp.bmp"))
                            {
                                System.Drawing.Image img = System.Drawing.Image.FromFile("zp.bmp");
                                user.FacePicBase64 = ImageHelper.ToBase64Str(new System.Drawing.Bitmap(img));
                                img.Dispose();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        e.Log(Log.GetLog());
                    }

                }
                else
                {
                    res.msg = "身份证模式设置失败";
                    return res;
                }
            }


            res.response = user;
            res.msg = "获取成功";
            return res;
        }

        public MessageModel<string> ReadHFCardNo(long? com = null)
        {
            return null;
        }

        public IdentityInfo RetunGATUser(byte[] pucCHMsg)
        {
            IdentityInfo user = new IdentityInfo();
            byte[] name = new byte[30];
            byte[] sex = new byte[2];
            byte[] brath = new byte[16];
            byte[] address = new byte[70];
            byte[] num = new byte[36];
            byte[] security = new byte[30];
            byte[] startdate = new byte[16];
            byte[] enddate = new byte[16];
            byte[] txnum = new byte[18];
            byte[] QFnum = new byte[4];
            Array.Copy(pucCHMsg, 0, name, 0, 30);
            Array.Copy(pucCHMsg, 30, sex, 0, 2);
            Array.Copy(pucCHMsg, 36, brath, 0, 16);
            Array.Copy(pucCHMsg, 52, address, 0, 70);
            Array.Copy(pucCHMsg, 122, num, 0, 36);
            Array.Copy(pucCHMsg, 158, security, 0, 30);
            Array.Copy(pucCHMsg, 188, startdate, 0, 16);
            Array.Copy(pucCHMsg, 204, enddate, 0, 16);
            Array.Copy(pucCHMsg, 220, txnum, 0, 18);
            Array.Copy(pucCHMsg, 238, QFnum, 0, 4);
            string str = "";
            user.Name = Encoding.Unicode.GetString(name);
            str = Encoding.Unicode.GetString(sex);

            #region 性别

            if (str == "1")
            {
                user.Sex = "男";
            }
            else if (str == "0")
            {
                user.Sex = "未知";
            }
            else if (str == "2")
            {
                user.Sex = "女";
            }
            else
            {
                user.Sex = "未说明";
            }

            #endregion

            user.Birth = Encoding.Unicode.GetString(brath);
            user.Addr = Encoding.Unicode.GetString(address);
            user.Identity = Encoding.Unicode.GetString(num);
            // user.Country = Encoding.Unicode.GetString(startdate);
            //user.Brath = Encoding.Unicode.GetString(brath);
            //user.Address = Encoding.Unicode.GetString(address);
            //user.Num = Encoding.Unicode.GetString(num);
            //user.Security = Encoding.Unicode.GetString(security);
            //user.Startdate = Encoding.Unicode.GetString(startdate);
            //user.Enddate = Encoding.Unicode.GetString(enddate);
            //user.TXNum = Encoding.Unicode.GetString(txnum);
            //user.QFNum = Encoding.Unicode.GetString(QFnum);
            return user;
        }

        private IdentityInfo RetunForeignUser(byte[] pucCHMsg)
        {
            IdentityInfo user = new IdentityInfo();
            byte[] name = new byte[120];
            byte[] sex = new byte[2];
            byte[] num = new byte[30];
            byte[] Nationality = new byte[6];
            byte[] CName = new byte[30];
            byte[] startdate = new byte[16];
            byte[] enddate = new byte[16];
            byte[] brath = new byte[16];
            byte[] VersionNum = new byte[4];
            byte[] security = new byte[8];

            Array.Copy(pucCHMsg, 0, name, 0, 120);
            Array.Copy(pucCHMsg, 120, sex, 0, 2);
            Array.Copy(pucCHMsg, 122, num, 0, 30);
            Array.Copy(pucCHMsg, 152, Nationality, 0, 6);
            Array.Copy(pucCHMsg, 158, CName, 0, 30);
            Array.Copy(pucCHMsg, 188, startdate, 0, 16);
            Array.Copy(pucCHMsg, 204, enddate, 0, 16);
            Array.Copy(pucCHMsg, 220, brath, 0, 16);
            Array.Copy(pucCHMsg, 136, VersionNum, 0, 4);
            Array.Copy(pucCHMsg, 240, security, 0, 8);
            string str = "";
            user.Name = Encoding.Unicode.GetString(name);
            str = Encoding.Unicode.GetString(sex);

            #region 性别

            if (str == "1")
            {
                user.Sex = "男";
            }
            else if (str == "0")
            {
                user.Sex = "未知";
            }
            else if (str == "2")
            {
                user.Sex = "女";
            }
            else
            {
                user.Sex = "未说明";
            }

            #endregion

            user.Birth = Encoding.Unicode.GetString(brath);
            // user.Addr = Encoding.Unicode.GetString(address);
            user.Identity = Encoding.Unicode.GetString(num);
            user.Country = Encoding.Unicode.GetString(Nationality);
            return user;
        }

        public IdentityInfo RetunUser(byte[] pucCHMsg)
        {
            IdentityInfo user = new IdentityInfo();
            byte[] name = new byte[30];
            //MessageBox.Show(BitConverter.ToString(name));打印16进制出来
            byte[] sex = new byte[2];
            byte[] minzu = new byte[4];
            byte[] brath = new byte[16];
            byte[] address = new byte[70];
            byte[] num = new byte[36];
            byte[] security = new byte[30];
            byte[] startdate = new byte[16];
            byte[] enddate = new byte[16];
            Array.Copy(pucCHMsg, 0, name, 0, 30);
            Array.Copy(pucCHMsg, 30, sex, 0, 2);
            Array.Copy(pucCHMsg, 32, minzu, 0, 4);
            Array.Copy(pucCHMsg, 36, brath, 0, 16);
            Array.Copy(pucCHMsg, 52, address, 0, 70);
            Array.Copy(pucCHMsg, 122, num, 0, 36);
            Array.Copy(pucCHMsg, 158, security, 0, 30);
            Array.Copy(pucCHMsg, 188, startdate, 0, 16);
            Array.Copy(pucCHMsg, 204, enddate, 0, 16);
            string str = "";
            user.Name = Encoding.Unicode.GetString(name);
            str = Encoding.Unicode.GetString(sex);

            #region 性别

            if (str == "1")
            {
                user.Sex = "男";
            }
            else if (str == "0")
            {
                user.Sex = "未知";
            }
            else if (str == "2")
            {
                user.Sex = "女";
            }
            else
            {
                user.Sex = "未说明";
            }

            #endregion

            user.Birth = Encoding.Unicode.GetString(brath);
            user.Addr = Encoding.Unicode.GetString(address);
            user.Identity = Encoding.Unicode.GetString(num);
            // user.Country = Encoding.Unicode.GetString(security);
            //user.Brath = Encoding.Unicode.GetString(brath);
            //user.Address = Encoding.Unicode.GetString(address);
            //user.Num = Encoding.Unicode.GetString(num);
            //user.Security = Encoding.Unicode.GetString(security);
            //user.Startdate = Encoding.Unicode.GetString(startdate);
            //user.Enddate = Encoding.Unicode.GetString(enddate);
            return user;
        }
    }
}