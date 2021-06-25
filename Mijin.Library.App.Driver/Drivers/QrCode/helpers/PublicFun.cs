using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    class PublicFun
    {
        public static String g_strVIDPID = "";
        public static int g_icbVidPid_SelectedIndex = -1;
        /// <summary>
        /// 通过vid，pid获得串口设备号
        /// </summary>
        /// <param name="vid">vid</param>
        /// <param name="pid">pid</param>
        /// <returns>串口号</returns>
        public static string GetPortNameFormVidPid(string vid, string pid)
        {
            Guid myGUID = Guid.Empty;
            string enumerator = "USB";
            try
            {
                IntPtr hDevInfo = QrCodeLib.SetupDiGetClassDevs(ref myGUID, enumerator, IntPtr.Zero, QrCodeLib.DIGCF_ALLCLASSES | QrCodeLib.DIGCF_PRESENT);
                if (hDevInfo.ToInt64() == QrCodeLib.INVALID_HANDLE_VALUE)
                {
                    throw new Exception("没有该类设备");
                }
                QrCodeLib.SP_DEVINFO_DATA deviceInfoData;//想避免在api中使用ref，就把structure映射成类
                deviceInfoData = new QrCodeLib.SP_DEVINFO_DATA();

                //需要.NET CF4.0以上-------------------------------------------------------------------
                //if (Environment.Is64BitOperatingSystem)
                //    deviceInfoData.cbSize = 32;
                //else
                //    deviceInfoData.cbSize = 28;
                //----------------------------------------------------------------------------------
                if (IntPtr.Size == 8)
                {
                    //64 bit
                    deviceInfoData.cbSize = 32;//如果要使用SP_DEVINFO_DATA，一定要给该项赋值32
                }
                else if (IntPtr.Size == 4)
                {
                    //32 bit
                    deviceInfoData.cbSize = 28;//如果要使用SP_DEVINFO_DATA，一定要给该项赋值28=16+4+4+4
                }
                else
                {
                    return null;
                }
                deviceInfoData.devInst = 0;
                deviceInfoData.classGuid = System.Guid.Empty;
                deviceInfoData.reserved = 0;
                UInt32 i;
                StringBuilder property = new StringBuilder(QrCodeLib.MAX_DEV_LEN);
                for (i = 0; QrCodeLib.SetupDiEnumDeviceInfo(hDevInfo, i, deviceInfoData); i++)
                {
                    //       Console.Write(deviceInfoData.classGuid.ToString());
                    //       HardWareOperation.SetupDiGetDeviceInstanceId(hDevInfo, deviceInfoData, porperty, (uint)porperty.Capacity, 0);
                    QrCodeLib.SetupDiGetDeviceRegistryProperty(hDevInfo, deviceInfoData,
                        (uint)QrCodeLib.SPDRP_.SPDRP_CLASS,
                        0, property, (uint)property.Capacity, IntPtr.Zero);
                    if (property.ToString().ToLower() != "ports") continue;//首先看看是不是串口设备（有些USB设备不是串口设备）
                    QrCodeLib.SetupDiGetDeviceRegistryProperty(hDevInfo, deviceInfoData,
                        (uint)QrCodeLib.SPDRP_.SPDRP_HARDWAREID,
                        0, property, (uint)property.Capacity, IntPtr.Zero);
                    if (!(property.ToString().ToLower().Contains(vid.ToLower())
                        && property.ToString().ToLower().Contains(pid.ToLower()))) continue;//找到对应于vid&pid的设备
                    QrCodeLib.SetupDiGetDeviceRegistryProperty(hDevInfo, deviceInfoData,
                        (uint)QrCodeLib.SPDRP_.SPDRP_FRIENDLYNAME,
                        0, property, (uint)property.Capacity, IntPtr.Zero);
                    break;

                }
                QrCodeLib.SetupDiDestroyDeviceInfoList(hDevInfo);//记得用完释放相关内存
                if (property.ToString().IndexOf("COM") == -1)
                {
                    return null;
                }
                string friendlyName = property.ToString();
                char[] separatorMark = { '(', ')' };
                string[] strList1 = friendlyName.Split(separatorMark);
                if (strList1[1].Substring(0, 3) == "COM")
                {
                    return strList1[1];
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
