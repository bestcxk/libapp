using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.DhCamera
{
    /// <summary>
    /// 大华摄像头业务方法
    /// </summary>
    public static class DhSdk
    {
        /// <summary>
        /// whether to throw an exception.
        /// 是否抛异常
        /// </summary>
        private static bool m_IsThrowErrorMessage = false;

        #region 错误码对应错误信息

        /// <summary>
        /// zh-cn language
        /// 中文错误码对应的错误信息
        /// </summary>
        private static Dictionary<DhStruct.EM_ErrorCode, string> zh_cn_String = new Dictionary<DhStruct.EM_ErrorCode, string>()
        {
            {DhStruct.EM_ErrorCode.NET_NOERROR,"没有错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR,"未知错误"},
            {DhStruct.EM_ErrorCode.NET_SYSTEM_ERROR,"Windows系统出错"},
            {DhStruct.EM_ErrorCode.NET_NETWORK_ERROR,"网络错误,可能是因为网络超时"},
            {DhStruct.EM_ErrorCode.NET_DEV_VER_NOMATCH,"设备协议不匹配"},
            {DhStruct.EM_ErrorCode.NET_INVALID_HANDLE,"句柄无效"},
            {DhStruct.EM_ErrorCode.NET_OPEN_CHANNEL_ERROR,"打开通道失败"},
            {DhStruct.EM_ErrorCode.NET_CLOSE_CHANNEL_ERROR,"关闭通道失败"},
            {DhStruct.EM_ErrorCode.NET_ILLEGAL_PARAM,"用户参数不合法"},
            {DhStruct.EM_ErrorCode.NET_SDK_INIT_ERROR,"SDK初始化出错"},
            {DhStruct.EM_ErrorCode.NET_SDK_UNINIT_ERROR,"SDK清理出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_OPEN_ERROR,"申请render资源出错"},
            {DhStruct.EM_ErrorCode.NET_DEC_OPEN_ERROR,"打开解码库出错"},
            {DhStruct.EM_ErrorCode.NET_DEC_CLOSE_ERROR,"关闭解码库出错"},
            {DhStruct.EM_ErrorCode.NET_MULTIPLAY_NOCHANNEL,"多画面预览中检测到通道数为0"},
            {DhStruct.EM_ErrorCode.NET_TALK_INIT_ERROR,"录音库初始化失败"},
            {DhStruct.EM_ErrorCode.NET_TALK_NOT_INIT,"录音库未经初始化"},
            {DhStruct.EM_ErrorCode.NET_TALK_SENDDATA_ERROR,"发送音频数据出错"},
            {DhStruct.EM_ErrorCode.NET_REAL_ALREADY_SAVING,"实时数据已经处于保存状态"},
            {DhStruct.EM_ErrorCode.NET_NOT_SAVING,"未保存实时数据"},
            {DhStruct.EM_ErrorCode.NET_OPEN_FILE_ERROR,"打开文件出错"},
            {DhStruct.EM_ErrorCode.NET_PTZ_SET_TIMER_ERROR,"启动云台控制定时器失败"},
            {DhStruct.EM_ErrorCode.NET_RETURN_DATA_ERROR,"对返回数据的校验出错"},
            {DhStruct.EM_ErrorCode.NET_INSUFFICIENT_BUFFER,"没有足够的缓存"},
            {DhStruct.EM_ErrorCode.NET_NOT_SUPPORTED,"当前SDK未支持该功能"},
            {DhStruct.EM_ErrorCode.NET_NO_RECORD_FOUND,"查询不到录象"},
            {DhStruct.EM_ErrorCode.NET_NOT_AUTHORIZED,"无操作权限"},
            {DhStruct.EM_ErrorCode.NET_NOT_NOW,"暂时无法执行"},
            {DhStruct.EM_ErrorCode.NET_NO_TALK_CHANNEL,"未发现对讲通道"},
            {DhStruct.EM_ErrorCode.NET_NO_AUDIO,"未发现音频"},
            {DhStruct.EM_ErrorCode.NET_NO_INIT,"网络SDK未经初始化"},
            {DhStruct.EM_ErrorCode.NET_DOWNLOAD_END,"下载已结束"},
            {DhStruct.EM_ErrorCode.NET_EMPTY_LIST,"查询结果为空"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SYSATTR,"获取系统属性配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SERIAL,"获取序列号失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_GENERAL,"获取常规属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DSPCAP,"获取DSP能力描述失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_NETCFG,"获取网络配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_CHANNAME,"获取通道名称失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEO,"获取视频属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_RECORD,"获取录象配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_PRONAME,"获取解码器协议名称失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_FUNCNAME,"获取232串口功能名称失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_485DECODER,"获取解码器属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_232COM,"获取232串口配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ALARMIN,"获取外部报警输入配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ALARMDET,"获取动态检测报警失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SYSTIME,"获取设备时间失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_PREVIEW,"获取预览参数失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_AUTOMT,"获取自动维护配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEOMTRX,"获取视频矩阵配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_COVER,"获取区域遮挡配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WATERMAKE,"获取图象水印配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MULTICAST,"获取配置失败位置：组播端口按通道配置"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_GENERAL,"修改常规属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_NETCFG,"修改网络配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_CHANNAME,"修改通道名称失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEO,"修改视频属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_RECORD,"修改录象配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_485DECODER,"修改解码器属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_232COM,"修改232串口配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ALARMIN,"修改外部输入报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ALARMDET,"修改动态检测报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SYSTIME,"修改设备时间失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_PREVIEW,"修改预览参数失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_AUTOMT,"修改自动维护配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEOMTRX,"修改视频矩阵配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_COVER,"修改区域遮挡配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WATERMAKE,"修改图象水印配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WLAN,"修改无线网络信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WLANDEV,"选择无线网络设备失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_REGISTER,"修改主动注册参数配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_CAMERA,"修改摄像头属性配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_INFRARED,"修改红外报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SOUNDALARM,"修改音频报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_STORAGE,"修改存储位置配置失败"},
            {DhStruct.EM_ErrorCode.NET_AUDIOENCODE_NOTINIT,"音频编码接口没有成功初始化"},
            {DhStruct.EM_ErrorCode.NET_DATA_TOOLONGH,"数据过长"},
            {DhStruct.EM_ErrorCode.NET_UNSUPPORTED,"设备不支持该操作"},
            {DhStruct.EM_ErrorCode.NET_DEVICE_BUSY,"设备资源不足"},
            {DhStruct.EM_ErrorCode.NET_SERVER_STARTED,"服务器已经启动"},
            {DhStruct.EM_ErrorCode.NET_SERVER_STOPPED,"服务器尚未成功启动"},
            {DhStruct.EM_ErrorCode.NET_LISTER_INCORRECT_SERIAL,"输入序列号有误"},
            {DhStruct.EM_ErrorCode.NET_QUERY_DISKINFO_FAILED,"获取硬盘信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SESSION,"获取连接Session信息"},
            {DhStruct.EM_ErrorCode.NET_USER_FLASEPWD_TRYTIME,"输入密码错误超过限制次数"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_PASSWORD,"密码不正确"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_USER,"帐户不存在"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_TIMEOUT,"等待登录返回超时"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_RELOGGIN,"帐号已登录"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_LOCKED,"帐号已被锁定"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_BLACKLIST,"帐号已被列为黑名单"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_BUSY,"资源不足,系统忙"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_CONNECT,"登录设备超时,请检查网络并重试"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_NETWORK,"网络连接失败"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_SUBCONNECT,"登录设备成功,但无法创建视频通道,请检查网络状况"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_MAXCONNECT,"超过最大连接数"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_PROTOCOL3_ONLY,"只支持3代协议"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_UKEY_LOST,"未插入U盾或U盾信息错误"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_NO_AUTHORIZED,"客户端IP地址没有登录权限"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_USER_OR_PASSOWRD,"账号或密码错误"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SOUND_ON_ERROR,"Render库打开音频出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SOUND_OFF_ERROR,"Render库关闭音频出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SET_VOLUME_ERROR,"Render库控制音量出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_ADJUST_ERROR,"Render库设置画面参数出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_PAUSE_ERROR,"Render库暂停播放出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SNAP_ERROR,"Render库抓图出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_STEP_ERROR,"Render库步进出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_FRAMERATE_ERROR,"Render库设置帧率出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_DISPLAYREGION_ERROR,"Render库设置显示区域出错"},
            {DhStruct.EM_ErrorCode.NET_RENDER_GETOSDTIME_ERROR,"Render库获取当前播放时间出错"},
            {DhStruct.EM_ErrorCode.NET_GROUP_EXIST,"组名已存在"},
            {DhStruct.EM_ErrorCode.NET_GROUP_NOEXIST,"组名不存在"},
            {DhStruct.EM_ErrorCode.NET_GROUP_RIGHTOVER,"组的权限超出权限列表范围"},
            {DhStruct.EM_ErrorCode.NET_GROUP_HAVEUSER,"组下有用户,不能删除"},
            {DhStruct.EM_ErrorCode.NET_GROUP_RIGHTUSE,"组的某个权限被用户使用,不能出除"},
            {DhStruct.EM_ErrorCode.NET_GROUP_SAMENAME,"新组名同已有组名重复"},
            {DhStruct.EM_ErrorCode.NET_USER_EXIST,"用户已存在"},
            {DhStruct.EM_ErrorCode.NET_USER_NOEXIST,"用户不存在"},
            {DhStruct.EM_ErrorCode.NET_USER_RIGHTOVER,"用户权限超出组权限"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD,"保留帐号,不容许修改密码"},
            {DhStruct.EM_ErrorCode.NET_USER_FLASEPWD,"密码不正确"},
            {DhStruct.EM_ErrorCode.NET_USER_NOMATCHING,"密码不匹配"},
            {DhStruct.EM_ErrorCode.NET_USER_INUSE,"账号正在使用中"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ETHERNET,"获取网卡配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WLAN,"获取无线网络信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WLANDEV,"获取无线网络设备失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_REGISTER,"获取主动注册参数失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_CAMERA,"获取摄像头属性失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_INFRARED,"获取红外报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SOUNDALARM,"获取音频报警配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_STORAGE,"获取存储位置配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MAIL,"获取邮件配置失败"},
            {DhStruct.EM_ErrorCode.NET_CONFIG_DEVBUSY,"暂时无法设置"},
            {DhStruct.EM_ErrorCode.NET_CONFIG_DATAILLEGAL,"配置数据不合法"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DST,"获取夏令时配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_DST,"设置夏令时配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEO_OSD,"获取视频OSD叠加配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEO_OSD,"设置视频OSD叠加配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_GPRSCDMA,"获取CDMA\\GPRS网络配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_GPRSCDMA,"设置CDMA\\GPRS网络配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_IPFILTER,"获取IP过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_IPFILTER,"设置IP过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_TALKENCODE,"获取语音对讲编码配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_TALKENCODE,"设置语音对讲编码配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_RECORDLEN,"获取录像打包长度配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_RECORDLEN,"设置录像打包长度配置失败"},
            {DhStruct.EM_ErrorCode.NET_DONT_SUPPORT_SUBAREA,"不支持网络硬盘分区"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_AUTOREGSERVER,"获取设备上主动注册服务器信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CONTROL_AUTOREGISTER,"主动注册重定向注册错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DISCONNECT_AUTOREGISTER,"断开主动注册服务器错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MMS,"获取mms配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_MMS,"设置mms配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SMSACTIVATION,"获取短信激活无线连接配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SMSACTIVATION,"设置短信激活无线连接配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DIALINACTIVATION,"获取拨号激活无线连接配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_DIALINACTIVATION,"设置拨号激活无线连接配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEOOUT,"查询视频输出参数配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEOOUT,"设置视频输出参数配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_OSDENABLE,"获取osd叠加使能配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_OSDENABLE,"设置osd叠加使能配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ENCODERINFO,"设置数字通道前端编码接入配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_TVADJUST,"获取TV调节配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_TVADJUST,"设置TV调节配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CONNECT_FAILED,"请求建立连接失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_BURNFILE,"请求刻录文件上传失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SNIFFER_GETCFG,"获取抓包配置信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SNIFFER_SETCFG,"设置抓包配置信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DOWNLOADRATE_GETCFG,"查询下载限制信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DOWNLOADRATE_SETCFG,"设置下载限制信息失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SEARCH_TRANSCOM,"查询串口参数失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_POINT,"获取预制点信息错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_POINT,"设置预制点信息错误"},
            {DhStruct.EM_ErrorCode.NET_SDK_LOGOUT_ERROR,"SDK没有正常登出设备"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_VEHICLE_CFG,"获取车载配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_VEHICLE_CFG,"设置车载配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_ATM_OVERLAY_CFG,"获取atm叠加配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_ATM_OVERLAY_CFG,"设置atm叠加配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_ATM_OVERLAY_ABILITY,"获取atm叠加能力失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DECODER_TOUR_CFG,"获取解码器解码轮巡配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_DECODER_TOUR_CFG,"设置解码器解码轮巡配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CTRL_DECODER_TOUR,"控制解码器解码轮巡失败"},
            {DhStruct.EM_ErrorCode.NET_GROUP_OVERSUPPORTNUM,"超出设备支持最大用户组数目"},
            {DhStruct.EM_ErrorCode.NET_USER_OVERSUPPORTNUM,"超出设备支持最大用户数目"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_SIP_CFG,"获取SIP配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_SIP_CFG,"设置SIP配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_SIP_ABILITY,"获取SIP能力失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_WIFI_AP_CFG,"获取WIFI ap配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_WIFI_AP_CFG,"设置WIFI ap配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DECODE_POLICY,"获取解码策略配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_DECODE_POLICY,"设置解码策略配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_REJECT,"拒绝对讲"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_OPENED,"对讲被其他客户端打开"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_RESOURCE_CONFLICIT,"资源冲突"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_UNSUPPORTED_ENCODE,"不支持的语音编码格式"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_RIGHTLESS,"无权限"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_FAILED,"请求对讲失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_MACHINE_CFG,"获取机器相关配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_MACHINE_CFG,"设置机器相关配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DATA_FAILED,"设备无法获取当前请求数据"},
            {DhStruct.EM_ErrorCode.NET_ERROR_MAC_VALIDATE_FAILED,"MAC地址验证失败 "},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_INSTANCE,"获取服务器实例失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_JSON_REQUEST,"生成的jason字符串错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_JSON_RESPONSE,"响应的jason字符串错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_VERSION_HIGHER,"协议版本低于当前使用的版本"},
            {DhStruct.EM_ErrorCode.NET_SPARE_NO_CAPACITY,"热备操作失败, 容量不足"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SOURCE_IN_USE,"显示源被其他输出占用"},
            {DhStruct.EM_ErrorCode.NET_ERROR_REAVE,"高级用户抢占低级用户资源"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NETFORBID,"禁止入网 "},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MACFILTER,"获取MAC过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_MACFILTER,"设置MAC过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_IPMACFILTER,"获取IP/MAC过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_IPMACFILTER,"设置IP/MAC过滤配置失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OPERATION_OVERTIME,"当前操作超时 "},
            {DhStruct.EM_ErrorCode.NET_ERROR_SENIOR_VALIDATE_FAILED,"高级校验失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_ID_NOT_EXIST,"设备ID不存在"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNSUPPORTED,"不支持当前操作"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_DLLLOAD,"代理库加载失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_ILLEGAL_PARAM,"代理用户参数不合法"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_INVALID_HANDLE,"代理句柄无效"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_LOGIN_DEVICE_ERROR,"代理登入前端设备失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_START_SERVER_ERROR,"启动代理服务失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SPEAK_FAILED,"请求喊话失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NOT_SUPPORT_F6,"设备不支持此F6接口调用"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CD_UNREADY,"光盘未就绪"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DIR_NOT_EXIST,"目录不存在"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNSUPPORTED_SPLIT_MODE,"设备不支持的分割模式"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OPEN_WND_PARAM,"开窗参数不合法"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_WND_COUNT,"开窗数量超过限制"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNMATCHED_REQUEST,"请求命令与当前模式不匹配"},
            {DhStruct.EM_ErrorCode.NET_RENDER_ENABLELARGEPICADJUSTMENT_ERROR,"Render库启用高清图像内部调整策略出错"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UPGRADE_FAILED,"设备升级失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_TARGET_DEVICE,"找不到目标设备"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_VERIFY_DEVICE,"找不到验证设备"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CASCADE_RIGHTLESS,"无级联权限"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOW_PRIORITY,"低优先级"},
            {DhStruct.EM_ErrorCode.NET_ERROR_REMOTE_REQUEST_TIMEOUT,"远程设备请求超时"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_INPUT_SOURCE,"输入源超出最大路数限制"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_LOG_PRINT_INFO,"设置日志打印失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PARAM_DWSIZE_ERROR,"入参的dwsize字段出错"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_MONITORWALL_COUNT,"电视墙数量超过上限"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PART_PROCESS_FAILED,"部分过程执行失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TARGET_NOT_SUPPORT,"该功能不支持转发"},
            {DhStruct.EM_ErrorCode.NET_ERROR_VISITE_FILE,"访问文件失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_STATUS_BUSY,"设备忙"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD_NOT_AUTHORIZED,"修改密码无权限"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD_NOT_STRONG,"密码强度不够"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_SUCH_CONFIG,"没有对应的配置"},
            {DhStruct.EM_ErrorCode.NET_ERROR_AUDIO_RECORD_FAILED,"录音失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SEND_DATA_FAILED,"数据发送失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OBSOLESCENT_INTERFACE,"废弃接口"},
            {DhStruct.EM_ErrorCode.NET_ERROR_INSUFFICIENT_INTERAL_BUF,"内部缓冲不足"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NEED_ENCRYPTION_PASSWORD,"修改设备ip时,需要校验密码"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NOSUPPORT_RECORD,"设备不支持此记录集"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SERIALIZE_ERROR,"数据序列化错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DESERIALIZE_ERROR,"数据反序列化错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_EXISTED,"该无线ID已存在"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_LIMIT,"无线ID数量已超限"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_ABNORMAL,"无线异常添加"},
            {DhStruct.EM_ErrorCode.NET_ERROR_ENCRYPT, "加密数据失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PWD_ILLEGAL, "新密码不合规范"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_ALREADY_INIT, "设备已经初始化"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SECURITY_CODE, "安全码错误"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SECURITY_CODE_TIMEOUT, "安全码超出有效期"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_PWD_SPECI, "获取密码规范失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_AUTHORITY_OF_OPERATION, "无权限进行该操作"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DECRYPT, "解密数据失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_2D_CODE, "2D code校验失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_INVALID_REQUEST, "非法的RPC请求"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PWD_RESET_DISABLE, "密码重置功能已关闭"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PLAY_PRIVATE_DATA, "显示私有数据，比如规则框等失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_ROBOT_OPERATE_FAILED, "机器人操作失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CHANNEL_ALREADY_OPENED, "通道已经打开"},

            {DhStruct.EM_ErrorCode.ERR_INTERNAL_INVALID_CHANNEL,"错误的通道号"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_REOPEN_CHANNEL,"打开重复通道"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_SEND_DATA,"发送消息失败"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_CREATE_SOCKET,"创建socket失败"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_LISTEN_FAILED,"启动监听失败"},
            {DhStruct.EM_ErrorCode.NET_ERROR_FACE_RECOGNITION_SERVER_GROUP_ID_EXCEED, "组ID超过最大值" },
            {DhStruct.EM_ErrorCode.ERR_NOT_SUPPORT_HIGHLEVEL_SECURITY_LOGIN, "设备不支持高安全等级登录" },

        };

        /// <summary>
        /// catch a figure type CLIENT_CapturePictureEx interface using
        /// 抓图类型
        /// </summary>
        public enum EM_NET_CAPTURE_FORMATS
        {
            /// <summary>
            /// BMP
            /// BMP
            /// </summary>
            BMP,
            /// <summary>
            /// 100% quality JPEG
            /// 100%质量的JPEG
            /// </summary>
            JPEG,
            /// <summary>
            /// 70% quality JPEG
            /// 70%质量的JPEG
            /// </summary>
            JPEG_70,
            /// <summary>
            /// 50% quality JPEG
            /// 50%质量的JPEG
            /// </summary>                       
            JPEG_50,
            /// <summary>
            /// 30% quality JPEG
            /// 30%质量的JPEG
            /// </summary>
            JPEG_30,
        }

        /// <summary>
        /// set snapshot callback function
        /// 设置远程抓图回调
        /// </summary>
        /// <param name="OnSnapRevMessage">snapshot data callback function 抓图数据回调</param>
        /// <param name="dwUser">user data, there is no data, please use IntPtr.Zero 用户数据</param>
        public static void SetSnapRevCallBack(fSnapRevCallBack OnSnapRevMessage, IntPtr dwUser)
        {
            ImportDhSdk.CLIENT_SetSnapRevCallBack(OnSnapRevMessage, dwUser);
        }

        /// <summary>
        /// snapshot callback function original shape
        /// 远程抓图数据回调
        /// </summary>
        /// <param name="lLoginID">loginID,login returns value 登陆ID</param>
        /// <param name="pBuf">byte array, length is RevLen 数据缓存
        ///                    <para>pointer to data</para></param>
        /// <param name="RevLen">pBuf's size 数据缓存大小</param>
        /// <param name="EncodeType">image encode type：0：mpeg4 I frame;10：jpeg 编码类型</param>
        /// <param name="CmdSerial">operation NO.,not used in Synchronous capture conditions 序列号</param>
        /// <param name="dwUser">user data,which input above 用户数据</param>
        public delegate void fSnapRevCallBack(IntPtr lLoginID, IntPtr pBuf, uint RevLen, uint EncodeType, uint CmdSerial, IntPtr dwUser);


        /// <summary>
        /// start real-time monitor.support 32bit and 64bit
        /// 开始实时监视.支持32位和64位
        /// </summary>
        /// <param name="lLoginID">user LoginID:Login's returns value 登陆ID,Login返回值</param>
        /// <param name="nChannelID">real time monitor channel NO.(from 0). 通道号</param>
        /// <param name="hWnd">display window handle. When value is 0(IntPtr.Zero), data are not decoded or displayed 显示窗口句柄</param>
        /// <param name="rType">realplay type 监视类型</param>
        /// <returns>failed return 0, successful return the real time monitorID(real time monitor handle),as parameter of related function. 失败返回0，成功返回大于0的值</returns>
        public static IntPtr RealPlay(IntPtr lLoginID, int nChannelID, IntPtr hWnd, DhStruct.EM_RealPlayType rType = DhStruct.EM_RealPlayType.Realplay)
        {
            IntPtr result = IntPtr.Zero;
            result = ImportDhSdk.CLIENT_RealPlayEx(lLoginID, nChannelID, hWnd, rType);
            NetGetLastError(result);
            return result;
        }


        /// <summary>
        ///  capture a picture
        ///  本地抓图
        /// </summary>
        /// <param name="hPlayHandle">real handle or palyback handle 实时监视或回放的句柄
        ///                            <para>StartRealPlay returns value StartRealPlay返回值</para>
        ///                            <para>PlayBackByTime returns value PlayBackByTime返回值</para></param>
        /// <param name="pchPicFileName">picture's saving name 保存的文件路径</param>
        /// <param name="eFormat">picture type 图片类型</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool CapturePicture(IntPtr hPlayHandle, string pchPicFileName, EM_NET_CAPTURE_FORMATS eFormat)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_CapturePictureEx(hPlayHandle, pchPicFileName, eFormat);
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// snapshot request
        /// 远程抓图请求
        /// </summary>
        /// <param name="lLoginID">user LoginID:Login's return value 登陆ID，Login返回值</param>
        /// <param name="par">Snapshot parameter(structure) 抓图参数</param>
        /// <param name="reserved">reserved 保留参数</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool SnapPictureEx(IntPtr lLoginID, NET_SNAP_PARAMS par, IntPtr reserved)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_SnapPictureEx(lLoginID, ref par, reserved);
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// en-us language
        /// 英文错误码对应的错误信息
        /// </summary>
        private static Dictionary<DhStruct.EM_ErrorCode, string> en_us_String = new Dictionary<DhStruct.EM_ErrorCode, string>()
        {
            {DhStruct.EM_ErrorCode.NET_NOERROR,"No error"},
            {DhStruct.EM_ErrorCode.NET_ERROR,"Unknown error"},
            {DhStruct.EM_ErrorCode.NET_SYSTEM_ERROR,"Windows system error"},
            {DhStruct.EM_ErrorCode.NET_NETWORK_ERROR,"Protocol error it may result from network timeout"},
            {DhStruct.EM_ErrorCode.NET_DEV_VER_NOMATCH,"Device protocol does not match"},
            {DhStruct.EM_ErrorCode.NET_INVALID_HANDLE,"Handle is invalid"},
            {DhStruct.EM_ErrorCode.NET_OPEN_CHANNEL_ERROR,"Failed to open channel"},
            {DhStruct.EM_ErrorCode.NET_CLOSE_CHANNEL_ERROR,"Failed to close channel"},
            {DhStruct.EM_ErrorCode.NET_ILLEGAL_PARAM,"User parameter is illegal"},
            {DhStruct.EM_ErrorCode.NET_SDK_INIT_ERROR,"SDK initialization error"},
            {DhStruct.EM_ErrorCode.NET_SDK_UNINIT_ERROR,"SDK clear error"},
            {DhStruct.EM_ErrorCode.NET_RENDER_OPEN_ERROR,"Error occurs when apply for render resources"},
            {DhStruct.EM_ErrorCode.NET_DEC_OPEN_ERROR,"Error occurs when opening the decoder library"},
            {DhStruct.EM_ErrorCode.NET_DEC_CLOSE_ERROR,"Error occurs when closing the decoder library"},
            {DhStruct.EM_ErrorCode.NET_MULTIPLAY_NOCHANNEL,"The detected channel number is 0 in multiple-channel preview"},
            {DhStruct.EM_ErrorCode.NET_TALK_INIT_ERROR,"Failed to initialize record library"},
            {DhStruct.EM_ErrorCode.NET_TALK_NOT_INIT,"The record library has not been initialized"},
            {DhStruct.EM_ErrorCode.NET_TALK_SENDDATA_ERROR,"Error occurs when sending out audio data"},
            {DhStruct.EM_ErrorCode.NET_REAL_ALREADY_SAVING,"The real-time has been protected"},
            {DhStruct.EM_ErrorCode.NET_NOT_SAVING,"The real-time data has not been save"},
            {DhStruct.EM_ErrorCode.NET_OPEN_FILE_ERROR,"Error occurs when opening the file"},
            {DhStruct.EM_ErrorCode.NET_PTZ_SET_TIMER_ERROR,"Failed to enable PTZ to control timer"},
            {DhStruct.EM_ErrorCode.NET_RETURN_DATA_ERROR,"Error occurs when verify returned data"},
            {DhStruct.EM_ErrorCode.NET_INSUFFICIENT_BUFFER,"There is no sufficient buffer"},
            {DhStruct.EM_ErrorCode.NET_NOT_SUPPORTED,"The current SDK does not support this function"},
            {DhStruct.EM_ErrorCode.NET_NO_RECORD_FOUND,"There is no searched result"},
            {DhStruct.EM_ErrorCode.NET_NOT_AUTHORIZED,"You have no operation right"},
            {DhStruct.EM_ErrorCode.NET_NOT_NOW,"Can not operate right now"},
            {DhStruct.EM_ErrorCode.NET_NO_TALK_CHANNEL,"There is no audio talk channel"},
            {DhStruct.EM_ErrorCode.NET_NO_AUDIO,"There is no audio"},
            {DhStruct.EM_ErrorCode.NET_NO_INIT,"The network SDK has not been initialized"},
            {DhStruct.EM_ErrorCode.NET_DOWNLOAD_END,"The download completed"},
            {DhStruct.EM_ErrorCode.NET_EMPTY_LIST,"There is no searched result"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SYSATTR,"Failed to get system property setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SERIAL,"Failed to get SN"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_GENERAL,"Failed to get general property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DSPCAP,"Failed to get DSP capacity description"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_NETCFG,"Failed to get network channel setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_CHANNAME,"Failed to get channel name"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEO,"Failed to get video property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_RECORD,"Failed to get record setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_PRONAME,"Failed to get decoder protocol name"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_FUNCNAME,"Failed to get 232 COM function name"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_485DECODER,"Failed to get decoder property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_232COM,"Failed to get 232 COM setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ALARMIN,"Failed to get external alarm input setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ALARMDET,"Failed to get motion detection alarm"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SYSTIME,"Failed to get device time"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_PREVIEW,"Failed to get preview parameter"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_AUTOMT,"Failed to get audio maintenance setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEOMTRX,"Failed to get video matrix setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_COVER,"Failed to get privacy mask zone setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WATERMAKE,"Failed to get video watermark setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MULTICAST,"Failed to get config multicast port by channel"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_GENERAL,"Failed to modify general property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_NETCFG,"Failed to modify channel setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_CHANNAME,"Failed to modify channel name"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEO,"Failed to modify video channel"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_RECORD,"Failed to modify record setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_485DECODER,"Failed to modify decoder property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_232COM,"Failed to modify 232 COM setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ALARMIN,"Failed to modify external input alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ALARMDET,"Failed to modify motion detection alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SYSTIME,"Failed to modify device time"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_PREVIEW,"Failed to modify preview parameter"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_AUTOMT,"Failed to modify auto maintenance setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEOMTRX,"Failed to modify video matrix setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_COVER,"Failed to modify privacy mask zone"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WATERMAKE,"Failed to modify video watermark setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WLAN,"Failed to modify wireless network information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_WLANDEV,"Failed to select wireless network device"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_REGISTER,"Failed to modify the actively registration parameter setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_CAMERA,"Failed to modify camera property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_INFRARED,"Failed to modify IR alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SOUNDALARM,"Failed to modify audio alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_STORAGE,"Failed to modify storage position setup"},
            {DhStruct.EM_ErrorCode.NET_AUDIOENCODE_NOTINIT,"The audio encode port has not been successfully initialized"},
            {DhStruct.EM_ErrorCode.NET_DATA_TOOLONGH,"The data are too long"},
            {DhStruct.EM_ErrorCode.NET_UNSUPPORTED,"The device does not support current operation"},
            {DhStruct.EM_ErrorCode.NET_DEVICE_BUSY,"Device resources is not sufficient"},
            {DhStruct.EM_ErrorCode.NET_SERVER_STARTED,"The server has boot up"},
            {DhStruct.EM_ErrorCode.NET_SERVER_STOPPED,"The server has not fully boot up"},
            {DhStruct.EM_ErrorCode.NET_LISTER_INCORRECT_SERIAL,"Input serial number is not correct"},
            {DhStruct.EM_ErrorCode.NET_QUERY_DISKINFO_FAILED,"Failed to get HDD information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SESSION,"Failed to get connect session information"},
            {DhStruct.EM_ErrorCode.NET_USER_FLASEPWD_TRYTIME,"The password you typed is incorrect. You have exceeded the maximum number of retries"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_PASSWORD,"Password is not correct"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_USER,"The account does not exist"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_TIMEOUT,"Time out for log in returned value"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_RELOGGIN,"The account has logged in"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_LOCKED,"The account has been locked"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_BLACKLIST,"The account has been in the black list"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_BUSY,"Resources are not sufficient. System is busy now"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_CONNECT,"Time out. Please check network and try again"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_NETWORK,"Network connection failed"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_SUBCONNECT,"Successfully logged in the device but can not create video channel. Please check network connection"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_MAXCONNECT,"exceed the max connect number"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_PROTOCOL3_ONLY,"protocol 3 support"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_UKEY_LOST,"There is no USB or USB info error"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_NO_AUTHORIZED,"Client-end IP address has no right to login"},
            {DhStruct.EM_ErrorCode.NET_LOGIN_ERROR_USER_OR_PASSOWRD,"user or password error"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SOUND_ON_ERROR,"Error occurs when Render library open audio"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SOUND_OFF_ERROR,"Error occurs when Render library close audio"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SET_VOLUME_ERROR,"Error occurs when Render library control volume"},
            {DhStruct.EM_ErrorCode.NET_RENDER_ADJUST_ERROR,"Error occurs when Render library set video parameter"},
            {DhStruct.EM_ErrorCode.NET_RENDER_PAUSE_ERROR,"Error occurs when Render library pause play"},
            {DhStruct.EM_ErrorCode.NET_RENDER_SNAP_ERROR,"Render library snapshot error"},
            {DhStruct.EM_ErrorCode.NET_RENDER_STEP_ERROR,"Render library stepper error"},
            {DhStruct.EM_ErrorCode.NET_RENDER_FRAMERATE_ERROR,"Error occurs when Render library set frame rate"},
            {DhStruct.EM_ErrorCode.NET_RENDER_DISPLAYREGION_ERROR,"Error occurs when Render lib setting show region"},
            {DhStruct.EM_ErrorCode.NET_RENDER_GETOSDTIME_ERROR,"An error occurred when Render library getting current play time"},
            {DhStruct.EM_ErrorCode.NET_GROUP_EXIST,"Group name has been existed"},
            {DhStruct.EM_ErrorCode.NET_GROUP_NOEXIST,"The group name does not exist"},
            {DhStruct.EM_ErrorCode.NET_GROUP_RIGHTOVER,"The group right exceeds the right list"},
            {DhStruct.EM_ErrorCode.NET_GROUP_HAVEUSER,"The group can not be removed since there is user in it"},
            {DhStruct.EM_ErrorCode.NET_GROUP_RIGHTUSE,"The user has used one of the group right. It can not be removed"},
            {DhStruct.EM_ErrorCode.NET_GROUP_SAMENAME,"New group name has been existed"},
            {DhStruct.EM_ErrorCode.NET_USER_EXIST,"The user name has been existed"},
            {DhStruct.EM_ErrorCode.NET_USER_NOEXIST,"The account does not exist"},
            {DhStruct.EM_ErrorCode.NET_USER_RIGHTOVER,"User right exceeds the group right"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD,"Reserved account. It does not allow to be modified"},
            {DhStruct.EM_ErrorCode.NET_USER_FLASEPWD,"password is not correct"},
            {DhStruct.EM_ErrorCode.NET_USER_NOMATCHING,"Password is invalid"},
            {DhStruct.EM_ErrorCode.NET_USER_INUSE,"account in use"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_ETHERNET,"Failed to get network card setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WLAN,"Failed to get wireless network information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_WLANDEV,"Failed to get wireless network device"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_REGISTER,"Failed to get actively registration parameter"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_CAMERA,"Failed to get camera property"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_INFRARED,"Failed to get IR alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SOUNDALARM,"Failed to get audio alarm setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_STORAGE,"Failed to get storage position"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MAIL,"Failed to get mail setup"},
            {DhStruct.EM_ErrorCode.NET_CONFIG_DEVBUSY,"Can not set right now"},
            {DhStruct.EM_ErrorCode.NET_CONFIG_DATAILLEGAL,"The configuration setup data are illegal"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DST,"Failed to get DST setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_DST,"Failed to set DST"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEO_OSD,"Failed to get video osd setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEO_OSD,"Failed to set video osd"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_GPRSCDMA,"Failed to get CDMA\\GPRS configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_GPRSCDMA,"Failed to set CDMA\\GPRS configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_IPFILTER,"Failed to get IP Filter configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_IPFILTER,"Failed to set IP Filter configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_TALKENCODE,"Failed to get Talk Encode configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_TALKENCODE,"Failed to set Talk Encode configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_RECORDLEN,"Failed to get The length of the video package configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_RECORDLEN,"Failed to set The length of the video package configuration"},
            {DhStruct.EM_ErrorCode.NET_DONT_SUPPORT_SUBAREA,"Not support Network hard disk partition"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_AUTOREGSERVER,"Failed to get the register server information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CONTROL_AUTOREGISTER,"Failed to control actively registration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DISCONNECT_AUTOREGISTER,"Failed to disconnect actively registration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MMS,"Failed to get mms configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_MMS,"Failed to set mms configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_SMSACTIVATION,"Failed to get SMS configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_SMSACTIVATION,"Failed to set SMS configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_DIALINACTIVATION,"Failed to get activation of a wireless connection"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_DIALINACTIVATION,"Failed to set activation of a wireless connection"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_VIDEOOUT,"Failed to get the parameter of video output"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_VIDEOOUT,"Failed to set the configuration of video output"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_OSDENABLE,"Failed to get osd overlay enabling"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_OSDENABLE,"Failed to set OSD overlay enabling"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_ENCODERINFO,"Failed to set digital input configuration of front encoders"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_TVADJUST,"Failed to get TV adjust configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_TVADJUST,"Failed to set TV adjust configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CONNECT_FAILED,"Failed to request to establish a connection"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_BURNFILE,"Failed to request to upload burn files"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SNIFFER_GETCFG,"Failed to get capture configuration information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SNIFFER_SETCFG,"Failed to set capture configuration information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DOWNLOADRATE_GETCFG,"Failed to get download restrictions information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DOWNLOADRATE_SETCFG,"Failed to set download restrictions information"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SEARCH_TRANSCOM,"Failed to query serial port parameters"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_POINT,"Failed to get the preset info"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_POINT,"Failed to set the preset info"},
            {DhStruct.EM_ErrorCode.NET_SDK_LOGOUT_ERROR,"SDK log out the device abnormally"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_VEHICLE_CFG,"Failed to get vehicle configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_VEHICLE_CFG,"Failed to set vehicle configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_ATM_OVERLAY_CFG,"Failed to get ATM overlay configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_ATM_OVERLAY_CFG,"Failed to set ATM overlay configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_ATM_OVERLAY_ABILITY,"Failed to get ATM overlay ability"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DECODER_TOUR_CFG,"Failed to get decoder tour configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_DECODER_TOUR_CFG,"Failed to set decoder tour configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CTRL_DECODER_TOUR,"Failed to control decoder tour"},
            {DhStruct.EM_ErrorCode.NET_GROUP_OVERSUPPORTNUM,"Beyond the device supports for the largest number of user groups"},
            {DhStruct.EM_ErrorCode.NET_USER_OVERSUPPORTNUM,"Beyond the device supports for the largest number of users"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_SIP_CFG,"Failed to get SIP configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_SIP_CFG,"Failed to set SIP configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_SIP_ABILITY,"Failed to get SIP capability"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_WIFI_AP_CFG,"Failed to get 'WIFI ap' configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_WIFI_AP_CFG,"Failed to set 'WIFI ap' configuration"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DECODE_POLICY,"Failed to get decode policy"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_DECODE_POLICY,"Failed to set decode policy"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_REJECT,"refuse talk"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_OPENED,"talk has opened by other client"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_RESOURCE_CONFLICIT,"resource conflict"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_UNSUPPORTED_ENCODE,"unsupported encode type"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_RIGHTLESS,"no right"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TALK_FAILED,"request failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_MACHINE_CFG,"Failed to get device relative config"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_MACHINE_CFG,"Failed to set device relative config"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_DATA_FAILED,"get data failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_MAC_VALIDATE_FAILED,"MAC validate failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_INSTANCE,"Failed to get server instance"},
            {DhStruct.EM_ErrorCode.NET_ERROR_JSON_REQUEST,"Generated json string is error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_JSON_RESPONSE,"The responding json string is error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_VERSION_HIGHER,"The protocol version is lower than current version"},
            {DhStruct.EM_ErrorCode.NET_SPARE_NO_CAPACITY,"Hotspare disk operation failed. The capacity is low"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SOURCE_IN_USE,"Display source is used by other output"},
            {DhStruct.EM_ErrorCode.NET_ERROR_REAVE,"advanced users grab low-level user resource"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NETFORBID,"net forbid"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_MACFILTER,"get MAC filter configuration error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_MACFILTER,"set MAC filter configuration error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GETCFG_IPMACFILTER,"get IP/MAC filter configuration error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SETCFG_IPMACFILTER,"set IP/MAC filter configuration error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OPERATION_OVERTIME,"operation over time"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SENIOR_VALIDATE_FAILED,"senior validation failure"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_ID_NOT_EXIST,"device ID is not exist"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNSUPPORTED,"unsupport operation"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_DLLLOAD,"proxy dll load error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_ILLEGAL_PARAM,"proxy user parameter is not legal"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_INVALID_HANDLE,"handle invalid"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_LOGIN_DEVICE_ERROR,"login device error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PROXY_START_SERVER_ERROR,"start proxy server error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SPEAK_FAILED,"request speak failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NOT_SUPPORT_F6,"unsupport F6"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CD_UNREADY,"CD is not ready"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DIR_NOT_EXIST,"Directory does not exist"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNSUPPORTED_SPLIT_MODE,"The device does not support the segmentation model"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OPEN_WND_PARAM,"Open the window parameter is illegal"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_WND_COUNT,"Open the window more than limit"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UNMATCHED_REQUEST,"Request command with the current pattern don't match"},
            {DhStruct.EM_ErrorCode.NET_RENDER_ENABLELARGEPICADJUSTMENT_ERROR,"Render Library to enable high-definition image internal adjustment strategy error"},
            {DhStruct.EM_ErrorCode.NET_ERROR_UPGRADE_FAILED,"Upgrade equipment failure"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_TARGET_DEVICE,"Can't find the target device"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_VERIFY_DEVICE,"Can't find the verify device"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CASCADE_RIGHTLESS,"No cascade permissions"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOW_PRIORITY,"low priority"},
            {DhStruct.EM_ErrorCode.NET_ERROR_REMOTE_REQUEST_TIMEOUT,"The remote device request timeout"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_INPUT_SOURCE,"Input source beyond maximum route restrictions"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SET_LOG_PRINT_INFO,"Failed to set log print"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PARAM_DWSIZE_ERROR,"'dwSize' is not initialized in input param"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LIMITED_MONITORWALL_COUNT,"TV wall exceed limit"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PART_PROCESS_FAILED,"Fail to execute part of the process"},
            {DhStruct.EM_ErrorCode.NET_ERROR_TARGET_NOT_SUPPORT,"Fail to transmit due to not supported by target"},
            {DhStruct.EM_ErrorCode.NET_ERROR_VISITE_FILE,"Access to the file failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_STATUS_BUSY,"Device busy"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD_NOT_AUTHORIZED,"Fail to change the password"},
            {DhStruct.EM_ErrorCode.NET_USER_PWD_NOT_STRONG,"Password strength is not enough"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_SUCH_CONFIG,"No corresponding setup"},
            {DhStruct.EM_ErrorCode.NET_ERROR_AUDIO_RECORD_FAILED,"Failed to record audio"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SEND_DATA_FAILED,"Failed to send out data"},
            {DhStruct.EM_ErrorCode.NET_ERROR_OBSOLESCENT_INTERFACE,"Abandoned port"},
            {DhStruct.EM_ErrorCode.NET_ERROR_INSUFFICIENT_INTERAL_BUF,"Internal buffer is not sufficient"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NEED_ENCRYPTION_PASSWORD,"verify password when changing device IP"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NOSUPPORT_RECORD,"device not support the record"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SERIALIZE_ERROR,"Failed to serialize data"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DESERIALIZE_ERROR,"Failed to deserialize data"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_EXISTED,"the wireless id is already existed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_LIMIT,"the wireless id limited"},
            {DhStruct.EM_ErrorCode.NET_ERROR_LOWRATEWPAN_ID_ABNORMAL,"add the wireless id abnormaly"},
            {DhStruct.EM_ErrorCode.NET_ERROR_ENCRYPT, "encrypt data fail"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PWD_ILLEGAL, "new password illegal"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DEVICE_ALREADY_INIT, "device is already init"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SECURITY_CODE, "security code check out fail"},
            {DhStruct.EM_ErrorCode.NET_ERROR_SECURITY_CODE_TIMEOUT, "security code out of time"},
            {DhStruct.EM_ErrorCode.NET_ERROR_GET_PWD_SPECI, "get passwd specification fail"},
            {DhStruct.EM_ErrorCode.NET_ERROR_NO_AUTHORITY_OF_OPERATION, "no authority of operation"},
            {DhStruct.EM_ErrorCode.NET_ERROR_DECRYPT, "decrypt data fail"},
            {DhStruct.EM_ErrorCode.NET_ERROR_2D_CODE, "2D code check out fail"},
            {DhStruct.EM_ErrorCode.NET_ERROR_INVALID_REQUEST, "invalid request"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PWD_RESET_DISABLE, "pwd reset disabled"},
            {DhStruct.EM_ErrorCode.NET_ERROR_PLAY_PRIVATE_DATA, "failed to display private data,such as rule box"},
            {DhStruct.EM_ErrorCode.NET_ERROR_ROBOT_OPERATE_FAILED, "robot operate failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_CHANNEL_ALREADY_OPENED, "channel has already been opened"},

            {DhStruct.EM_ErrorCode.ERR_INTERNAL_INVALID_CHANNEL,"invaild channel"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_REOPEN_CHANNEL,"reopen channel failed"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_SEND_DATA,"send data failed"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_CREATE_SOCKET,"create socket failed"},
            {DhStruct.EM_ErrorCode.ERR_INTERNAL_LISTEN_FAILED,"Start listen failed"},
            {DhStruct.EM_ErrorCode.NET_ERROR_FACE_RECOGNITION_SERVER_GROUP_ID_EXCEED, "face recognition server group id exceed " },
            {DhStruct.EM_ErrorCode.ERR_NOT_SUPPORT_HIGHLEVEL_SECURITY_LOGIN, "device not support high level security login" },
        };

        #endregion

        /// <summary>
        /// event data callback
        /// 事件数据回调函数
        /// </summary>
        /// <param name="lAnalyzerHandle">analyzerHandle:RealLoadPicture returns value 事件句柄</param>
        /// <param name="dwEventType">event type,see EM_EVENT_IVS_TYPE 事件类型</param>
        /// <param name="pEventInfo">event information 事件信息</param>
        /// <param name="pBuffer">picture buffer 数据缓存</param>
        /// <param name="dwBufSize">picture buffer size 数据缓存大小</param>
        /// <param name="dwUser">user data from RealLoadPicture function 用户数据</param>
        /// <param name="nSequence">means status of the same uploaded image, when it is 0, it appears first time.When it is 2, it appears last time or appears once.When it is 1, it will appear again. 序列号</param>
        /// <param name="reserved">int nState = (int) reserved means current callback data status;when it is 1, it means current data is real time and current callback data is offline;when it is 2,it means offline data send structure 保留</param>
        /// <param name="path">图片保存地址</param>
        /// <returns>reserved 保留</returns>
        public delegate int fAnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved);

        /// <summary>
        /// subscribe video statistical summary
        /// 订阅视频统计摘要信息
        /// </summary>
        /// <param name="lLoginID">CLIENT_Login's return value 登陆ID</param>
        /// <param name="pInParam">in param 输入参数</param>
        /// <param name="pOutParam">out param, not useful, dwsize need assign too 输出参数</param>
        /// <param name="nWaitTime">Wait timeout, million second 等待时间</param>
        /// <returns>Attach Handle 订阅句柄</returns>
        public static IntPtr AttachVideoStatSummary(IntPtr lLoginID, ref DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM pInParam, ref DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM pOutParam, int nWaitTime)
        {
            IntPtr pRet = IntPtr.Zero;
            pRet = ImportDhSdk.CLIENT_AttachVideoStatSummary(lLoginID, ref pInParam, ref pOutParam, nWaitTime);
            NetGetLastError(pRet);
            return pRet;
        }

        /// <summary>
        /// network disconnection callback function original shape
        /// 断线回调函数
        /// </summary>
        /// <param name="lLoginID">user LoginID:Login's returns value 登陆ID</param>
        /// <param name="pchDVRIP">device IP 设备IP</param>
        /// <param name="nDVRPort">device prot 设备端口</param>
        /// <param name="dwUser">user data from Init function 用户数据</param>
        public delegate void fDisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser);

        /// <summary>
        ///  video statistical summary callback function type
        ///  视频统计摘要信息回调函数
        /// </summary>
        /// <param name="lAttachHandle">return value of AttachVideoStatSummary AttachVideoStatSummary返回值</param>
        /// <param name="pBuf">pointer to NET_VIDEOSTAT_SUMMARY 数据缓存</param>
        /// <param name="dwBufLen">buffer length 数据缓存大小</param>
        /// <param name="dwUser">user data of AttachVideoStatSummary 用户数据</param>
        public delegate void fVideoStatSumCallBack(IntPtr lAttachHandle, IntPtr pBuf, uint dwBufLen, IntPtr dwUser);

        /// <summary>
        /// initialize SDK,can only be called once.Must be called before others SDK function,otherwise others SDK function will fail.
        /// 初始化SDK，只能被调用一次，必须在别的SDK接口函数调用之前调用。
        /// </summary>
        /// <param name="cbDisConnect">disconnect the callback function, see the delegate 断线回调函数</param>
        /// <param name="dwUser">user data, there is no data, please use IntPtr.Zero 用户数据</param>
        /// <param name="initParam">initialization parameter,can input null SDK初始化参数</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool Init(fDisConnectCallBack cbDisConnect, IntPtr dwUser, DhStruct.NETSDK_INIT_PARAM? stuInitParam)
        {
            bool result = false;
            IntPtr lpInitParam = IntPtr.Zero;
            try
            {
                if (null != stuInitParam)
                {
                    lpInitParam = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DhStruct.NETSDK_INIT_PARAM)));
                    Marshal.StructureToPtr(stuInitParam, lpInitParam, true);
                }
                result = ImportDhSdk.CLIENT_InitEx(cbDisConnect, dwUser, lpInitParam);
                NetGetLastError(result);
            }
            finally
            {
                Marshal.FreeHGlobal(lpInitParam);
            }
            return result;
        }

        /// <summary>
        /// log off device
        /// 登出设备
        /// </summary>
        /// <param name="lLoginID">user LoginID:Login's returns value，登陆ID,Login返回值</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool Logout(IntPtr lLoginID)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_Logout(lLoginID);
            NetGetLastError(result);
            return result;
        }

        public static bool RenderPrivateData(IntPtr lPlayHandle, bool bTrue)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_RenderPrivateData(lPlayHandle, bTrue);
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// judge the SDK function is failed or successful
        /// 判断SDK接口函数调用是否成功
        /// </summary>
        /// <typeparam name="T">value type 接口函数返回值类型</typeparam>
        /// <param name="value">the value is SDK function returns value,the value must be value type 接口函数返回值</param>
        private static void NetGetLastError<T>(T value)
            where T : struct
        {
            object temp = value;
            bool isGetLastError = false;
            if (value is IntPtr)
            {
                IntPtr tempValue = (IntPtr)temp;
                if (IntPtr.Zero == tempValue)
                {
                    isGetLastError = true;
                }
            }
            else if (value is int)
            {
                int tempValue = (int)temp;
                if (0 > tempValue)
                {
                    isGetLastError = true;
                }
            }
            else if (value is bool)
            {
                bool tempValue = (bool)temp;
                if (false == tempValue)
                {
                    isGetLastError = true;
                }
            }
            else
            {
                return;
            }
            if (isGetLastError)
            {
                if (!m_IsThrowErrorMessage)
                {
                    return;
                }
                int error = ImportDhSdk.CLIENT_GetLastError();
                if (0 != error)
                {

                    string errorMessage = GetLastErrorMessage((DhStruct.EM_ErrorCode)error);
                    throw new NETClientExcetion(error, errorMessage);

                }
            }
        }

        /// <summary>
        /// error code convert to error message 
        /// 错误码转成错误信息
        /// </summary>
        /// <param name="errorCode">SDK error code SDK错误码</param>
        /// <returns>error message description 错误信息描述</returns>
        private static string GetLastErrorMessage(DhStruct.EM_ErrorCode errorCode)
        {
            string result = string.Empty;
            switch (System.Globalization.CultureInfo.CurrentCulture.LCID)
            {
                case 0x00804:
                    zh_cn_String.TryGetValue(errorCode, out result);
                    break;
                default:
                    en_us_String.TryGetValue(errorCode, out result);
                    break;
            }
            if (null == result)
            {
                result = errorCode.ToString("X");
            }
            return result;
        }

        /// <summary>
        /// stop real time monitoring
        /// 关闭实时监视
        /// </summary>
        /// <param name="lRealHandle">monitor handle StartRealPlay returns value 监视ID StartRealPlay返回值</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool StopRealPlay(IntPtr lRealHandle)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_StopRealPlayEx(lRealHandle);
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// unsubscribe event
        /// 取消订阅事件
        /// </summary>
        /// <param name="lAnalyzerHandle">analyzerHandle:RealLoadPicture returns value RealLoadPicture返回值</param>
        /// <returns>failed return false, successful return true 失败返回false 成功返回true</returns>
        public static bool StopLoadPic(IntPtr lAnalyzerHandle)
        {
            bool result = false;
            result = ImportDhSdk.CLIENT_StopLoadPic(lAnalyzerHandle);
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// login device with high level security
        /// 高安全级别登陆
        /// </summary>
        /// <param name="pchDVRIP">device IP 设备IP</param>
        /// <param name="wDVRPort">device port 设备端口</param>
        /// <param name="pchUserName">username 用户名</param>
        /// <param name="pchPassword">password 密码</param>
        /// <param name="emSpecCap">device supported capacity,when the value is EM_LOGIN_SPAC_CAP_TYPE.SERVER_CONN means active listen mode user login(mobile dvr login) 登陆方式</param>
        /// <param name="pCapParam">nSpecCap compensation parameter，nSpecCap = EM_LOGIN_SPAC_CAP_TYPE.SERVER_CONN，pCapParam fill in device serial number string(mobile dvr login) emSpecCap参数，只有当 EM_LOGIN_SPAC_CAP_TYPE.SERVER_CONN有效</param>
        /// <param name="deviceInfo">device information，for output parmaeter 输出的设备信息</param>
        /// <returns>failed return 0,successful return LoginID,after successful login, device Operation may be via this this value(device handle)corresponding to corresponding device.失败返回0，成功返回大于O的值</returns>
        public static IntPtr LoginWithHighLevelSecurity(string pchDVRIP, ushort wDVRPort, string pchUserName, string pchPassword, DhStruct.EM_LOGIN_SPAC_CAP_TYPE emSpecCap, IntPtr pCapParam, ref DhStruct.NET_DEVICEINFO_Ex deviceInfo)
        {
            IntPtr result = IntPtr.Zero;
            DhStruct.NET_IN_LOGIN_WITH_HIGHLEVEL_SECURITY stuInParam = new DhStruct.NET_IN_LOGIN_WITH_HIGHLEVEL_SECURITY();
            stuInParam.dwSize = (uint)Marshal.SizeOf(stuInParam);
            stuInParam.szIP = pchDVRIP;
            stuInParam.nPort = wDVRPort;
            stuInParam.szUserName = pchUserName;
            stuInParam.szPassword = pchPassword;
            stuInParam.emSpecCap = emSpecCap;
            stuInParam.pCapParam = pCapParam;
            DhStruct.NET_OUT_LOGIN_WITH_HIGHLEVEL_SECURITY stuOutParam = new DhStruct.NET_OUT_LOGIN_WITH_HIGHLEVEL_SECURITY();
            stuOutParam.dwSize = (uint)Marshal.SizeOf(stuOutParam);
            result = ImportDhSdk.CLIENT_LoginWithHighLevelSecurity(ref stuInParam, ref stuOutParam);
            deviceInfo = stuOutParam.stuDeviceInfo;
            NetGetLastError(result);
            return result;
        }

        /// <summary>
        /// get last error message
        /// 获取错误信息
        /// </summary>
        /// <returns>error message 错误信息</returns>
        public static string GetLastError()
        {
            string reslut = null;
            int error = ImportDhSdk.CLIENT_GetLastError();
            if (0 != error)
            {

                reslut = GetLastErrorMessage((DhStruct.EM_ErrorCode)error);
            }
            return reslut;
        }

        /// <summary>
        /// subscribe event
        /// 订阅事件
        /// </summary>
        /// <param name="lLoginID">loginID:login returns value 登陆ID</param>
        /// <param name="nChannelID">channel id 通道号</param>
        /// <param name="dwAlarmType">event type see EM_EVENT_IVS_TYPE 事件类型</param>
        /// <param name="bNeedPicFile">subscribe image file or not,ture-yes,return intelligent image info during callback function,false not return intelligent image info during callback function 是否需要图片</param>
        /// <param name="cbAnalyzerData">intelligent data analysis callback 事件回调函数</param>
        /// <param name="dwUser">user data 用户数据</param>
        /// <param name="reserved">reserved 保留参数</param>
        /// <returns>failed return 0, successful return the analyzerHandle</returns>
        public static IntPtr RealLoadPicture(IntPtr lLoginID, int nChannelID, uint dwAlarmType, bool bNeedPicFile, fAnalyzerDataCallBack cbAnalyzerData, IntPtr dwUser, IntPtr reserved)
        {
            IntPtr result = IntPtr.Zero;
            result = ImportDhSdk.CLIENT_RealLoadPictureEx(lLoginID, nChannelID, dwAlarmType, bNeedPicFile, cbAnalyzerData, dwUser, reserved);
            NetGetLastError(result);
            return result;
        }
    }

    /// <summary>
    /// throw SDK exception Class
    /// SDK异常类
    /// </summary>
    public class NETClientExcetion : Exception
    {
        /// <summary>
        /// SDK error code property
        /// SDK错误码属性
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// SDK error message property
        /// SDK错误信息属性
        /// </summary>
        new public string Message { get; private set; }

        /// <summary>
        /// construct function.
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">SDK error code number</param>
        /// <param name="message">SDK error message</param>
        public NETClientExcetion(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }


    public struct NET_SNAP_PARAMS
    {
        /// <summary>
        /// Snapshot channel
        /// 抓图的通道
        /// </summary>
        public uint Channel;
        /// <summary>
        /// Image quality:level 1 to level 6
        /// 画质；1~6
        /// </summary>
        public uint Quality;
        /// <summary>
        /// Video size;0:QCIF,1:CIF,2:D1
        /// 画面大小；0：QCIF,1：CIF,2：D1
        /// </summary>
        public uint ImageSize;
        /// <summary>
        /// Snapshot mode;0:request one frame,1:send out requestion regularly,2: Request consecutively
        /// 抓图模式；0xFFFFFFFF:表示停止抓图, 0：表示请求一帧, 1：表示定时发送请求, 2：表示连续请求
        /// </summary>
        public uint mode;
        /// <summary>
        /// Time unit is second.If mode=1, it means send out requestion regularly. The time is valid.
        /// 时间单位秒；若mode=1表示定时发送请求时只有部分特殊设备(如：车载设备)支持通过该字段实现定时抓图时间间隔的配置建议通过 CFG_CMD_ENCODE 配置的stuSnapFormat[nSnapMode].stuVideoFormat.nFrameRate字段实现相关功能
        /// </summary>
        public uint InterSnap;
        /// <summary>
        /// Request serial number
        /// 请求序列号，有效值范围 0~65535，超过范围会被截断为 unsigned short
        /// </summary>
        public uint CmdSerial;
        /// <summary>
        /// Reserved
        /// 保留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] Reserved;
    }


}
