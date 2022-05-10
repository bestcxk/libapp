using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.DhCamera
{
    /// <summary>
    /// 大华数据结构体
    /// </summary>
    public static class DhStruct
    {
        /// <summary>
        /// initialization parameter structure
        /// 初始化接口参数结构体
        /// </summary>
        public struct NETSDK_INIT_PARAM
        {
            /// <summary>
            /// specify netsdk's normal network process thread number, zero means using default value
            /// 指定NetSDK常规网络处理线程数, 当值为0时, 使用内部默认值
            /// </summary>
            public int nThreadNum;
            /// <summary>
            /// reserved
            /// 保留字节
            /// </summary>                      
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] bReserved;
        }

        /// <summary>
        /// video statistical summary
        /// 视频统计摘要信息
        /// </summary>
        public struct NET_VIDEOSTAT_SUMMARY
        {
            /// <summary>
            /// channel ID 
            /// 通道号
            /// </summary>
            public int nChannelID;
            /// <summary>
            /// rule name
            /// 规则名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] szRuleName;
            /// <summary>
            /// time of this statistics
            /// 统计时间
            /// </summary>
            public NET_TIME_EX stuTime;
            /// <summary>
            /// subtotal for the entered
            /// 进入小计
            /// </summary>
            public NET_VIDEOSTAT_SUBTOTAL stuEnteredSubtotal;
            /// <summary>
            /// subtotal for the exited
            /// 出去小计
            /// </summary>
            public NET_VIDEOSTAT_SUBTOTAL stuExitedSubtotal;
            /// <summary>
            /// num of the inside
            /// 区域内人数
            /// </summary>
            public uint nInsidePeopleNum;
            /// <summary>
            /// rule type
            /// 规则类型
            /// </summary>
            public EM_RULE_TYPE emRuleType;
            /// <summary>
            /// num of the exit
            /// 离开的人数个数
            /// </summary>
            public int nRetExitManNum;
            /// <summary>
            /// 离开人员的滞留时间信息
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public NET_EXITMAN_STAY_STAT[] stuExitManStayInfo;
            /// <summary>
            /// 计划ID,仅球机有效,从1开始
            /// </summary>
            public uint nPlanID;
            /// <summary>
            /// 区域ID(一个预置点可以对应多个区域ID)
            /// </summary>
            public uint nAreaID;
            /// <summary>
            /// 当天区域内总人数
            /// </summary>
            public uint nCurrentDayInsidePeopleNum;
            /// <summary>
            /// reserved
            /// 保留字节
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1012)]
            public byte[] reserved;
        }

        /// <summary>
        /// time struct
        /// 时间信息结构体
        /// </summary>
        public struct NET_TIME_EX
        {
            /// <summary>
            /// Year
            /// 年
            /// </summary>
            public uint dwYear;
            /// <summary>
            /// Month
            /// 月
            /// </summary>
            public uint dwMonth;
            /// <summary>
            /// Day
            /// 日
            /// </summary>
            public uint dwDay;
            /// <summary>
            /// Hour
            /// 时
            /// </summary>
            public uint dwHour;
            /// <summary>
            /// Minute
            /// 分
            /// </summary>
            public uint dwMinute;
            /// <summary>
            /// Second
            /// 秒
            /// </summary>
            public uint dwSecond;
            /// <summary>
            /// Millisecond
            /// 毫秒
            /// </summary>
            public uint dwMillisecond;
            /// <summary>
            /// indicates UTC, invalid when 0
            /// utc时间(获取时0表示无效，非0有效 下发无效)
            /// </summary>
            public uint dwUTC;
            /// <summary>
            /// reserved
            /// 保留
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public uint[] dwReserved;

            /// <summary>
            /// override tostring function
            /// 重写tostring函数
            /// </summary>
            /// <returns>timer string</returns>
            public override string ToString()
            {
                return string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}", dwYear.ToString("D4"), dwMonth.ToString("D2"), dwDay.ToString("D2"), dwHour.ToString("D2"), dwMinute.ToString("D2"), dwSecond.ToString("D2"), dwMillisecond.ToString("D3"));
            }

            public string ToShortString()
            {
                return string.Format("{0}-{1}-{2} {3}:{4}:{5}", dwYear.ToString("D4"), dwMonth.ToString("D2"), dwDay.ToString("D2"), dwHour.ToString("D2"), dwMinute.ToString("D2"), dwSecond.ToString("D2"));
            }

            public DateTime ToDateTime()
            {
                try
                {
                    return new DateTime((int)dwYear, (int)dwMonth, (int)dwDay, (int)dwHour, (int)dwMinute, (int)dwSecond, (int)dwMillisecond);
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }

        /// <summary>
        /// video statistical subtotal
        /// 视频统计小计信息
        /// </summary>
        public struct NET_VIDEOSTAT_SUBTOTAL
        {
            /// <summary>
            /// count since device operation
            /// 设备运行后人数统计总数
            /// </summary>
            public int nTotal;
            /// <summary>
            /// count in the last hour
            /// 小时内的总人数
            /// </summary>
            public int nHour;
            /// <summary>
            /// count for today
            /// 当天的总人数, 不可手动清除
            /// </summary>
            public int nToday;
            /// <summary>
            /// count for today, on screen display 
            /// 统计人数, 用于OSD显示, 可手动清除
            /// </summary>
            public int nOSD;
            /// <summary>
            /// reserved
            /// 保留字节
            /// </summary>   
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 252)]
            public byte[] reserved;
        }

        /// <summary>
        /// The type of rule
        /// 规则类型
        /// </summary>
        public enum EM_RULE_TYPE
        {
            /// <summary>
            /// Unknown
            /// 未知
            /// </summary>
            EM_RULE_UNKNOWN,
            /// <summary>
            /// NumberStat
            /// 人数统计
            /// </summary>
            EM_RULE_NUMBER_STAT,
            /// <summary>
            /// Man number detection
            /// 区域内人数统计
            /// </summary>
            EM_RULE_MAN_NUM_DETECTION,
        }

        /// <summary>
        /// 离开人员的滞留时间信息
        /// </summary>
        public struct NET_EXITMAN_STAY_STAT
        {
            public NET_TIME stuEnterTime;       // 人员进入区域时间
            public NET_TIME stuExitTime;        // 人员离开区域时间
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] reserved;             //保留字节
        }

        /// <summary>
        /// Time structure
        /// 时间结构体
        /// </summary>
        public struct NET_TIME
        {
            /// <summary>
            /// Year
            /// 年
            /// </summary>
            public uint dwYear;
            /// <summary>
            /// Month
            /// 月
            /// </summary>
            public uint dwMonth;
            /// <summary>
            /// Day
            /// 天
            /// </summary>
            public uint dwDay;
            /// <summary>
            /// Hour
            /// 小时
            /// </summary>
            public uint dwHour;
            /// <summary>
            /// Minute
            /// 分
            /// </summary>
            public uint dwMinute;
            /// <summary>
            /// Second
            /// 秒
            /// </summary>
            public uint dwSecond;
            /// <summary>
            /// DateTime change to NET_TIME static funtion.
            /// DateTime转为NET_TIME静态函数
            /// </summary>
            /// <param name="dateTime">datetime</param>
            /// <returns>NET_TIME</returns>
            public static NET_TIME FromDateTime(DateTime dateTime)
            {
                try
                {
                    NET_TIME net_time = new NET_TIME();
                    net_time.dwYear = (uint)dateTime.Year;
                    net_time.dwMonth = (uint)dateTime.Month;
                    net_time.dwDay = (uint)dateTime.Day;
                    net_time.dwHour = (uint)dateTime.Hour;
                    net_time.dwMinute = (uint)dateTime.Minute;
                    net_time.dwSecond = (uint)dateTime.Second;
                    return net_time;
                }
                catch
                {
                    return new NET_TIME();
                }
            }
            /// <summary>
            /// change NET_TIME to DateTime
            /// NET_TIME 转为 DateTime
            /// </summary>
            /// <returns>DateTime</returns>
            public DateTime ToDateTime()
            {
                try
                {
                    return new DateTime((int)dwYear, (int)dwMonth, (int)dwDay, (int)dwHour, (int)dwMinute, (int)dwSecond);
                }
                catch
                {
                    return DateTime.Now;
                }
            }
            /// <summary>
            /// oveeride toString function
            /// 重写toString函数
            /// </summary>
            /// <returns>return time string</returns>
            public override string ToString()
            {
                return string.Format("{0}-{1}-{2} {3}:{4}:{5}", dwYear.ToString("D4"), dwMonth.ToString("D2"), dwDay.ToString("D2"), dwHour.ToString("D2"), dwMinute.ToString("D2"), dwSecond.ToString("D2"));
            }
        }

        /// <summary>
        /// SDK error code number enumeration
        /// SDK错误码枚举
        /// </summary>
        public enum EM_ErrorCode : uint
        {
            /// <summary>
            /// No error
            /// 没有错误
            /// </summary>
            NET_NOERROR = 0,
            /// <summary>
            /// Unknown error
            /// 未知错误
            /// </summary>
            NET_ERROR = 0xFFFFFFFF,
            /// <summary>
            /// Windows system error
            /// Windows系统错误
            /// </summary>
            NET_SYSTEM_ERROR = 0x80000000 | 1,
            /// <summary>
            /// Protocol error it may result from network timeout
            /// 网络错误,可能是因为网络超时
            /// </summary>
            NET_NETWORK_ERROR = 0x80000000 | 2,
            /// <summary>
            /// Device protocol does not match
            /// 设备协议不匹配
            /// </summary>
            NET_DEV_VER_NOMATCH = 0x80000000 | 3,
            /// <summary>
            /// Handle is invalid
            /// 句柄无效
            /// </summary>
            NET_INVALID_HANDLE = 0x80000000 | 4,
            /// <summary>
            /// Failed to open channel
            /// 打开通道失败
            /// </summary>
            NET_OPEN_CHANNEL_ERROR = 0x80000000 | 5,
            /// <summary>
            /// Failed to close channel
            /// 关闭通道失败
            /// </summary>
            NET_CLOSE_CHANNEL_ERROR = 0x80000000 | 6,
            /// <summary>
            /// User parameter is illegal
            /// 用户参数不合法
            /// </summary>
            NET_ILLEGAL_PARAM = 0x80000000 | 7,
            /// <summary>
            /// SDK initialization error
            /// SDK初始化出错
            /// </summary>
            NET_SDK_INIT_ERROR = 0x80000000 | 8,
            /// <summary>
            /// SDK clear error 
            /// SDK清理出错
            /// </summary>
            NET_SDK_UNINIT_ERROR = 0x80000000 | 9,
            /// <summary>
            /// Error occurs when apply for render resources
            /// 申请render资源出错
            /// </summary>
            NET_RENDER_OPEN_ERROR = 0x80000000 | 10,
            /// <summary>
            /// Error occurs when opening the decoder library
            /// 打开解码库出错
            /// </summary>
            NET_DEC_OPEN_ERROR = 0x80000000 | 11,
            /// <summary>
            /// Error occurs when closing the decoder library
            /// 关闭解码库出错
            /// </summary>
            NET_DEC_CLOSE_ERROR = 0x80000000 | 12,
            /// <summary>
            /// The detected channel number is 0 in multiple-channel preview
            /// 多画面预览中检测到通道数为0
            /// </summary>
            NET_MULTIPLAY_NOCHANNEL = 0x80000000 | 13,
            /// <summary>
            /// Failed to initialize record library
            /// 录音库初始化失败
            /// </summary>
            NET_TALK_INIT_ERROR = 0x80000000 | 14,
            /// <summary>
            /// The record library has not been initialized
            /// 录音库未经初始化
            /// </summary>
            NET_TALK_NOT_INIT = 0x80000000 | 15,
            /// <summary>
            /// Error occurs when sending out audio data
            /// 发送音频数据出错
            /// </summary>
            NET_TALK_SENDDATA_ERROR = 0x80000000 | 16,
            /// <summary>
            /// The real-time has been protected
            /// 实时数据已经处于保存状态
            /// </summary>
            NET_REAL_ALREADY_SAVING = 0x80000000 | 17,
            /// <summary>
            /// The real-time data has not been save
            /// 未保存实时数据
            /// </summary>
            NET_NOT_SAVING = 0x80000000 | 18,
            /// <summary>
            /// Error occurs when opening the file
            /// 打开文件出错
            /// </summary>
            NET_OPEN_FILE_ERROR = 0x80000000 | 19,
            /// <summary>
            /// Failed to enable PTZ to control timer
            /// 启动云台控制定时器失败
            /// </summary>
            NET_PTZ_SET_TIMER_ERROR = 0x80000000 | 20,
            /// <summary>
            /// Error occurs when verify returned data
            /// 对返回数据的校验出错
            /// </summary>
            NET_RETURN_DATA_ERROR = 0x80000000 | 21,
            /// <summary>
            /// There is no sufficient buffer
            /// 没有足够的缓存
            /// </summary>
            NET_INSUFFICIENT_BUFFER = 0x80000000 | 22,
            /// <summary>
            /// The current SDK does not support this fucntion
            /// 当前SDK未支持该功能
            /// </summary>
            NET_NOT_SUPPORTED = 0x80000000 | 23,
            /// <summary>
            /// There is no searched result
            /// 查询不到录象
            /// </summary>
            NET_NO_RECORD_FOUND = 0x80000000 | 24,
            /// <summary>
            /// You have no operation right
            /// 无操作权限
            /// </summary>
            NET_NOT_AUTHORIZED = 0x80000000 | 25,
            /// <summary>
            /// Can not operate right now
            /// 暂时无法执行
            /// </summary>
            NET_NOT_NOW = 0x80000000 | 26,
            /// <summary>
            /// There is no audio talk channel
            /// 未发现对讲通道
            /// </summary>
            NET_NO_TALK_CHANNEL = 0x80000000 | 27,
            /// <summary>
            /// There is no audio
            /// 未发现音频
            /// </summary>
            NET_NO_AUDIO = 0x80000000 | 28,
            /// <summary>
            /// The network SDK has not been initialized
            /// 网络SDK未经初始化
            /// </summary>
            NET_NO_INIT = 0x80000000 | 29,
            /// <summary>
            /// The download completed
            /// 下载已结束
            /// </summary>
            NET_DOWNLOAD_END = 0x80000000 | 30,
            /// <summary>
            /// There is no searched result
            /// 查询结果为空
            /// </summary>
            NET_EMPTY_LIST = 0x80000000 | 31,
            /// <summary>
            /// Failed to get system property setup
            /// 获取系统属性配置失败
            /// </summary>
            NET_ERROR_GETCFG_SYSATTR = 0x80000000 | 32,
            /// <summary>
            /// Failed to get SN
            /// 获取序列号失败
            /// </summary>
            NET_ERROR_GETCFG_SERIAL = 0x80000000 | 33,
            /// <summary>
            /// Failed to get general property
            /// 获取常规属性失败
            /// </summary>
            NET_ERROR_GETCFG_GENERAL = 0x80000000 | 34,
            /// <summary>
            /// Failed to get DSP capacity description
            /// 获取DSP能力描述失败
            /// </summary>
            NET_ERROR_GETCFG_DSPCAP = 0x80000000 | 35,
            /// <summary>
            /// Failed to get network channel setup
            /// 获取网络配置失败
            /// </summary>
            NET_ERROR_GETCFG_NETCFG = 0x80000000 | 36,
            /// <summary>
            /// Failed to get channel name
            /// 获取通道名称失败
            /// </summary>
            NET_ERROR_GETCFG_CHANNAME = 0x80000000 | 37,
            /// <summary>
            /// Failed to get video property
            /// 获取视频属性失败
            /// </summary>
            NET_ERROR_GETCFG_VIDEO = 0x80000000 | 38,
            /// <summary>
            /// Failed to get record setup
            /// 获取录象配置失败
            /// </summary>
            NET_ERROR_GETCFG_RECORD = 0x80000000 | 39,
            /// <summary>
            /// Failed to get decoder protocol name
            /// 获取解码器协议名称失败
            /// </summary>
            NET_ERROR_GETCFG_PRONAME = 0x80000000 | 40,
            /// <summary>
            /// Failed to get 232 COM function name
            /// 获取232串口功能名称失败
            /// </summary>
            NET_ERROR_GETCFG_FUNCNAME = 0x80000000 | 41,
            /// <summary>
            /// Failed to get decoder property
            /// 获取解码器属性失败
            /// </summary>
            NET_ERROR_GETCFG_485DECODER = 0x80000000 | 42,
            /// <summary>
            /// Failed to get 232 COM setup
            /// 获取232串口配置失败
            /// </summary>
            NET_ERROR_GETCFG_232COM = 0x80000000 | 43,
            /// <summary>
            /// Failed to get external alarm input setup
            /// 获取外部报警输入配置失败
            /// </summary>
            NET_ERROR_GETCFG_ALARMIN = 0x80000000 | 44,
            /// <summary>
            /// Failed to get motion detection alarm
            /// 获取动态检测报警失败
            /// </summary>
            NET_ERROR_GETCFG_ALARMDET = 0x80000000 | 45,
            /// <summary>
            /// Failed to get device time
            /// 获取设备时间失败
            /// </summary>
            NET_ERROR_GETCFG_SYSTIME = 0x80000000 | 46,
            /// <summary>
            /// Failed to get preview parameter
            /// 获取预览参数失败
            /// </summary>
            NET_ERROR_GETCFG_PREVIEW = 0x80000000 | 47,
            /// <summary>
            /// Failed to get audio maintenance setup
            /// 获取自动维护配置失败
            /// </summary>
            NET_ERROR_GETCFG_AUTOMT = 0x80000000 | 48,
            /// <summary>
            /// Failed to get video matrix setup
            /// 获取视频矩阵配置失败
            /// </summary>
            NET_ERROR_GETCFG_VIDEOMTRX = 0x80000000 | 49,
            /// <summary>
            /// Failed to get privacy mask zone setup
            /// 获取区域遮挡配置失败
            /// </summary>
            NET_ERROR_GETCFG_COVER = 0x80000000 | 50,
            /// <summary>
            /// Failed to get video watermark setup
            /// 获取图象水印配置失败
            /// </summary>
            NET_ERROR_GETCFG_WATERMAKE = 0x80000000 | 51,
            /// <summary>
            /// Failed to get config￡omulticast port by channel
            /// 获取配置失败位置：组播端口按通道配置
            /// </summary>
            NET_ERROR_GETCFG_MULTICAST = 0x80000000 | 52,
            /// <summary>
            /// Failed to modify general property
            /// 修改常规属性失败
            /// </summary>
            NET_ERROR_SETCFG_GENERAL = 0x80000000 | 55,
            /// <summary>
            /// Failed to modify channel setup
            /// 修改网络配置失败
            /// </summary>
            NET_ERROR_SETCFG_NETCFG = 0x80000000 | 56,
            /// <summary>
            /// Failed to modify channel name
            /// 修改通道名称失败
            /// </summary>
            NET_ERROR_SETCFG_CHANNAME = 0x80000000 | 57,
            /// <summary>
            /// Failed to modify video channel
            /// 修改视频属性失败
            /// </summary>
            NET_ERROR_SETCFG_VIDEO = 0x80000000 | 58,
            /// <summary>
            /// Failed to modify record setup
            /// 修改录象配置失败
            /// </summary>
            NET_ERROR_SETCFG_RECORD = 0x80000000 | 59,
            /// <summary>
            /// Failed to modify decoder property 
            /// 修改解码器属性失败
            /// </summary>
            NET_ERROR_SETCFG_485DECODER = 0x80000000 | 60,
            /// <summary>
            /// Failed to modify 232 COM setup
            /// 修改232串口配置失败
            /// </summary>
            NET_ERROR_SETCFG_232COM = 0x80000000 | 61,
            /// <summary>
            /// Failed to modify external input alarm setup
            /// 修改外部输入报警配置失败
            /// </summary>
            NET_ERROR_SETCFG_ALARMIN = 0x80000000 | 62,
            /// <summary>
            /// Failed to modify motion detection alarm setup
            /// 修改动态检测报警配置失败
            /// </summary>
            NET_ERROR_SETCFG_ALARMDET = 0x80000000 | 63,
            /// <summary>
            /// Failed to modify device time
            /// 修改设备时间失败
            /// </summary>
            NET_ERROR_SETCFG_SYSTIME = 0x80000000 | 64,
            /// <summary>
            /// Failed to modify preview parameter
            /// 修改预览参数失败
            /// </summary>
            NET_ERROR_SETCFG_PREVIEW = 0x80000000 | 65,
            /// <summary>
            /// Failed to modify auto maintenance setup
            /// 修改自动维护配置失败
            /// </summary>
            NET_ERROR_SETCFG_AUTOMT = 0x80000000 | 66,
            /// <summary>
            /// Failed to modify video matrix setup
            /// 修改视频矩阵配置失败
            /// </summary>
            NET_ERROR_SETCFG_VIDEOMTRX = 0x80000000 | 67,
            /// <summary>
            /// Failed to modify privacy mask zone
            /// 修改区域遮挡配置失败
            /// </summary>
            NET_ERROR_SETCFG_COVER = 0x80000000 | 68,
            /// <summary>
            /// Failed to modify video watermark setup
            /// 修改图象水印配置失败
            /// </summary>
            NET_ERROR_SETCFG_WATERMAKE = 0x80000000 | 69,
            /// <summary>
            /// Failed to modify wireless network information
            /// 修改无线网络信息失败
            /// </summary>
            NET_ERROR_SETCFG_WLAN = 0x80000000 | 70,
            /// <summary>
            /// Failed to select wireless network device
            /// 选择无线网络设备失败
            /// </summary>
            NET_ERROR_SETCFG_WLANDEV = 0x80000000 | 71,
            /// <summary>
            /// Failed to modify the actively registration parameter setup
            /// 修改主动注册参数配置失败
            /// </summary>
            NET_ERROR_SETCFG_REGISTER = 0x80000000 | 72,
            /// <summary>
            /// Failed to modify camera property
            /// 修改摄像头属性配置失败
            /// </summary>
            NET_ERROR_SETCFG_CAMERA = 0x80000000 | 73,
            /// <summary>
            /// Failed to modify IR alarm setup
            /// 修改红外报警配置失败
            /// </summary>
            NET_ERROR_SETCFG_INFRARED = 0x80000000 | 74,
            /// <summary>
            /// Failed to modify audio alarm setup
            /// 修改音频报警配置失败
            /// </summary>
            NET_ERROR_SETCFG_SOUNDALARM = 0x80000000 | 75,
            /// <summary>
            /// Failed to modify storage position setup
            /// 修改存储位置配置失败
            /// </summary>
            NET_ERROR_SETCFG_STORAGE = 0x80000000 | 76,
            /// <summary>
            /// The audio encode port has not been successfully initialized
            /// 音频编码接口没有成功初始化
            /// </summary>
            NET_AUDIOENCODE_NOTINIT = 0x80000000 | 77,
            /// <summary>
            /// The data are too long
            /// 数据过长
            /// </summary>
            NET_DATA_TOOLONGH = 0x80000000 | 78,
            /// <summary>
            /// The device does not support current operation
            /// 设备不支持该操作
            /// </summary>
            NET_UNSUPPORTED = 0x80000000 | 79,
            /// <summary>
            /// Device resources is not sufficient
            /// 设备资源不足
            /// </summary>
            NET_DEVICE_BUSY = 0x80000000 | 80,
            /// <summary>
            /// The server has boot up
            /// 服务器已经启动
            /// </summary>
            NET_SERVER_STARTED = 0x80000000 | 81,
            /// <summary>
            /// The server has not fully boot up
            /// 服务器尚未成功启动
            /// </summary>
            NET_SERVER_STOPPED = 0x80000000 | 82,
            /// <summary>
            /// Input serial number is not correct
            /// 输入序列号有误
            /// </summary>
            NET_LISTER_INCORRECT_SERIAL = 0x80000000 | 83,
            /// <summary>
            /// Failed to get HDD information
            /// 获取硬盘信息失败
            /// </summary>
            NET_QUERY_DISKINFO_FAILED = 0x80000000 | 84,
            /// <summary>
            /// Failed to get connect session information
            /// 获取连接Session信息
            /// </summary>
            NET_ERROR_GETCFG_SESSION = 0x80000000 | 85,
            /// <summary>
            /// The password you typed is incorrect. You have exceeded the maximum number of retries
            /// 输入密码错误超过限制次数
            /// </summary>
            NET_USER_FLASEPWD_TRYTIME = 0x80000000 | 86,
            /// <summary>
            /// Password is not correct
            /// 密码不正确
            /// </summary>
            NET_LOGIN_ERROR_PASSWORD = 0x80000000 | 100,
            /// <summary>
            /// The account does not exist
            /// 帐户不存在
            /// </summary>
            NET_LOGIN_ERROR_USER = 0x80000000 | 101,
            /// <summary>
            /// Time out for log in returned value
            /// 等待登录返回超时
            /// </summary>
            NET_LOGIN_ERROR_TIMEOUT = 0x80000000 | 102,
            /// <summary>
            /// The account has logged in
            /// 帐号已登录
            /// </summary>
            NET_LOGIN_ERROR_RELOGGIN = 0x80000000 | 103,
            /// <summary>
            /// The account has been locked
            /// 帐号已被锁定
            /// </summary>
            NET_LOGIN_ERROR_LOCKED = 0x80000000 | 104,
            /// <summary>
            /// The account bas been in the black list
            /// 帐号已被列为黑名单
            /// </summary>
            NET_LOGIN_ERROR_BLACKLIST = 0x80000000 | 105,
            /// <summary>
            /// Resources are not sufficient. System is busy now
            /// 资源不足,系统忙
            /// </summary>
            NET_LOGIN_ERROR_BUSY = 0x80000000 | 106,
            /// <summary>
            /// Time out. Please check network and try again
            /// 登录设备超时,请检查网络并重试
            /// </summary>
            NET_LOGIN_ERROR_CONNECT = 0x80000000 | 107,
            /// <summary>
            /// Network connection failed
            /// 网络连接失败
            /// </summary>
            NET_LOGIN_ERROR_NETWORK = 0x80000000 | 108,
            /// <summary>
            /// Successfully logged in the device but can not create video channel. Please check network connection
            /// 登录设备成功,但无法创建视频通道,请检查网络状况
            /// </summary>
            NET_LOGIN_ERROR_SUBCONNECT = 0x80000000 | 109,
            /// <summary>
            /// exceed the max connect number
            /// 超过最大连接数
            /// </summary>
            NET_LOGIN_ERROR_MAXCONNECT = 0x80000000 | 110,
            /// <summary>
            /// protocol 3 support
            /// 只支持3代协议
            /// </summary>
            NET_LOGIN_ERROR_PROTOCOL3_ONLY = 0x80000000 | 111,
            /// <summary>
            /// There is no USB or USB info error
            /// 未插入U盾或U盾信息错误
            /// </summary>
            NET_LOGIN_ERROR_UKEY_LOST = 0x80000000 | 112,
            /// <summary>
            /// Client-end IP address has no right to login
            /// 客户端IP地址没有登录权限
            /// </summary>
            NET_LOGIN_ERROR_NO_AUTHORIZED = 0x80000000 | 113,
            /// <summary>
            /// user or password error
            /// 账号或密码错误 
            /// </summary>
            NET_LOGIN_ERROR_USER_OR_PASSOWRD = 0X80000000 | 117,
            /// <summary>
            /// Error occurs when Render library open audio
            /// Render库打开音频出错
            /// </summary>
            NET_RENDER_SOUND_ON_ERROR = 0x80000000 | 120,
            /// <summary>
            /// Error occurs when Render library close audio
            /// Render库关闭音频出错
            /// </summary>
            NET_RENDER_SOUND_OFF_ERROR = 0x80000000 | 121,
            /// <summary>
            /// Error occurs when Render library control volume
            /// Render库控制音量出错
            /// </summary>
            NET_RENDER_SET_VOLUME_ERROR = 0x80000000 | 122,
            /// <summary>
            /// Error occurs when Render library set video parameter
            /// Render库设置画面参数出错
            /// </summary>
            NET_RENDER_ADJUST_ERROR = 0x80000000 | 123,
            /// <summary>
            /// Error occurs when Render library pause play
            /// Render库暂停播放出错
            /// </summary>
            NET_RENDER_PAUSE_ERROR = 0x80000000 | 124,
            /// <summary>
            /// Render library snapshot error
            /// Render库抓图出错
            /// </summary>
            NET_RENDER_SNAP_ERROR = 0x80000000 | 125,
            /// <summary>
            /// Render library stepper error
            /// Render库步进出错
            /// </summary>
            NET_RENDER_STEP_ERROR = 0x80000000 | 126,
            /// <summary>
            /// Error occurs when Render library set frame rate
            /// Render库设置帧率出错
            /// </summary>
            NET_RENDER_FRAMERATE_ERROR = 0x80000000 | 127,
            /// <summary>
            /// Error occurs when Render lib setting show region
            /// Render库设置显示区域出错
            /// </summary>
            NET_RENDER_DISPLAYREGION_ERROR = 0x80000000 | 128,
            /// <summary>
            /// An error occurred when Render library getting current play time
            /// Render库获取当前播放时间出错
            /// </summary>
            NET_RENDER_GETOSDTIME_ERROR = 0x80000000 | 129,
            /// <summary>
            /// Group name has been existed
            /// 组名已存在
            /// </summary>
            NET_GROUP_EXIST = 0x80000000 | 140,
            /// <summary>
            /// The group name does not exist
            /// 组名不存在
            /// </summary>
            NET_GROUP_NOEXIST = 0x80000000 | 141,
            /// <summary>
            /// The group right exceeds the right list
            /// 组的权限超出权限列表范围
            /// </summary>
            NET_GROUP_RIGHTOVER = 0x80000000 | 142,
            /// <summary>
            /// The group can not be removed since there is user in it
            /// 组下有用户,不能删除
            /// </summary>
            NET_GROUP_HAVEUSER = 0x80000000 | 143,
            /// <summary>
            /// The user has used one of the group right. It can not be removed
            /// 组的某个权限被用户使用,不能出除
            /// </summary>
            NET_GROUP_RIGHTUSE = 0x80000000 | 144,
            /// <summary>
            /// New group name has been existed
            /// 新组名同已有组名重复
            /// </summary>
            NET_GROUP_SAMENAME = 0x80000000 | 145,
            /// <summary>
            /// The user name has been existed
            /// 用户已存在
            /// </summary>
            NET_USER_EXIST = 0x80000000 | 146,
            /// <summary>
            /// The account does not exist
            /// 用户不存在
            /// </summary>
            NET_USER_NOEXIST = 0x80000000 | 147,
            /// <summary>
            /// User right exceeds the group right
            /// 用户权限超出组权限
            /// </summary>
            NET_USER_RIGHTOVER = 0x80000000 | 148,
            /// <summary>
            /// Reserved account. It does not allow to be modified
            /// 保留帐号,不容许修改密码
            /// </summary>
            NET_USER_PWD = 0x80000000 | 149,
            /// <summary>
            /// password is not correct
            /// 密码不正确
            /// </summary>
            NET_USER_FLASEPWD = 0x80000000 | 150,
            /// <summary>
            /// Password is invalid
            /// 密码不匹配
            /// </summary>
            NET_USER_NOMATCHING = 0x80000000 | 151,
            /// <summary>
            /// account in use
            /// 账号正在使用中
            /// </summary>
            NET_USER_INUSE = 0x80000000 | 152,
            /// <summary>
            /// Failed to get network card setup
            /// 获取网卡配置失败
            /// </summary>
            NET_ERROR_GETCFG_ETHERNET = 0x80000000 | 300,
            /// <summary>
            /// Failed to get wireless network information
            /// 获取无线网络信息失败
            /// </summary>
            NET_ERROR_GETCFG_WLAN = 0x80000000 | 301,
            /// <summary>
            /// Failed to get wireless network device
            /// 获取无线网络设备失败
            /// </summary>
            NET_ERROR_GETCFG_WLANDEV = 0x80000000 | 302,
            /// <summary>
            /// Failed to get actively registration parameter
            /// 获取主动注册参数失败
            /// </summary>
            NET_ERROR_GETCFG_REGISTER = 0x80000000 | 303,
            /// <summary>
            /// Failed to get camera property
            /// 获取摄像头属性失败
            /// </summary>
            NET_ERROR_GETCFG_CAMERA = 0x80000000 | 304,
            /// <summary>
            /// Failed to get IR alarm setup
            /// 获取红外报警配置失败
            /// </summary>
            NET_ERROR_GETCFG_INFRARED = 0x80000000 | 305,
            /// <summary>
            /// Failed to get audio alarm setup
            /// 获取音频报警配置失败
            /// </summary>
            NET_ERROR_GETCFG_SOUNDALARM = 0x80000000 | 306,
            /// <summary>
            /// Failed to get storage position
            /// 获取存储位置配置失败
            /// </summary>
            NET_ERROR_GETCFG_STORAGE = 0x80000000 | 307,
            /// <summary>
            /// Failed to get mail setup.
            /// 获取邮件配置失败
            /// </summary>
            NET_ERROR_GETCFG_MAIL = 0x80000000 | 308,
            /// <summary>
            /// Can not set right now.
            /// 暂时无法设置
            /// </summary>
            NET_CONFIG_DEVBUSY = 0x80000000 | 309,
            /// <summary>
            /// The configuration setup data are illegal.
            /// 配置数据不合法
            /// </summary>
            NET_CONFIG_DATAILLEGAL = 0x80000000 | 310,
            /// <summary>
            /// Failed to get DST setup
            /// 获取夏令时配置失败
            /// </summary>
            NET_ERROR_GETCFG_DST = 0x80000000 | 311,
            /// <summary>
            /// Failed to set DST 
            /// 设置夏令时配置失败
            /// </summary>
            NET_ERROR_SETCFG_DST = 0x80000000 | 312,
            /// <summary>
            /// Failed to get video osd setup.
            /// 获取视频OSD叠加配置失败
            /// </summary>
            NET_ERROR_GETCFG_VIDEO_OSD = 0x80000000 | 313,
            /// <summary>
            /// Failed to set video osd 
            /// 设置视频OSD叠加配置失败
            /// </summary>
            NET_ERROR_SETCFG_VIDEO_OSD = 0x80000000 | 314,
            /// <summary>
            /// Failed to get CDMA\GPRS configuration
            /// 获取CDMA\GPRS网络配置失败
            /// </summary>
            NET_ERROR_GETCFG_GPRSCDMA = 0x80000000 | 315,
            /// <summary>
            /// Failed to set CDMA\GPRS configuration
            /// 设置CDMA\GPRS网络配置失败
            /// </summary>
            NET_ERROR_SETCFG_GPRSCDMA = 0x80000000 | 316,
            /// <summary>
            /// Failed to get IP Filter configuration
            /// 获取IP过滤配置失败
            /// </summary>
            NET_ERROR_GETCFG_IPFILTER = 0x80000000 | 317,
            /// <summary>
            /// Failed to set IP Filter configuration
            /// 设置IP过滤配置失败
            /// </summary>
            NET_ERROR_SETCFG_IPFILTER = 0x80000000 | 318,
            /// <summary>
            /// Failed to get Talk Encode configuration
            /// 获取语音对讲编码配置失败
            /// </summary>
            NET_ERROR_GETCFG_TALKENCODE = 0x80000000 | 319,
            /// <summary>
            /// Failed to set Talk Encode configuration
            /// 设置语音对讲编码配置失败
            /// </summary>
            NET_ERROR_SETCFG_TALKENCODE = 0x80000000 | 320,
            /// <summary>
            /// Failed to get The length of the video package configuration
            /// 获取录像打包长度配置失败
            /// </summary>
            NET_ERROR_GETCFG_RECORDLEN = 0x80000000 | 321,
            /// <summary>
            /// Failed to set The length of the video package configuration
            /// 设置录像打包长度配置失败
            /// </summary>
            NET_ERROR_SETCFG_RECORDLEN = 0x80000000 | 322,
            /// <summary>
            /// Not support Network hard disk partition
            /// 不支持网络硬盘分区
            /// </summary>
            NET_DONT_SUPPORT_SUBAREA = 0x80000000 | 323,
            /// <summary>
            /// Failed to get the register server information
            /// 获取设备上主动注册服务器信息失败
            /// </summary>
            NET_ERROR_GET_AUTOREGSERVER = 0x80000000 | 324,
            /// <summary>
            /// Failed to control actively registration
            /// 主动注册重定向注册错误
            /// </summary>
            NET_ERROR_CONTROL_AUTOREGISTER = 0x80000000 | 325,
            /// <summary>
            /// Failed to disconnect actively registration
            /// 断开主动注册服务器错误
            /// </summary>
            NET_ERROR_DISCONNECT_AUTOREGISTER = 0x80000000 | 326,
            /// <summary>
            /// Failed to get mms configuration
            /// 获取mms配置失败
            /// </summary>
            NET_ERROR_GETCFG_MMS = 0x80000000 | 327,
            /// <summary>
            /// Failed to set mms configuration
            /// 设置mms配置失败
            /// </summary>
            NET_ERROR_SETCFG_MMS = 0x80000000 | 328,
            /// <summary>
            /// Failed to get SMS configuration
            /// 获取短信激活无线连接配置失败
            /// </summary>
            NET_ERROR_GETCFG_SMSACTIVATION = 0x80000000 | 329,
            /// <summary>
            /// Failed to set SMS configuration
            /// 设置短信激活无线连接配置失败
            /// </summary>
            NET_ERROR_SETCFG_SMSACTIVATION = 0x80000000 | 330,
            /// <summary>
            /// Failed to get activation of a wireless connection
            /// 获取拨号激活无线连接配置失败
            /// </summary>
            NET_ERROR_GETCFG_DIALINACTIVATION = 0x80000000 | 331,
            /// <summary>
            /// Failed to set activation of a wireless connection
            /// 设置拨号激活无线连接配置失败
            /// </summary>
            NET_ERROR_SETCFG_DIALINACTIVATION = 0x80000000 | 332,
            /// <summary>
            /// Failed to get the parameter of video output
            /// 查询视频输出参数配置失败
            /// </summary>
            NET_ERROR_GETCFG_VIDEOOUT = 0x80000000 | 333,
            /// <summary>
            /// Failed to set the configuration of video output
            /// 设置视频输出参数配置失败
            /// </summary>
            NET_ERROR_SETCFG_VIDEOOUT = 0x80000000 | 334,
            /// <summary>
            /// Failed to get osd overlay enabling
            /// 获取osd叠加使能配置失败
            /// </summary>
            NET_ERROR_GETCFG_OSDENABLE = 0x80000000 | 335,
            /// <summary>
            /// Failed to set OSD overlay enabling
            /// 设置osd叠加使能配置失败
            /// </summary>
            NET_ERROR_SETCFG_OSDENABLE = 0x80000000 | 336,
            /// <summary>
            /// Failed to set digital input configuration of front encoders
            /// 设置数字通道前端编码接入配置失败
            /// </summary>
            NET_ERROR_SETCFG_ENCODERINFO = 0x80000000 | 337,
            /// <summary>
            /// Failed to get TV adjust configuration
            /// 获取TV调节配置失败
            /// </summary>
            NET_ERROR_GETCFG_TVADJUST = 0x80000000 | 338,
            /// <summary>
            /// Failed to set TV adjust configuration
            /// 设置TV调节配置失败
            /// </summary>
            NET_ERROR_SETCFG_TVADJUST = 0x80000000 | 339,
            /// <summary>
            /// Failed to request to establish a connection
            /// 请求建立连接失败
            /// </summary>
            NET_ERROR_CONNECT_FAILED = 0x80000000 | 340,
            /// <summary>
            /// Failed to request to upload burn files
            /// 请求刻录文件上传失败
            /// </summary>
            NET_ERROR_SETCFG_BURNFILE = 0x80000000 | 341,
            /// <summary>
            /// Failed to get capture configuration information
            /// 获取抓包配置信息失败
            /// </summary>
            NET_ERROR_SNIFFER_GETCFG = 0x80000000 | 342,
            /// <summary>
            /// Failed to set capture configuration information
            /// 设置抓包配置信息失败
            /// </summary>
            NET_ERROR_SNIFFER_SETCFG = 0x80000000 | 343,
            /// <summary>
            /// Failed to get download restrictions information
            /// 查询下载限制信息失败
            /// </summary>
            NET_ERROR_DOWNLOADRATE_GETCFG = 0x80000000 | 344,
            /// <summary>
            /// Failed to set download restrictions information
            /// 设置下载限制信息失败
            /// </summary>
            NET_ERROR_DOWNLOADRATE_SETCFG = 0x80000000 | 345,
            /// <summary>
            /// Failed to query serial port parameters
            /// 查询串口参数失败
            /// </summary>
            NET_ERROR_SEARCH_TRANSCOM = 0x80000000 | 346,
            /// <summary>
            /// Failed to get the preset info
            /// 获取预制点信息错误
            /// </summary>
            NET_ERROR_GETCFG_POINT = 0x80000000 | 347,
            /// <summary>
            /// Failed to set the preset info
            /// 设置预制点信息错误
            /// </summary>
            NET_ERROR_SETCFG_POINT = 0x80000000 | 348,
            /// <summary>
            /// SDK log out the device abnormally
            /// SDK没有正常登出设备
            /// </summary>
            NET_SDK_LOGOUT_ERROR = 0x80000000 | 349,
            /// <summary>
            /// Failed to get vehicle configuration
            /// 获取车载配置失败
            /// </summary>
            NET_ERROR_GET_VEHICLE_CFG = 0x80000000 | 350,
            /// <summary>
            /// Failed to set vehicle configuration
            /// 设置车载配置失败
            /// </summary>
            NET_ERROR_SET_VEHICLE_CFG = 0x80000000 | 351,
            /// <summary>
            /// Failed to get ATM overlay configuration
            /// 获取atm叠加配置失败
            /// </summary>
            NET_ERROR_GET_ATM_OVERLAY_CFG = 0x80000000 | 352,
            /// <summary>
            /// Failed to set ATM overlay configuration
            /// 设置atm叠加配置失败
            /// </summary>
            NET_ERROR_SET_ATM_OVERLAY_CFG = 0x80000000 | 353,
            /// <summary>
            /// Failed to get ATM overlay ability
            /// 获取atm叠加能力失败
            /// </summary>
            NET_ERROR_GET_ATM_OVERLAY_ABILITY = 0x80000000 | 354,
            /// <summary>
            /// Failed to get decoder tour configuration
            /// 获取解码器解码轮巡配置失败
            /// </summary>
            NET_ERROR_GET_DECODER_TOUR_CFG = 0x80000000 | 355,
            /// <summary>
            /// Failed to set decoder tour configuration
            /// 设置解码器解码轮巡配置失败
            /// </summary>
            NET_ERROR_SET_DECODER_TOUR_CFG = 0x80000000 | 356,
            /// <summary>
            /// Failed to control decoder tour
            /// 控制解码器解码轮巡失败
            /// </summary>
            NET_ERROR_CTRL_DECODER_TOUR = 0x80000000 | 357,
            /// <summary>
            /// Beyond the device supports for the largest number of user groups
            /// 超出设备支持最大用户组数目
            /// </summary>
            NET_GROUP_OVERSUPPORTNUM = 0x80000000 | 358,
            /// <summary>
            /// Beyond the device supports for the largest number of users
            /// 超出设备支持最大用户数目
            /// </summary>
            NET_USER_OVERSUPPORTNUM = 0x80000000 | 359,
            /// <summary>
            /// Failed to get SIP configuration
            /// 获取SIP配置失败
            /// </summary>
            NET_ERROR_GET_SIP_CFG = 0x80000000 | 368,
            /// <summary>
            /// Failed to set SIP configuration
            /// 设置SIP配置失败
            /// </summary>
            NET_ERROR_SET_SIP_CFG = 0x80000000 | 369,
            /// <summary>
            /// Failed to get SIP capability
            /// 获取SIP能力失败
            /// </summary>
            NET_ERROR_GET_SIP_ABILITY = 0x80000000 | 370,
            /// <summary>
            /// Failed to get "WIFI ap' configuration
            /// 获取WIFI ap配置失败
            /// </summary>
            NET_ERROR_GET_WIFI_AP_CFG = 0x80000000 | 371,
            /// <summary>
            /// Failed to set "WIFI ap" configuration
            /// 设置WIFI ap配置失败
            /// </summary>
            NET_ERROR_SET_WIFI_AP_CFG = 0x80000000 | 372,
            /// <summary>
            /// Failed to get decode policy
            /// 获取解码策略配置失败
            /// </summary>
            NET_ERROR_GET_DECODE_POLICY = 0x80000000 | 373,
            /// <summary>
            /// Failed to set decode policy
            /// 设置解码策略配置失败
            /// </summary>
            NET_ERROR_SET_DECODE_POLICY = 0x80000000 | 374,
            /// <summary>
            /// refuse talk
            /// 拒绝对讲
            /// </summary>
            NET_ERROR_TALK_REJECT = 0x80000000 | 375,
            /// <summary>
            /// talk has opened by other client
            /// 对讲被其他客户端打开
            /// </summary>
            NET_ERROR_TALK_OPENED = 0x80000000 | 376,
            /// <summary>
            /// resource conflict
            /// 资源冲突
            /// </summary>
            NET_ERROR_TALK_RESOURCE_CONFLICIT = 0x80000000 | 377,
            /// <summary>
            /// unsupported encode type
            /// 不支持的语音编码格式
            /// </summary>
            NET_ERROR_TALK_UNSUPPORTED_ENCODE = 0x80000000 | 378,
            /// <summary>
            /// no right
            /// 无权限
            /// </summary>
            NET_ERROR_TALK_RIGHTLESS = 0x80000000 | 379,
            /// <summary>
            /// request failed
            /// 请求对讲失败
            /// </summary>
            NET_ERROR_TALK_FAILED = 0x80000000 | 380,
            /// <summary>
            /// Failed to get device relative config
            /// 获取机器相关配置失败
            /// </summary>
            NET_ERROR_GET_MACHINE_CFG = 0x80000000 | 381,
            /// <summary>
            /// Failed to set device relative config
            /// 设置机器相关配置失败
            /// </summary>
            NET_ERROR_SET_MACHINE_CFG = 0x80000000 | 382,
            /// <summary>
            /// get data failed
            /// 设备无法获取当前请求数据
            /// </summary>
            NET_ERROR_GET_DATA_FAILED = 0x80000000 | 383,
            /// <summary>
            /// MAC validate failed
            /// MAC地址验证失败 
            /// </summary>
            NET_ERROR_MAC_VALIDATE_FAILED = 0x80000000 | 384,
            /// <summary>
            /// Failed to get server instance 
            /// 获取服务器实例失败
            /// </summary>
            NET_ERROR_GET_INSTANCE = 0x80000000 | 385,
            /// <summary>
            /// Generated json string is error
            /// 生成的jason字符串错误
            /// </summary>
            NET_ERROR_JSON_REQUEST = 0x80000000 | 386,
            /// <summary>
            /// The responding json string is error
            /// 响应的jason字符串错误
            /// </summary>
            NET_ERROR_JSON_RESPONSE = 0x80000000 | 387,
            /// <summary>
            /// The protocol version is lower than current version
            /// 协议版本低于当前使用的版本
            /// </summary>
            NET_ERROR_VERSION_HIGHER = 0x80000000 | 388,
            /// <summary>
            /// Hotspare disk operation failed. The capacity is low
            /// 热备操作失败, 容量不足
            /// </summary>
            NET_SPARE_NO_CAPACITY = 0x80000000 | 389,
            /// <summary>
            /// Display source is used by other output
            /// 显示源被其他输出占用
            /// </summary>
            NET_ERROR_SOURCE_IN_USE = 0x80000000 | 390,
            /// <summary>
            /// advanced users grab low-level user resource
            /// 高级用户抢占低级用户资源
            /// </summary>
            NET_ERROR_REAVE = 0x80000000 | 391,
            /// <summary>
            /// net forbid
            /// 禁止入网
            /// </summary>
            NET_ERROR_NETFORBID = 0x80000000 | 392,
            /// <summary>
            /// get MAC filter configuration error
            /// 获取MAC过滤配置失败
            /// </summary>
            NET_ERROR_GETCFG_MACFILTER = 0x80000000 | 393,
            /// <summary>
            /// set MAC filter configuration error
            /// 设置MAC过滤配置失败
            /// </summary>
            NET_ERROR_SETCFG_MACFILTER = 0x80000000 | 394,
            /// <summary>
            /// get IP/MAC filter configuration error
            /// 获取IP/MAC过滤配置失败
            /// </summary>
            NET_ERROR_GETCFG_IPMACFILTER = 0x80000000 | 395,
            /// <summary>
            /// set IP/MAC filter configuration error
            /// 设置IP/MAC过滤配置失败
            /// </summary>
            NET_ERROR_SETCFG_IPMACFILTER = 0x80000000 | 396,
            /// <summary>
            /// operation over time 
            /// 当前操作超时
            /// </summary>
            NET_ERROR_OPERATION_OVERTIME = 0x80000000 | 397,
            /// <summary>
            /// senior validation failure
            /// 高级校验失败
            /// </summary>
            NET_ERROR_SENIOR_VALIDATE_FAILED = 0x80000000 | 398,
            /// <summary>
            /// device ID is not exist
            /// 设备ID不存在
            /// </summary>
            NET_ERROR_DEVICE_ID_NOT_EXIST = 0x80000000 | 399,
            /// <summary>
            /// unsupport operation
            /// 不支持当前操作
            /// </summary>
            NET_ERROR_UNSUPPORTED = 0x80000000 | 400,
            /// <summary>
            /// proxy dll load error
            /// 代理库加载失败
            /// </summary>
            NET_ERROR_PROXY_DLLLOAD = 0x80000000 | 401,
            /// <summary>
            /// proxy user parameter is not legal
            /// 代理用户参数不合法
            /// </summary>
            NET_ERROR_PROXY_ILLEGAL_PARAM = 0x80000000 | 402,
            /// <summary>
            /// handle invalid
            /// 代理句柄无效
            /// </summary>
            NET_ERROR_PROXY_INVALID_HANDLE = 0x80000000 | 403,
            /// <summary>
            /// login device error
            /// 代理登入前端设备失败
            /// </summary>
            NET_ERROR_PROXY_LOGIN_DEVICE_ERROR = 0x80000000 | 404,
            /// <summary>
            /// start proxy server error
            /// 启动代理服务失败
            /// </summary>
            NET_ERROR_PROXY_START_SERVER_ERROR = 0x80000000 | 405,
            /// <summary>
            /// request speak failed
            /// 请求喊话失败
            /// </summary>
            NET_ERROR_SPEAK_FAILED = 0x80000000 | 406,
            /// <summary>
            /// unsupport F6
            /// 设备不支持此F6接口调用
            /// </summary>
            NET_ERROR_NOT_SUPPORT_F6 = 0x80000000 | 407,
            /// <summary>
            /// CD is not ready
            /// 光盘未就绪
            /// </summary>
            NET_ERROR_CD_UNREADY = 0x80000000 | 408,
            /// <summary>
            /// Directory does not exist
            /// 目录不存在
            /// </summary>
            NET_ERROR_DIR_NOT_EXIST = 0x80000000 | 409,
            /// <summary>
            /// The device does not support the segmentation model
            /// 设备不支持的分割模式
            /// </summary>
            NET_ERROR_UNSUPPORTED_SPLIT_MODE = 0x80000000 | 410,
            /// <summary>
            /// Open the window parameter is illegal
            /// 开窗参数不合法
            /// </summary>
            NET_ERROR_OPEN_WND_PARAM = 0x80000000 | 411,
            /// <summary>
            /// Open the window more than limit
            /// 开窗数量超过限制
            /// </summary>
            NET_ERROR_LIMITED_WND_COUNT = 0x80000000 | 412,
            /// <summary>
            /// Request command with the current pattern don't match
            /// 请求命令与当前模式不匹配
            /// </summary>
            NET_ERROR_UNMATCHED_REQUEST = 0x80000000 | 413,
            /// <summary>
            /// Render Library to enable high-definition image internal adjustment strategy error
            /// Render库启用高清图像内部调整策略出错
            /// </summary>
            NET_RENDER_ENABLELARGEPICADJUSTMENT_ERROR = 0x80000000 | 414,
            /// <summary>
            /// Upgrade equipment failure
            /// 设备升级失败
            /// </summary>
            NET_ERROR_UPGRADE_FAILED = 0x80000000 | 415,
            /// <summary>
            /// Can't find the target device
            /// 找不到目标设备
            /// </summary>
            NET_ERROR_NO_TARGET_DEVICE = 0x80000000 | 416,
            /// <summary>
            /// Can't find the verify device
            /// 找不到验证设备
            /// </summary>
            NET_ERROR_NO_VERIFY_DEVICE = 0x80000000 | 417,
            /// <summary>
            /// No cascade permissions
            /// 无级联权限
            /// </summary>
            NET_ERROR_CASCADE_RIGHTLESS = 0x80000000 | 418,
            /// <summary>
            /// low priority
            /// 低优先级
            /// </summary>
            NET_ERROR_LOW_PRIORITY = 0x80000000 | 419,
            /// <summary>
            /// The remote device request timeout
            /// 远程设备请求超时
            /// </summary>
            NET_ERROR_REMOTE_REQUEST_TIMEOUT = 0x80000000 | 420,
            /// <summary>
            /// Input source beyond maximum route restrictions
            /// 输入源超出最大路数限制
            /// </summary>
            NET_ERROR_LIMITED_INPUT_SOURCE = 0x80000000 | 421,
            /// <summary>
            /// Failed to set log print
            /// 设置日志打印失败
            /// </summary>
            NET_ERROR_SET_LOG_PRINT_INFO = 0x80000000 | 422,
            /// <summary>
            /// "dwSize" is not initialized in input param
            /// 入参的dwsize字段出错
            /// </summary>
            NET_ERROR_PARAM_DWSIZE_ERROR = 0x80000000 | 423,
            /// <summary>
            /// TV wall exceed limit
            /// 电视墙数量超过上限
            /// </summary>
            NET_ERROR_LIMITED_MONITORWALL_COUNT = 0x80000000 | 424,
            /// <summary>
            /// Fail to execute part of the process
            /// 部分过程执行失败
            /// </summary>
            NET_ERROR_PART_PROCESS_FAILED = 0x80000000 | 425,
            /// <summary>
            /// Fail to transmit due to not supported by target
            /// 该功能不支持转发
            /// </summary>
            NET_ERROR_TARGET_NOT_SUPPORT = 0x80000000 | 426,
            /// <summary>
            /// Access to the file failed
            /// 访问文件失败
            /// </summary>
            NET_ERROR_VISITE_FILE = 0x80000000 | 510,
            /// <summary>
            /// Device busy
            /// 设备忙
            /// </summary>
            NET_ERROR_DEVICE_STATUS_BUSY = 0x80000000 | 511,
            /// <summary>
            /// Fail to change the password
            /// 修改密码无权限
            /// </summary>
            NET_USER_PWD_NOT_AUTHORIZED = 0x80000000 | 512,
            /// <summary>
            /// Password strength is not enough
            /// 密码强度不够
            /// </summary>
            NET_USER_PWD_NOT_STRONG = 0x80000000 | 513,
            /// <summary>
            /// No corresponding setup
            /// 没有对应的配置
            /// </summary>
            NET_ERROR_NO_SUCH_CONFIG = 0x80000000 | 514,
            /// <summary>
            /// Failed to record audio
            /// 录音失败
            /// </summary>
            NET_ERROR_AUDIO_RECORD_FAILED = 0x80000000 | 515,
            /// <summary>
            /// Failed to send out data 
            /// 数据发送失败
            /// </summary>
            NET_ERROR_SEND_DATA_FAILED = 0x80000000 | 516,
            /// <summary>
            /// Abandoned port 
            /// 废弃接口
            /// </summary>
            NET_ERROR_OBSOLESCENT_INTERFACE = 0x80000000 | 517,
            /// <summary>
            /// Internal buffer is not sufficient 
            /// 内部缓冲不足
            /// </summary>
            NET_ERROR_INSUFFICIENT_INTERAL_BUF = 0x80000000 | 518,
            /// <summary>
            /// verify password when changing device IP
            /// 修改设备ip时,需要校验密码
            /// </summary>
            NET_ERROR_NEED_ENCRYPTION_PASSWORD = 0x80000000 | 519,
            /// <summary>
            /// device not support the record
            /// 设备不支持此记录集
            /// </summary>
            NET_ERROR_NOSUPPORT_RECORD = 0x80000000 | 520,
            /// <summary>
            /// Failed to serialize data
            /// 数据序列化错误
            /// </summary>
            NET_ERROR_SERIALIZE_ERROR = 0x80000000 | 1010,
            /// <summary>
            /// Failed to deserialize data
            /// 数据序列化错误
            /// </summary>
            NET_ERROR_DESERIALIZE_ERROR = 0x80000000 | 1011,
            /// <summary>
            /// the wireless id is already existed
            /// 数据反序列化错误
            /// </summary>
            NET_ERROR_LOWRATEWPAN_ID_EXISTED = 0x80000000 | 1012,
            /// <summary>
            /// the wireless id limited
            /// 该无线ID已存在
            /// </summary>
            NET_ERROR_LOWRATEWPAN_ID_LIMIT = 0x80000000 | 1013,
            /// <summary>
            /// add the wireless id abnormaly
            /// 无线ID数量已超限
            /// </summary>
            NET_ERROR_LOWRATEWPAN_ID_ABNORMAL = 0x80000000 | 1014,
            /// <summary>
            /// encrypt data fail
            /// 加密数据失败
            /// </summary>
            NET_ERROR_ENCRYPT = 0x80000000 | 1015,
            /// <summary>
            /// new password illegal
            /// 新密码不合规范
            /// </summary>
            NET_ERROR_PWD_ILLEGAL = 0x80000000 | 1016,
            /// <summary>
            /// device is already init
            /// 设备已经初始化
            /// </summary>
            NET_ERROR_DEVICE_ALREADY_INIT = 0x80000000 | 1017,
            /// <summary>
            /// security code check out fail
            /// 安全码错误
            /// </summary>
            NET_ERROR_SECURITY_CODE = 0x80000000 | 1018,
            /// <summary>
            /// security code out of time
            /// 安全码超出有效期
            /// </summary>
            NET_ERROR_SECURITY_CODE_TIMEOUT = 0x80000000 | 1019,
            /// <summary>
            /// get passwd specification fail
            /// 获取密码规范失败
            /// </summary>
            NET_ERROR_GET_PWD_SPECI = 0x80000000 | 1020,
            /// <summary>
            /// no authority of operation 
            /// 无权限进行该操作
            /// </summary>
            NET_ERROR_NO_AUTHORITY_OF_OPERATION = 0x80000000 | 1021,
            /// <summary>
            /// decrypt data fail
            /// 解密数据失败
            /// </summary>
            NET_ERROR_DECRYPT = 0x80000000 | 1022,
            /// <summary>
            /// 2D code check out fail
            /// 2D code校验失败
            /// </summary>
            NET_ERROR_2D_CODE = 0x80000000 | 1023,
            /// <summary>
            /// invalid request
            /// 非法的RPC请求
            /// </summary>
            NET_ERROR_INVALID_REQUEST = 0x80000000 | 1024,
            /// <summary>
            /// pwd reset disabled
            /// 密码重置功能已关闭
            /// </summary>
            NET_ERROR_PWD_RESET_DISABLE = 0x80000000 | 1025,
            /// <summary>
            /// failed to display private data,such as rule box
            /// 显示私有数据，比如规则框等失败
            /// </summary>
            NET_ERROR_PLAY_PRIVATE_DATA = 0x80000000 | 1026,
            /// <summary>
            /// robot operate failed
            /// 机器人操作失败
            /// </summary>
            NET_ERROR_ROBOT_OPERATE_FAILED = 0x80000000 | 1027,
            /// <summary>
            /// channel has already been opened
            /// 通道已经打开
            /// </summary>
            NET_ERROR_CHANNEL_ALREADY_OPENED = 0x80000000 | 1033,
            /// <summary>
            /// 组ID超过最大值
            /// </summary>
            NET_ERROR_FACE_RECOGNITION_SERVER_GROUP_ID_EXCEED = 0x80000000 | 1051,
            /// <summary>
            /// 设备不支持高安全等级登录
            /// </summary>
            ERR_NOT_SUPPORT_HIGHLEVEL_SECURITY_LOGIN = 0x80000000 | 1153,
            /// <summary>
            /// invaild channel
            /// 无效的通道
            /// </summary>
            ERR_INTERNAL_INVALID_CHANNEL = 0x90001002,
            /// <summary>
            /// reopen channel failed
            /// 重新打开通道失败
            /// </summary>
            ERR_INTERNAL_REOPEN_CHANNEL = 0x90001003,
            /// <summary>
            /// send data failed
            /// 发送数据失败
            /// </summary>
            ERR_INTERNAL_SEND_DATA = 0x90002008,
            /// <summary>
            /// creat socket failed
            /// 创建套接字失败
            /// </summary>
            ERR_INTERNAL_CREATE_SOCKET = 0x90002003,
            ERR_INTERNAL_LISTEN_FAILED = 0x90010010,
        }

        /// <summary>
        /// intelligent event type,used in RealLoadPicture or fAnalyzerDataCallBack
        /// 智能事件类型
        /// </summary>
        public enum EM_EVENT_IVS_TYPE
        {
            /// <summary>
            /// subscription all event
            /// 订阅所有事件
            /// </summary>
            ALL = 0x00000001,
            /// <summary>
            /// face detection(Corresponding to NET_DEV_EVENT_FACEDETECT_INFO)
            /// 人脸检测事件 (对应 NET_DEV_EVENT_FACEDETECT_INFO)
            /// </summary>
            FACEDETECT = 0x0000001A
        }

        /// <summary>
        /// the describe of EVENT_IVS_FACEDETECT's data
        /// 人脸检测事件对应的数据块描述信息
        /// </summary>
        public struct NET_DEV_EVENT_FACEDETECT_INFO
        {
            /// <summary>
            /// channel ID
            /// 通道号
            /// </summary>
            public int nChannelID;
            /// <summary>
            /// event name
            /// 事件名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szName;
            /// <summary>
            /// byte alignment
            /// 字节对齐
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] bReserved1;
            /// <summary>
            /// PTS(ms)
            /// 时间戳(单位是毫秒)
            /// </summary>
            public double PTS;
            /// <summary>
            /// the event happen time
            /// 事件发生的时间
            /// </summary>
            public NET_TIME_EX UTC;
            /// <summary>
            /// event ID
            /// 事件ID
            /// </summary>
            public int nEventID;
            /// <summary>
            /// have being detected object
            /// 检测到的物体
            /// </summary>
            public NET_MSG_OBJECT stuObject;
            /// <summary>
            /// event file info
            /// 事件对应文件信息
            /// </summary>
            public NET_EVENT_FILE_INFO stuFileInfo;
            /// <summary>
            /// Event action: 0 means pulse event,1 means continuous event's begin,2means continuous event's end
            /// 事件动作,0表示脉冲事件,1表示持续性事件开始,2表示持续性事件结束
            /// </summary>
            public byte bEventAction;
            /// <summary>
            /// reserved
            /// 保留字节
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] reserved;
            /// <summary>
            /// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
            /// 图片的序号, 同一时间内(精确到秒)可能有多张图片, 从0开始
            /// </summary>
            public byte byImageIndex;
            /// <summary>
            /// detect region point number
            /// 规则检测区域顶点数
            /// </summary>
            public int nDetectRegionNum;
            /// <summary>
            /// detect region
            /// 规则检测区域
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public NET_POINT[] DetectRegion;
            /// <summary>
            /// flag(by bit),see NET_RESERVED_COMMON
            /// 抓图标志(按位),具体见NET_RESERVED_COMMON  
            /// </summary>
            public uint dwSnapFlagMask;
            /// <summary>
            /// snapshot current face device address
            /// 抓拍当前人脸的设备地址
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szSnapDevAddress;
            /// <summary>
            /// event trigger accumilated times 
            /// 事件触发累计次数
            /// </summary>
            public uint nOccurrenceCount;
            /// <summary>
            /// sex type
            /// 性别
            /// </summary>
            public EM_DEV_EVENT_FACEDETECT_SEX_TYPE emSex;
            /// <summary>
            /// age, invalid if it is -1
            /// 年龄,-1表示该字段数据无效
            /// </summary>
            public int nAge;
            /// <summary>
            /// invalid number in array emFeature
            /// 人脸特征数组有效个数,与 emFeature 结合使用
            /// </summary>
            public uint nFeatureValidNum;
            /// <summary>
            /// human face features
            /// 人脸特征数组,与 nFeatureValidNum 结合使用
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public EM_DEV_EVENT_FACEDETECT_FEATURE_TYPE[] emFeature;
            /// <summary>
            /// number of stuFaces
            /// 指示stuFaces有效数量
            /// </summary>
            public int nFacesNum;
            /// <summary>
            /// when nFacesNum > 0, stuObject invalid
            /// 多张人脸时使用,此时没有Object
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public NET_FACE_INFO[] stuFaces;
            /// <summary>
            /// public info 
            /// 智能事件公共信息
            /// </summary>
            public NET_EVENT_INTELLI_COMM_INFO stuIntelliCommInfo;
            public EM_RACE_TYPE emRace;
            /// <summary>
            /// eyes state
            /// 眼睛状态
            /// </summary>
            public EM_EYE_STATE_TYPE emEye;
            /// <summary>
            /// mouth state
            /// 嘴巴状态
            /// </summary>
            public EM_MOUTH_STATE_TYPE emMouth;
            /// <summary>
            /// mask state
            /// 口罩状态
            /// </summary>
            public EM_MASK_STATE_TYPE emMask;
            /// <summary>
            /// beard state
            /// 胡子状态
            /// </summary>
            public EM_BEARD_STATE_TYPE emBeard;
            /// <summary>
            /// Attractive value, -1: invalid, 0:no disringuish,range: 1-100, the higher value, the higher charm
            /// 魅力值, -1表示无效, 0未识别，识别时范围1-100,得分高魅力高
            /// </summary>
            public int nAttractive;
            /// <summary>
            /// Unique identifier of the captured person
            /// 抓拍人员写入数据库的唯一标识符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szUID;
            /// <summary>
            /// byte alignment
            /// 保留字节
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] bReserved2;
            /// <summary>
            /// Eigenvalue info
            /// 特征值信息
            /// </summary>
            public NET_FEATURE_VECTOR stuFeatureVector;
            /// <summary>
            /// Eigenvalue algorithm version
            /// 特征值算法版本
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFeatureVersion;
            /// <summary>
            /// The state of human face in camera
            /// 0-unknown,1-appear,2-in picture 3-left
            /// 人脸在摄像机画面中的状态
            /// </summary>
            public EM_FACE_DETECT_STATUS emFaceDetectStatus;
            /// <summary>
            /// The angle information of human face in the captured picture
            /// 人脸在抓拍图片中的角度信息
            /// nPitch:Pitch angle(抬头低头的俯仰角), 
            /// nYaw:Yaw angle(左右转头的偏航角), 
            /// nRoll:Roll angle(头在平面内左偏右偏的翻滚角)
            //  Angle value range(角度值取值范围)[-90,90], 
            /// Three angle values are 999, indicating that the angle information is invalid
            /// 三个角度值都为999表示角度信息无效
            /// </summary>
            public NET_EULER_ANGLE stuFaceCaptureAngle;
            /// <summary>
            /// Face capture quality score, range 0-10000
            /// 人脸抓拍质量分数,范围 0~10000 
            /// </summary>
            public uint nFaceQuality;
            /// <summary>
            /// Human speed, km/h
            /// 人的运动速度, km/h
            /// </summary>
            public double dHumanSpeed;
            /// <summary>
            /// Face alignment score, range 0-10000, -1 means invalid
            /// 人脸对齐得分分数,范围 0~10000,-1为无效值
            /// </summary>
            public int nFaceAlignScore;
            /// <summary>
            /// Face clarity score, range 0-10000, -1 means invalid
            /// 人脸清晰度分数,范围 0~10000,-1为无效值
            /// </summary>
            public int nFaceClarity;
            /// <summary>
            /// Whether the information of human temperature is valid
            /// 人体温信息是否有效
            /// </summary>
            public bool bHumanTemperature;
            /// <summary>
            /// Human body temperature information
            /// 人体温信息
            /// </summary>
            public NET_HUMAN_TEMPERATURE_INFO stuHumanTemperature;
            /// <summary>
            /// 国标编码
            /// National Standard Code
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szCameraID;
            /// <summary>
            /// Reserved
            /// 保留字节
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 416)]
            public byte[] bReserved;
        }

        public struct NET_MSG_OBJECT
        {
            /// <summary>
            /// Object ID,each ID represent a unique object
            /// 物体ID,每个ID表示一个唯一的物体
            /// </summary>
            public int nObjectID;
            /// <summary>
            /// Object type
            /// 物体类型
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] szObjectType;
            /// <summary>
            /// Confidence(0~255),a high value indicate a high confidence
            /// 置信度(0~255),值越大表示置信度越高
            /// </summary>
            public int nConfidence;
            /// <summary>
            /// Object action:1:Appear 2:Move 3:Stay 4:Remove 5:Disappear 6:Split 7:Merge 8:Rename
            /// 物体动作:1:Appear 2:Move 3:Stay 4:Remove 5:Disappear 6:Split 7:Merge 8:Rename
            /// </summary>
            public int nAction;
            /// <summary>
            /// BoundingBox
            /// 包围盒
            /// </summary>
#if (LINUX_X64)
		public NET_RECT_LONG_TYPE                   BoundingBox;		     
#else
            public NET_RECT BoundingBox;
#endif
            /// <summary>
            /// The shape center of the object
            /// 物体型心
            /// </summary>
            public NET_POINT Center;
            /// <summary>
            /// the number of culminations for the polygon
            /// 多边形顶点个数
            /// </summary>
            public int nPolygonNum;
            /// <summary>
            /// a polygon that have a exactitude figure
            /// 较精确的轮廓多边形
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public NET_POINT[] Contour;
            /// <summary>
            /// The main color of the object;the first byte indicate red value, as byte order as green, blue, transparence, for example:RGB(0,255,0),transparence = 0, rgbaMainColor = 0x00ff0000.
            /// 表示车牌、车身等物体主要颜色；按字节表示,分别为红、绿、蓝和透明度,例如:RGB值为(0,255,0),透明度为0时, 其值为0x00ff0000.
            /// </summary>
            public uint rgbaMainColor;
            /// <summary>
            /// the interrelated text of object,such as number plate,container number
            /// 物体上相关的带0结束符文本,比如车牌,集装箱号等等
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] szText;
            /// <summary>
            /// object sub type,different object type has different sub type:Vehicle Category:"Unknown","Motor","Non-Motor","Bus","Bicycle","Motorcycle";Plate Category:"Unknown","mal","Yellow","DoubleYellow","Police","Armed"
            /// 物体子类别,根据不同的物体类型,可以取以下子类型:Vehicle Category:"Unknown"  未知,"Motor" 机动车,"Non-Motor":非机动车,"Bus": 公交车;Plate Category："Unknown" 未知,"Normal" 蓝牌黑牌,"Yellow" 黄牌,"DoubleYellow" 双层黄尾牌,"Police" 警牌"Armed" 武警牌
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] szObjectSubType;
            /// <summary>
            /// Specifies the sub-brand of vehicle,the real value can be found in a mapping table from the development manual
            /// 车辆子品牌 需要通过映射表得到真正的子品牌 映射表详见开发手册
            /// </summary>
            public ushort wSubBrand;
            /// <summary>
            /// reserved
            /// 保留，字节对齐
            /// </summary>
            public byte byReserved1;
            /// <summary>
            /// picture info enable
            /// 是否有物体对应图片文件信息
            /// </summary>
            public byte bPicEnble;
            /// <summary>
            /// picture info
            /// 物体对应图片信息
            /// </summary>
            public NET_PIC_INFO stPicInfo;
            /// <summary>
            /// is shot frame
            /// 是否是抓拍张的识别结果
            /// </summary>
            public byte bShotFrame;
            /// <summary>
            /// rgbaMainColor is enable
            /// 物体颜色(rgbaMainColor)是否可用
            /// </summary>
            public byte bColor;
            /// <summary>
            /// Reserved
            /// 保留，字节对齐
            /// </summary>
            public byte byReserved2;
            /// <summary>
            /// Time indicates the type of detailed instructions,EM_TIME_TYP
            /// 时间表示类型,详见EM_TIME_TYPE说明
            /// </summary>
            public byte byTimeType;
            /// <summary>
            /// in view of the video compression,current time(when object snap or reconfnition, the frame will be attached to the frame in a video or pictures,means the frame in the original video of the time)
            /// 针对视频浓缩,当前时间戳（物体抓拍或识别时,会将此识别智能帧附在一个视频帧或jpeg图片中,此帧所在原始视频中的出现时间）
            /// </summary>
            public NET_TIME_EX stuCurrentTime;
            /// <summary>
            /// strart time(object appearing for the first time)
            /// 开始时间戳（物体开始出现时）
            /// </summary>
            public NET_TIME_EX stuStartTime;
            /// <summary>
            /// end time(object appearing for the last time) 
            /// 结束时间戳（物体最后出现时）
            /// </summary>
            public NET_TIME_EX stuEndTime;
#if (LINUX_X64)
		public NET_RECT_LONG_TYPE                   stuOriginalBoundingBox;	
        public NET_RECT_LONG_TYPE                   stuSignBoundingBox;	    
#else
            /// <summary>
            /// original bounding box(absolute coordinates)
            /// 包围盒(绝对坐标)
            /// </summary>
            public NET_RECT stuOriginalBoundingBox;
            /// <summary>
            /// sign bounding box coordinate
            /// 车标坐标包围盒
            /// </summary>
            public NET_RECT stuSignBoundingBox;
#endif
            /// <summary>
            /// The current frame number (frames when grabbing the object)
            /// 当前帧序号（抓下这个物体时的帧）
            /// </summary>
            public uint dwCurrentSequence;
            /// <summary>
            /// Start frame number (object appeared When the frame number
            /// 开始帧序号（物体开始出现时的帧序号）
            /// </summary>
            public uint dwBeginSequence;
            /// <summary>
            /// The end of the frame number (when the object disappearing Frame number)
            /// 结束帧序号（物体消逝时的帧序号）
            /// </summary>
            public uint dwEndSequence;
            /// <summary>
            /// At the beginning of the file offset, Unit: Word Section (when objects began to appear, the video frames in the original video file offset relative to the beginning of the file
            /// 开始时文件偏移, 单位: 字节（物体开始出现时,视频帧在原始视频文件中相对于文件起始处的偏移）
            /// </summary>
            public Int64 nBeginFileOffset;
            /// <summary>
            /// At the end of the file offset, Unit: Word Section (when the object disappeared, video frames in the original video file offset relative to the beginning of the file)
            /// 结束时文件偏移, 单位: 字节（物体消逝时,视频帧在原始视频文件中相对于文件起始处的偏移）
            /// </summary>
            public Int64 nEndFileOffset;
            /// <summary>
            /// Object color similarity, the range :0-100, represents an array subscript Colors, see EM_COLOR_TYPE
            /// 物体颜色相似度,取值范围：0-100,数组下标值代表某种颜色,详见EM_COLOR_TYPE
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byColorSimilar;
            /// <summary>
            /// When upper body color similarity (valid object type man )
            /// 上半身物体颜色相似度(物体类型为人时有效)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byUpperBodyColorSimilar;
            /// <summary>
            /// Lower body color similarity when objects (object type human valid )
            /// 下半身物体颜色相似度(物体类型为人时有效)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] byLowerBodyColorSimilar;
            /// <summary>
            /// ID of relative object
            /// 相关物体ID
            /// </summary>
            public int nRelativeID;
            /// <summary>
            /// "ObjectType"is "Vehicle" or "Logo" means a certain brand under LOGO such as Audi A6L since there are so many brands SDK sends this field in real-time ,device filled as real.
            /// "ObjectType"为"Vehicle"或者"Logo"时,表示车标下的某一车系,比如奥迪A6L,由于车系较多,SDK实现时透传此字段,设备如实填写
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] szSubText;
            /// <summary>
            /// Specifies the model years of vehicle. the real value can be found in a mapping table from the development manual 
            /// 车辆品牌年款 需要通过映射表得到真正的年款 映射表详见开发手册
            /// </summary>
            public ushort wBrandYear;
        }

        /// <summary>
        /// rect size struct
        /// 矩形大小
        /// </summary>
        public struct NET_RECT
        {
            /// <summary>
            /// left
            /// 左
            /// </summary>
            public int nLeft;
            /// <summary>
            /// top
            /// 上
            /// </summary>
            public int nTop;
            /// <summary>
            /// right
            /// 右
            /// </summary>
            public int nRight;
            /// <summary>
            /// bottom
            /// 下
            /// </summary>
            public int nBottom;
        }

        /// <summary>
        /// picture information struct
        /// 图片信息
        /// </summary>
        public struct NET_PIC_INFO
        {
            /// <summary>
            /// current picture file's offset in the binary file, byte
            /// 文件在二进制数据块中的偏移位置, 单位:字节
            /// </summary>
            public uint dwOffSet;
            /// <summary>
            /// current picture file's size, byte
            /// 文件大小, 单位:字节
            /// </summary>
            public uint dwFileLenth;
            /// <summary>
            /// picture width, pixel
            /// 图片宽度, 单位:像素
            /// </summary>
            public ushort wWidth;
            /// <summary>
            /// picture high, pixel
            /// 图片高度, 单位:像素
            /// </summary>
            public ushort wHeight;
            /// <summary>
            /// File path,User use this field need to apply for space for copy and storage
            /// 文件路径，用户使用该字段时需要自行申请空间进行拷贝保存
            /// </summary>
            public IntPtr pszFilePath;
            /// <summary>
            /// When submit to the server, the algorithm has checked the image or not 
            /// 图片是否算法检测出来的检测过的提交识别服务器时
            /// </summary>
            public byte bIsDetected;
            /// <summary>
            /// Reserved
            /// 保留
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            /// <summary>
            /// pszFilePath length
            /// </summary>
            public int nFilePathLen;
            /// <summary>
            /// The upper left corner of the figure is in the big picture. Absolute coordinates are used
            /// 小图左上角在大图的位置，使用绝对坐标系
            /// </summary>
            public NET_POINT stuPoint;
        }

        /// <summary>
        /// dimension point struct
        /// 坐标点
        /// </summary>
        public struct NET_POINT
        {
            /// <summary>
            /// x
            /// 坐标x
            /// </summary>
            public short nx;
            /// <summary>
            /// y
            /// 坐标y
            /// </summary>
            public short ny;
        }

        // 人体温信息
        public struct NET_HUMAN_TEMPERATURE_INFO
        {
            public double dbTemperature;                          // 温度
            public EM_HUMAN_TEMPERATURE_UNIT emTemperatureUnit;   // 温度单位
            public bool bIsOverTemp;                              // 是否超温
            public bool bIsUnderTemp;                             // 是否低温
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
            public byte[] bReserved;                              // 预留字段
        }

        // 人体测温温度单位
        public enum EM_HUMAN_TEMPERATURE_UNIT
        {
            UNKNOWN = -1,   // 未知
            CENTIGRADE,            // 摄氏度
            FAHRENHEIT,            // 华氏度
            KELVIN,                // 开尔文
        }

        // 姿态角数据
        public struct NET_EULER_ANGLE
        {
            public int nPitch;             // 仰俯角
            public int nYaw;               // 偏航角
            public int nRoll;              // 翻滚角
        }

        // 人脸在摄像机画面中的状态
        public enum EM_FACE_DETECT_STATUS
        {
            UNKNOWN,             // 未知
            APPEAR,              // 出现
            INPICTURE,           // 在画面中
            EXIT,                // 离开
        }

        // 存储IVSS项目招行VIP需求,特征值信息
        public struct NET_FEATURE_VECTOR
        {
            public uint dwOffset;                         // 人脸小图特征值在二进制数据块中的偏移
            public uint dwLength;                         // 人脸小图特征值长度，单位:字节
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
            public byte[] byReserved;// 保留字节
        }

        /// <summary>
        /// eyes state
        /// 眼睛状态
        /// </summary>
        public enum EM_EYE_STATE_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// no disringuish
            /// 未识别
            /// </summary>
            NODISTI,
            /// <summary>
            /// close eyes
            /// 闭眼
            /// </summary>
            CLOSE,
            /// <summary>
            ///open eyes 
            /// 睁眼
            /// </summary>
            OPEN,
        }

        /// <summary>
        /// mouth state
        /// 嘴巴状态
        /// </summary>
        public enum EM_MOUTH_STATE_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// no disringuish
            /// 未识别
            /// </summary>
            NODISTI,
            /// <summary>
            /// close mouth
            /// 闭嘴
            /// </summary>
            CLOSE,
            /// <summary>
            /// open nouth
            /// 张嘴
            /// </summary>
            OPEN,
        }

        /// <summary>
        /// mask state
        /// 口罩状态
        /// </summary>
        public enum EM_MASK_STATE_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// no disringuish
            /// 未识别
            /// </summary>
            NODISTI,
            /// <summary>
            /// no mask
            /// 没戴口罩
            /// </summary>
            NOMASK,
            /// <summary>
            /// wearing mask
            /// 戴口罩
            /// </summary>
            WEAR,
        }

        /// <summary>
        /// beard state
        /// 胡子状态
        /// </summary>
        public enum EM_BEARD_STATE_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// no disringuish
            /// 未识别
            /// </summary>
            NODISTI,
            /// <summary>
            /// no beard
            /// 没胡子
            /// </summary>
            NOBEARD,
            /// <summary>
            /// have beard
            /// 有胡子
            /// </summary>
            HAVEBEARD,
        }

        /// <summary>
        /// type
        /// 肤色类型
        /// </summary>
        public enum EM_RACE_TYPE
        {
            UNKNOWN,
            NODISTI,
            YELLOW,
            BLACK,
            WHITE,
        }

        /// <summary>
        /// intelli event comm info
        /// 智能报警事件公共信息
        /// </summary>
        public struct NET_EVENT_INTELLI_COMM_INFO
        {
            /// <summary>
            /// class type
            /// 智能事件所属大类
            /// </summary>
            public EM_CLASS_TYPE emClassType;
            /// <summary>
            /// Preset ID
            /// 该事件触发的预置点，对应该设置规则的预置点
            /// </summary>
            public int nPresetID;
            /// <summary>
            /// reserved
            /// 保留
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 124)]
            public byte[] bReserved;
        }

        /// <summary>
        /// class type
        /// 类类型
        /// </summary>
        public enum EM_CLASS_TYPE
        {
            /// <summary>
            /// unknow
            /// 未知业务
            /// </summary>
            UNKNOWN = 0,
            /// <summary>
            /// video-synopsis
            /// 视频浓缩
            /// </summary>
            VIDEO_SYNOPSIS = 1,
            /// <summary>
            /// traffiv-gate
            /// 卡口
            /// </summary>
            TRAFFIV_GATE = 2,
            /// <summary>
            /// electronic-police
            /// 电警
            /// </summary>
            ELECTRONIC_POLICE = 3,
            /// <summary>
            /// single-PTZ-parking
            /// 单球违停
            /// </summary>
            SINGLE_PTZ_PARKING = 4,
            /// <summary>
            /// PTZ-parking
            /// 主从违停
            /// </summary>
            PTZ_PARKINBG = 5,
            /// <summary>
            /// Traffic
            /// 交通事件
            /// </summary>
            TRAFFIC = 6,
            /// <summary>
            /// Normal
            /// 通用行为分析
            /// </summary>
            NORMAL = 7,
            /// <summary>
            /// Prison
            /// 监所行为分析
            /// </summary>
            PRISON = 8,
            /// <summary>
            /// ATM
            /// 金融行为分析
            /// </summary>
            ATM = 9,
            /// <summary>
            /// metro
            /// 地铁行为分析
            /// </summary>
            METRO = 10,
            /// <summary>
            /// FaceDetection
            /// 人脸检测
            /// </summary>
            FACE_DETECTION = 11,
            /// <summary>
            /// FaceRecognition
            /// 人脸识别
            /// </summary>
            FACE_RECOGNITION = 12,
            /// <summary>
            /// NumberStat
            /// 人数统计
            /// </summary>
            NUMBER_STAT = 13,
            /// <summary>
            /// HeatMap
            /// 热度图
            /// </summary>
            HEAT_MAP = 14,
            /// <summary>
            /// VideoDiagnosis
            /// 视频诊断
            /// </summary>
            VIDEO_DIAGNOSIS = 15,
            /// <summary>
            /// VideoEnhance
            /// 视频增强
            /// </summary>
            VIDEO_ENHANCE = 16,
            /// <summary>
            /// Smokefire detect 
            /// 烟火检测
            /// </summary>
            SMOKEFIRE_DETECT = 17,
            /// <summary>
            /// VehicleAnalyse
            /// 车辆特征识别
            /// </summary>
            VEHICLE_ANALYSE = 18,
            /// <summary>
            /// Person feature
            /// 人员特征识别
            /// </summary>
            PERSON_FEATURE = 19,
            /// <summary>
            /// SDFaceDetect
            /// 多预置点人脸检测"SDFaceDetect",配置一条规则但可以在不同预置点下生效
            /// </summary>
            SDFACEDETECTION = 20,
            /// <summary>
            /// HeatMapPlan
            /// 球机热度图计划"HeatMapPlan" 
            /// </summary>
            HEAT_MAP_PLAN = 21,
            /// <summary>
            /// NumberStatPlan
            /// 球机客流量统计计划 "NumberStatPlan"
            /// </summary>
            NUMBERSTAT_PLAN = 22,
            /// <summary>
            /// ATMFD
            /// 金融人脸检测，包括正常人脸、异常人脸、相邻人脸、头盔人脸等针对ATM场景特殊优化
            /// </summary>
            ATMFD = 23,
            /// <summary>
            /// Highway
            /// 高速交通事件检测"Highway"
            /// </summary>
            HIGHWAY = 24,
            /// <summary>
            /// City
            /// 城市交通事件检测 "City"
            /// </summary>
            CITY = 25,
            /// <summary>
            /// LeTrack
            /// 民用简易跟踪"LeTrack"
            /// </summary>
            LETRACK = 26,
            /// <summary>
            /// SCR
            /// 打靶相机"SCR"
            /// </summary>
            SCR = 27,
            /// <summary>
            /// StereoVision
            /// 立体视觉(双目)"StereoVision"
            /// </summary>
            STEREO_VISION = 28,
            /// <summary>
            /// HumanDetect
            /// 人体检测"HumanDetect"
            /// </summary>
            HUMANDETECT = 29,
            /// <summary>
            /// FaceAnalysis
            /// 人脸分析 "FaceAnalysis"
            /// </summary>
            FACE_ANALYSIS = 30,
            /// <summary>
            /// XRayDetection
            /// X光检测"XRayDetection"
            /// </summary>
            XRAY_DETECTION = 31,
        }

        /// <summary>
        /// multi faces detect info
        /// 多人脸检测信息
        /// </summary>
        public struct NET_FACE_INFO
        {
            /// <summary>
            /// object id
            /// 物体ID,每个ID表示一个唯一的物体
            /// </summary>
            public int nObjectID;
            /// <summary>
            /// object type
            /// 物体类型
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szObjectType;
            /// <summary>
            /// same with the source picture id
            /// 这张人脸抠图所属的大图的ID
            /// </summary>
            public int nRelativeID;
            /// <summary>
            /// bounding box
            /// 包围盒
            /// </summary>
            public NET_RECT BoundingBox;
            /// <summary>
            /// object center
            /// 物体型心
            /// </summary>
            public NET_POINT Center;
        }

        /// <summary>
        /// feature type of detected human face
        /// 人脸检测对应人脸特征类型
        /// </summary>
        public enum EM_DEV_EVENT_FACEDETECT_FEATURE_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// wearing glasses
            /// 戴眼镜
            /// </summary>
            WEAR_GLASSES,
            /// <summary>
            /// smile
            /// 微笑
            /// </summary>
            SMILE,
            /// <summary>
            /// anger
            /// 愤怒
            /// </summary>
            ANGER,
            /// <summary>
            /// sadness
            /// 悲伤
            /// </summary>
            SADNESS,
            /// <summary>
            /// disgust
            /// 厌恶
            /// </summary>
            DISGUST,
            /// <summary>
            /// fear
            /// 害怕
            /// </summary>
            FEAR,
            /// <summary>
            /// surprise
            /// 惊讶
            /// </summary>
            SURPRISE,
            /// <summary>
            /// neutral
            /// 正常
            /// </summary>
            NEUTRAL,
            /// <summary>
            /// laugh
            /// 大笑
            /// </summary>
            LAUGH,
            /// <summary>
            /// not wear glasses
            /// 没戴眼镜
            /// </summary>
            NOGLASSES,
            /// <summary>
            /// happy
            /// 高兴
            /// </summary>
            HAPPY,
            /// <summary>
            /// confused
            /// 困惑
            /// </summary>
            CONFUSED,
            /// <summary>
            /// scream
            /// 尖叫
            /// </summary>
            SCREAM,
            /// <summary>
            /// wearing sun glasses
            /// 戴太阳眼镜
            /// </summary>
            WEAR_SUNGLASSES,
        }

        /// <summary>
        /// sex type of dectected human face
        /// 人脸检测对应性别类型
        /// </summary>
        public enum EM_DEV_EVENT_FACEDETECT_SEX_TYPE
        {
            /// <summary>
            /// unknown
            /// 未知
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// male
            /// 男性
            /// </summary>
            MAN,
            /// <summary>
            /// female
            /// 女性
            /// </summary>
            WOMAN,
        }

        /// <summary>
        /// event file information struct
        /// 事件对应文件信息
        /// </summary>
        public struct NET_EVENT_FILE_INFO
        {
            /// <summary>
            /// the file count in the current file's group
            /// 当前文件所在文件组中的文件总数
            /// </summary>
            public byte bCount;
            /// <summary>
            /// the index of the file in the group
            /// 当前文件在文件组中的文件编号(编号1开始)
            /// </summary>
            public byte bIndex;
            /// <summary>
            /// file tag, see the enum EM_EVENT_FILETAG
            /// 文件标签, EM_EVENT_FILETAG
            /// </summary>
            public byte bFileTag;
            /// <summary>
            /// file type,0-normal 1-compose 2-cut picture
            /// 文件类型,0-普通 1-合成 2-抠图
            /// </summary>
            public byte bFileType;
            /// <summary>
            /// file time
            /// 文件时间
            /// </summary>
            public NET_TIME_EX stuFileTime;
            /// <summary>
            /// the only id of one group file
            /// 同一组抓拍文件的唯一标识
            /// </summary>
            public uint nGroupId;
        }

        /// <summary>
        /// login device mode enumeration
        /// 登陆设备方式枚举
        /// </summary>
        public enum EM_LOGIN_SPAC_CAP_TYPE
        {
            /// <summary>
            /// TCP login, default
            /// TCP登陆, 默认方式
            /// </summary>
            TCP = 0,
            /// <summary>
            /// No criteria login
            /// 无条件登陆
            /// </summary>
            ANY = 1,
            /// <summary>
            /// auto sign up login
            /// 主动注册的登入
            /// </summary>
            SERVER_CONN = 2,
            /// <summary>
            /// multicast login, default
            /// 组播登陆
            /// </summary>
            MULTICAST = 3,
            /// <summary>
            /// UDP method login
            /// UDP方式下的登入
            /// </summary>
            UDP = 4,
            /// <summary>
            /// only main connection login
            /// 只建主连接下的登入
            /// </summary>
            MAIN_CONN_ONLY = 6,
            /// <summary>
            /// SSL encryption login
            /// SSL加密方式登陆
            /// </summary>
            SSL = 7,
            /// <summary>
            /// login IVS box remote device
            /// 登录智能盒远程设备
            /// </summary>
            INTELLIGENT_BOX = 9,
            /// <summary>
            /// login device do not config
            /// 登录设备后不做取配置操作
            /// </summary>
            NO_CONFIG = 10,
            /// <summary>
            /// USB key device login
            /// 用U盾设备的登入
            /// </summary>
            U_LOGIN = 11,
            /// <summary>
            /// LDAP login
            /// LDAP方式登录
            /// </summary>
            LDAP = 12,
            /// <summary>
            /// AD login
            /// AD（ActiveDirectory）登录方式
            /// </summary>
            AD = 13,
            /// <summary>
            /// Radius  login 
            /// Radius 登录方式
            /// </summary>
            RADIUS = 14,
            /// <summary>
            /// Socks5 login
            /// Socks5登陆方式
            /// </summary>
            SOCKET_5 = 15,
            /// <summary>
            /// cloud login
            /// 云登陆方式
            /// </summary>
            CLOUD = 16,
            /// <summary>
            /// dual authentication loin
            /// 二次鉴权登陆方式
            /// </summary>
            AUTH_TWICE = 17,
            /// <summary>
            /// TS stream client login
            /// TS码流客户端登陆方式
            /// </summary>
            TS = 18,
            /// <summary>
            /// web private login
            /// 为P2P登陆方式
            /// </summary>
            P2P = 19,
            /// <summary>
            /// mobile client login
            /// 手机客户端登陆
            /// </summary>
            MOBILE = 20,
            /// <summary>
            /// invalid login
            /// 无效的登陆方式
            /// </summary>          
            INVALID = 21,
        }

        // CLIENT_LoginWithHighLevelSecurity 输入参数
        public struct NET_IN_LOGIN_WITH_HIGHLEVEL_SECURITY
        {
            public uint dwSize;// 结构体大小
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szIP; // IP
            public int nPort;              // 端口
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szUserName; // 用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szPassword; // 密码
            public EM_LOGIN_SPAC_CAP_TYPE emSpecCap;           // 登录模式
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] byReserved; // 字节对齐
            public IntPtr pCapParam;            // 见 CLIENT_LoginEx 接口 pCapParam 与 nSpecCap 关系
        }

        // CLIENT_LoginWithHighLevelSecurity 输出参数
        public struct NET_OUT_LOGIN_WITH_HIGHLEVEL_SECURITY
        {
            public uint dwSize;// 结构体大小
            public NET_DEVICEINFO_Ex stuDeviceInfo;        // 设备信息
            public int nError;             // 错误码，见 CLIENT_Login 接口错误码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
            public byte[] byReserved; // 保留字节
        }

        /// <summary>
        /// device information structure
        /// 设备信息结构体
        /// </summary>
        public struct NET_DEVICEINFO_Ex
        {
            /// <summary>
            /// serial number
            /// 序列号
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string sSerialNumber;
            /// <summary>
            /// count of alarm input
            /// 报警输入个数
            /// </summary>
            public int nAlarmInPortNum;
            /// <summary>
            /// count of alarm output
            /// 报警输出个数
            /// </summary>
            public int nAlarmOutPortNum;
            /// <summary>
            /// number of disk
            /// 硬盘个数
            /// </summary>
            public int nDiskNum;
            /// <summary>
            /// device type, refer to EM_NET_DEVICE_TYPE
            /// 设备类型,见枚举NET_DEVICE_TYPE
            /// </summary>
            public EM_NET_DEVICE_TYPE nDVRType;
            /// <summary>
            /// number of channel
            /// 通道个数
            /// </summary>
            public int nChanNum;
            /// <summary>
            /// Online Timeout, Not Limited Access to 0, not 0 Minutes Limit Said
            /// 在线超时时间,为0表示不限制登陆,非0表示限制的分钟数
            /// </summary>
            public byte byLimitLoginTime;
            /// <summary>
            /// When login failed due to password error, notice user by this parameter.This parameter is invalid when remaining login times is zero
            /// 当登陆失败原因为密码错误时,通过此参数通知用户,剩余登陆次数,为0时表示此参数无效
            /// </summary>
            public byte byLeftLogTimes;
            /// <summary>
            /// keep bytes for aligned
            /// 保留字节,字节对齐
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            /// <summary>
            /// when log in failed,the left time for users to unlock (seconds), -1 indicate the device haven't set the parameter
            /// 当登陆失败,用户解锁剩余时间（秒数）, -1表示设备未设置该参数
            /// </summary>
            public int nLockLeftTime;
            /// <summary>
            /// reserved
            /// 保留字节
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Reserved;
        }

        /// <summary>
        /// device type enumeration
        /// 设备类型枚举
        /// </summary>
        public enum EM_NET_DEVICE_TYPE
        {
            /// <summary>
            /// Unknow
            //  未知
            /// </summary>
            NET_PRODUCT_NONE = 0,
            /// <summary>
            /// Non real-time MACE
            /// 非实时MACE
            /// </summary>
            NET_DVR_NONREALTIME_MACE,
            /// <summary>
            /// Non real-time
            /// 非实时
            /// </summary>
            NET_DVR_NONREALTIME,
            /// <summary>
            /// Network video server
            /// 网络视频服务器
            /// </summary>
            NET_NVS_MPEG1,
            /// <summary>
            /// MPEG1 2-ch DVR
            /// MPEG1二路录像机
            /// </summary>
            NET_DVR_MPEG1_2,
            /// <summary>
            /// MPEG1 8-ch DVR
            /// MPEG1八路录像机
            /// </summary>
            NET_DVR_MPEG1_8,
            /// <summary>
            /// MPEG4 8-ch DVR
            /// MPEG4八路录像机
            /// </summary>
            NET_DVR_MPEG4_8,
            /// <summary>
            /// MPEG4 16-ch DVR
            /// MPEG4 十六路录像机
            /// </summary>
            NET_DVR_MPEG4_16,
            /// <summary>
            /// LB series DVR
            /// LB系列录像机
            /// </summary>
            NET_DVR_MPEG4_SX2,
            /// <summary>
            /// GB  series DVR
            /// GB系列录像机
            /// </summary>
            NET_DVR_MEPG4_ST2,
            /// <summary>
            /// HB  series DVR
            /// HB系列录像机
            /// </summary>
            NET_DVR_MEPG4_SH2,
            /// <summary>
            /// GBE  series DVR
            /// GBE系列录像机
            /// </summary>
            NET_DVR_MPEG4_GBE,
            /// <summary>
            /// II network video server
            /// II代网络视频服务器
            /// </summary>
            NET_DVR_MPEG4_NVSII,
            /// <summary>
            /// New standard configuration protocol
            /// 新标准配置协议
            /// </summary>
            NET_DVR_STD_NEW,
            /// <summary>
            /// DDNS server
            /// DDNS服务器
            /// </summary>
            NET_DVR_DDNS,
            /// <summary>
            /// ATM series
            /// ATM机
            /// </summary>
            NET_DVR_ATM,
            /// <summary>
            /// 2nd non real-time NB series DVR
            /// 二代非实时NB系列机器
            /// </summary>
            NET_NB_SERIAL,
            /// <summary>
            /// LN  series
            /// LN系列产品
            /// </summary>
            NET_LN_SERIAL,
            /// <summary>
            /// BAV series
            /// BAV系列产品
            /// </summary>
            NET_BAV_SERIAL,
            /// <summary>
            /// SDIP series
            /// SDIP系列产品
            /// </summary>
            NET_SDIP_SERIAL,
            /// <summary>
            /// IPC series
            /// IPC系列产品
            /// </summary>
            NET_IPC_SERIAL,
            /// <summary>
            /// NVS B series
            /// NVS B系列
            /// </summary>
            NET_NVS_B,
            /// <summary>
            /// NVS H series
            /// NVS H系列
            /// </summary>
            NET_NVS_C,
            /// <summary>
            /// NVS S series
            /// NVS S系列
            /// </summary>
            NET_NVS_S,
            /// <summary>
            /// NVS E series
            /// NVS E系列
            /// </summary>
            NET_NVS_E,
            /// <summary>
            /// Search device type from QueryDevState. it is in string format
            /// 从QueryDevState中查询设备类型,以字符串格式
            /// </summary>
            NET_DVR_NEW_PROTOCOL,
            /// <summary>
            /// NVD
            /// 解码器
            /// </summary>
            NET_NVD_SERIAL,
            /// <summary>
            /// N5
            /// N5
            /// </summary>
            NET_DVR_N5,
            /// <summary>
            /// HDVR
            /// 混合DVR
            /// </summary>
            NET_DVR_MIX_DVR,
            /// <summary>
            /// SVR series
            /// SVR系列
            /// </summary>
            NET_SVR_SERIAL,
            /// <summary>
            /// SVR-BS
            /// SVR-BS
            /// </summary>
            NET_SVR_BS,
            /// <summary>
            /// NVR series
            /// NVR系列
            /// </summary>
            NET_NVR_SERIAL,
            /// <summary>
            /// N51
            /// N51
            /// </summary>
            NET_DVR_N51,
            /// <summary>
            /// ITSE Intelligent Analysis Box
            /// ITSE 智能分析盒
            /// </summary>
            NET_ITSE_SERIAL,
            /// <summary>
            /// Intelligent traffic camera equipment
            /// 智能交通像机设备
            /// </summary>
            NET_ITC_SERIAL,
            /// <summary>
            /// radar speedometer HWS
            /// 雷达测速仪HWS
            /// </summary>
            NET_HWS_SERIAL,
            /// <summary>
            /// portable video record
            /// 便携式音视频录像机
            /// </summary>
            NET_PVR_SERIAL,
            /// <summary>
            /// IVS(intelligent video server series)
            /// IVS（智能视频服务器系列）
            /// </summary>
            NET_IVS_SERIAL,
            /// <summary>
            /// universal intelligent detect video server series 
            /// 通用智能视频侦测服务器
            /// </summary>
            NET_IVS_B,
            /// <summary>
            /// face recognisation server
            /// 人脸识别服务器
            /// </summary>
            NET_IVS_F,
            /// <summary>
            /// video quality diagnosis server
            /// 视频质量诊断服务器
            /// </summary>
            NET_IVS_V,
            /// <summary>
            /// matrix
            /// 矩阵
            /// </summary>
            NET_MATRIX_SERIAL,
            /// <summary>
            /// N52
            /// N52
            /// </summary>
            NET_DVR_N52,
            /// <summary>
            /// N56
            /// N56
            /// </summary>
            NET_DVR_N56,
            /// <summary>
            /// ESS
            /// ESS
            /// </summary>
            NET_ESS_SERIAL,
            /// <summary>
            /// 人数统计服务器
            /// </summary>
            NET_IVS_PC,
            /// <summary>
            /// pc-nvr
            /// pc-nvr
            /// </summary>
            NET_PC_NVR,
            /// <summary>
            /// screen controller
            /// 大屏控制器
            /// </summary>
            NET_DSCON,
            /// <summary>
            /// network video storage server
            /// 网络视频存储服务器
            /// </summary>
            NET_EVS,
            /// <summary>
            /// an embedded intelligent video analysis system
            /// 嵌入式智能分析视频系统
            /// </summary>
            NET_EIVS,
            /// <summary>
            /// DVR-N6
            /// DVR-N6
            /// </summary>
            NET_DVR_N6,
            /// <summary>
            /// K-Lite Codec Pack
            /// 万能解码器
            /// </summary>
            NET_UDS,
            /// <summary>
            /// Bank alarm host
            /// 银行报警主机
            /// </summary>
            NET_AF6016,
            /// <summary>
            /// Video network alarm host
            /// 视频网络报警主机
            /// </summary>
            NET_AS5008,
            /// <summary>
            /// Network alarm host
            /// 网络报警主机
            /// </summary>
            NET_AH2008,
            /// <summary>
            /// Alarm host series
            /// 报警主机系列
            /// </summary>
            NET_A_SERIAL,
            /// <summary>
            /// Access control series of products
            /// 门禁系列产品
            /// </summary>
            NET_BSC_SERIAL,
            /// <summary>
            /// NVS series product
            /// NVS系列产品
            /// </summary>
            NET_NVS_SERIAL,
            /// <summary>
            /// VTO series product
            /// VTO系列产品
            /// </summary>                           
            NET_VTO_SERIAL,
            /// <summary>
            /// VTNC series product
            /// VTNC系列产品
            /// </summary>
            NET_VTNC_SERIAL,
            /// <summary>
            /// TPC series product, it is the thermal device 
            /// TPC系列产品, 即热成像设备
            /// </summary>
            NET_TPC_SERIAL,
            /// <summary>
            /// ASM series product
            /// 无线中继设备
            /// </summary>
            NET_ASM_SERIAL,
            /// <summary>
            /// VTS series product
            /// 管理机
            /// </summary>
            NET_VTS_SERIAL,
            /// <summary>
            /// Alarm host-ARC2016C
            /// 报警主机ARC2016C
            /// </summary>
            NET_ARC2016C,
            /// <summary>
            /// ASA Attendance machine
            /// 考勤机
            /// </summary>
            NET_ASA,
            /// <summary>
            /// Industry terminal walkie-talkie
            /// 行业对讲终端
            /// </summary>
            NET_VTT_SERIAL,
            /// <summary>
            /// Alarm column
            /// 报警柱
            /// </summary>
            NET_VTA_SERIAL,
            /// <summary>
            /// SIP Server
            /// SIP服务器
            /// </summary>
            NET_VTNS_SERIAL,
            /// <summary>
            /// Indoor unit
            /// 室内机
            /// </summary>
            NET_VTH_SERIAL,
        }

        /// <summary>
        /// realplay type
        /// 监视类型
        /// </summary>
        public enum EM_RealPlayType
        {
            /// <summary>
            /// Real-time preview
            /// 实时预览
            /// </summary>
            Realplay = 0,
            /// <summary>
            /// Multiple-channel preview 
            /// 多画面预览
            /// </summary>
            Multiplay,
            /// <summary>
            /// Real-time monitor-main stream. It is the same as EM_RealPlayType.Realplay
            /// 实时监视-主码流,等同于EM_RealPlayType.Realplay
            /// </summary>
            Realplay_0,
            /// <summary>
            /// Real-time monitor -- extra stream 1
            /// 实时监视-从码流1
            /// </summary>
            Realplay_1,
            /// <summary>
            /// Real-time monitor -- extra stream 2
            /// 实时监视-从码流2
            /// </summary>
            Realplay_2,
            /// <summary>
            /// Real-time monitor -- extra stream 3
            /// 实时监视-从码流3
            /// </summary>
            Realplay_3,
            /// <summary>
            /// Multiple-channel preview--1-window 
            /// 多画面预览－1画面
            /// </summary>
            Multiplay_1,
            /// <summary>
            /// Multiple-channel preview--4-window
            /// 多画面预览－4画面
            /// </summary>
            Multiplay_4,
            /// <summary>
            /// Multiple-channel preview--8-window
            /// 多画面预览－8画面
            /// </summary>
            Multiplay_8,
            /// <summary>
            /// Multiple-channel preview--9-window
            /// 多画面预览－9画面
            /// </summary>
            Multiplay_9,
            /// <summary>
            /// Multiple-channel preview--16-window
            /// 多画面预览－16画面
            /// </summary>
            Multiplay_16,
            /// <summary>
            /// Multiple-channel preview--6-window
            /// 多画面预览－6画面
            /// </summary>
            Multiplay_6,
            /// <summary>
            /// Multiple-channel preview--12-window
            /// 多画面预览－12画面
            /// </summary>
            Multiplay_12,
            /// <summary>
            /// Multiple-channel preview--25-window
            /// 多画面预览－25画面
            /// </summary>
            Multiplay_25,
            /// <summary>
            /// Multiple-channel preview--36-window
            /// 多画面预览－36画面
            /// </summary>
            Multiplay_36,
            /// <summary>
            /// test stream
            /// 带宽测试码流 
            /// </summary>
            Realplay_Test = 255,
        }

        /// <summary>
        /// input param for AttachVideoStatSummary
        /// AttachVideoStatSummary 入参
        /// </summary>
        public struct NET_IN_ATTACH_VIDEOSTAT_SUM
        {
            /// <summary>
            /// struct size
            /// 结构体大小
            /// </summary>
            public uint dwSize;
            /// <summary>
            /// video channel ID    
            /// 视频通道号
            /// </summary>
            public int nChannel;
            /// <summary>
            /// video statistical summary callback
            /// 视频统计摘要信息回调
            /// </summary>
            public DhSdk.fVideoStatSumCallBack cbVideoStatSum;
            /// <summary>
            /// user data
            /// 用户数据
            /// </summary>
            public IntPtr dwUser;
        }

        /// <summary>
        /// output param for AttachVideoStatSummary
        /// AttachVideoStatSummary 出参
        /// </summary>
        public struct NET_OUT_ATTACH_VIDEOSTAT_SUM
        {
            /// <summary>
            /// struct size
            /// 结构体大小
            /// </summary>
            public uint dwSize;
        }

    }
}
