using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IsUtil.Maps;

namespace Mijin.Library.App.Driver
{
    public class DriverHandle : IDriverHandle
    {
        private ICkDoorController _ckDoorController { get; }
        private ITrack _track { get; }
        private IMultiGrfid _multiGrfid { get; }
        private ISudo _sudo { get; }
        private IWjSIP2Client WjSIP2Client { get; }
        private IWenhuaSIP2Client _sIP2Client { get; }
        private ICabinetLock _cabinetLock { get; }
        private IPosPrint _posPrint { get; }
        private IdentityReader _identityReader { get; }
        private IHFReader _hFReader { get; }
        private IRfid _rfid { get; }
        private IKeyboard _keyboard { get; }
        private ISystemFunc _systemFunc { get; }
        private ICamera _camera { get; }
        private ICardSender _cardSender { get; }
        private IDoorController _doorController { get; }
        private IGRfidDoorController _gRfidDoorController { get; }
        private ITuChuangSIP2Client _tuChuangSIP2Client { get; }
        private IRRfid _rRfid { get; }
        private IQrCode _qrCode { get; }


        public static string[] BlackListLogMethod = { "ISystemFunc.SetLibrarySettings", "OnLockEvent" };

        string[] IDriverHandle.BlackListLogMethod
        {
            get => BlackListLogMethod;
        }

        /// <summary>
        /// 所有Driver模块的事件
        /// </summary>
        private event Action<object> OnAllDriverEvent;

        /// <summary>
        /// 所有Driver模块的事件 (访问器)
        /// </summary>
        public event Action<object> OnDriverEvent
        {
            add
            {
                //如果多次注册的话可能每次都要 执行 ReflectionRegiesterEvents,这里还不清楚，先留个坑,目前项目暂时这么写没问题

                // 第一次注册时
                if (OnAllDriverEvent == null)
                {
                    // 注册完后再注册Driver模块事件
                    OnAllDriverEvent += value;
                    ReflectionRegiesterEvents();
                }
                else
                {
                    OnAllDriverEvent += value;
                }
            }
            remove { OnAllDriverEvent -= value; }
        }


        #region 构造函数

        public DriverHandle(ISystemFunc systemFunc, IWenhuaSIP2Client sIP2Client, ICabinetLock cabinetLock,
            IPosPrint posPrint, IdentityReader identityReader, IHFReader HFReader, IRfid rfid, IKeyboard keyboard,
            ICamera camera, ICardSender cardSender, IDoorController doorController,
            IGRfidDoorController gRfidDoorController, ITuChuangSIP2Client tuChuangSIP2Client, IRRfid rRfid,
            IQrCode qrCode, ICkDoorController ckDoorController, ITrack track, IMultiGrfid multiGrfid, ISudo sudo, IWjSIP2Client wjSIP2Client)
        {
            _ckDoorController = ckDoorController;
            _track = track;
            _multiGrfid = multiGrfid;
            _sudo = sudo;
            WjSIP2Client = wjSIP2Client;
            _systemFunc = systemFunc;
            _sIP2Client = sIP2Client;
            _cabinetLock = cabinetLock;
            _posPrint = posPrint;
            _identityReader = identityReader;
            _hFReader = HFReader;
            _rfid = rfid;
            _keyboard = keyboard;
            _camera = camera;
            _cardSender = cardSender;
            _doorController = doorController;
            _gRfidDoorController = gRfidDoorController;
            _tuChuangSIP2Client = tuChuangSIP2Client;
            _rRfid = rRfid;
            _qrCode = qrCode;
        }

        #endregion

        /// <summary>
        /// 调用Driver方法
        /// </summary>
        /// <param name="cls">接口名</param>
        /// <param name="mthod">方法名</param>
        /// <param name="parameters">传入参数，无参数传null</param>
        /// <returns>调用结果</returns>
        public MessageModel<object> Invoke(string cls, string mthod, object[]? parameters)
        {
            // 获取执行参数的所有type
            Type[] parametersTypes =
                parameters == null ? new Type[] { } : parameters.Select(p => p.GetType()).ToArray();

            // 反射通过cls 获取 同名接口的 属性
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

            PropertyInfo propertyInfo = this.GetType().GetProperties(bindingFlags)
                .Where(p => p.PropertyType.Name == cls).FirstOrDefault();

            // 匹配不到执行类
            if (propertyInfo == null)
            {
                return new MessageModel<object>()
                {
                    msg = "未匹配到执行的类名"
                };
            }

            // 获取执行类实体
            var acionInstance = propertyInfo.GetValue(this);

            // 访问的是参数
            var proInfo = acionInstance.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(p => p.Name == mthod).FirstOrDefault();
            if (proInfo != null)
            {
                return new MessageModel<object>()
                {
                    devMsg = "因为该属性是参数，请不要判断 success，请判断 response",
                    success = true,
                    msg = "参数获取成功",
                    response = proInfo.GetValue(acionInstance)
                };
            }
            else // 访问的是方法
            {
                // 反射执行完方法后转换成 WebMessageModel 类
                var method = acionInstance.GetType().GetMethod(mthod, parametersTypes);

                if(method == null)
                {
                    return new MessageModel<object>()
                    {
                        msg = "未匹配到执行方法"
                    };
                }

                var invoke = method.Invoke(acionInstance, parameters);
                return invoke.JsonMapTo<MessageModel<object>>();
            }
        }

        /// <summary>
        /// 反射注册事件 , 需要在注册OnDriverEvent后调用
        /// </summary>
        private void ReflectionRegiesterEvents()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo[] propertyInfos = this.GetType().GetProperties(bindingFlags);

            foreach (var propertyInfo in propertyInfos)
            {
                // 获取执行类实体
                var instance = propertyInfo.GetValue(this);

                // 设置所有模块的事件传向 OnDriverEvent
                foreach (var eventInfo in instance.GetType().GetEvents())
                {
                    eventInfo.AddEventHandler(instance, OnAllDriverEvent);
                }
            }
        }
    }
}