using Mijin.Library.App.Driver.Drivers.Camera;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Util.Maps;

namespace Mijin.Library.App.Driver
{
    public class DriverHandle : IDriverHandle
    {
        private ISIP2Client _sIP2Client { get; }
        private ICabinetLock _cabinetLock { get; }
        private IPosPrint _posPrint { get; }
        private IdentityReader _identityReader { get; }
        private IHFReader _hFReader { get; }
        private IRfid _rfid { get; }
        private IRfidDoor _rfidDoor { get; }
        public IKeyboard _keyboard { get; }
        private ISystemFunc _systemFunc { get; }
        private ICamera _camera { get; }

        /// <summary>
        /// 锁孔板开/关 锁事件 false:未打开  true:打开
        /// </summary>
        public event Action<List<bool>> lockStatusEvent;

        // <summary>
        /// 读到标签事件
        /// </summary>
        public event Action<LabelInfo> OnTagEpcLog;

        /// <summary>
        /// 通道门进出事件
        /// </summary>
        public event Action<PeopleInOut> OnPeopleInOut;

        /// <summary>
        /// 摄像头获取图片事件
        /// </summary>
        public event Action<string> OnImgLog;

        #region 构造函数

        public DriverHandle(ISystemFunc systemFunc, ISIP2Client sIP2Client, ICabinetLock cabinetLock, IPosPrint posPrint, IdentityReader identityReader, IHFReader HFReader, IRfid rfid, IRfidDoor rfidDoor,IKeyboard keyboard,ICamera camera)
        {
            _systemFunc = systemFunc;
            _sIP2Client = sIP2Client;
            _cabinetLock = cabinetLock;
            _posPrint = posPrint;
            _identityReader = identityReader;
            _hFReader = HFReader;
            _rfid = rfid;
            _rfidDoor = rfidDoor;
            _keyboard = keyboard;
            _camera = camera;


            _cabinetLock.lockStatusEvent += lockStatusEvent;
            _rfid.OnTagEpcLog += OnTagEpcLog;

            _rfidDoor.OnPeopleInOut += OnPeopleInOut;
            _rfidDoor.OnTagEpcLog += OnTagEpcLog;

            _camera.OnImgLog += OnImgLog;

        }
        
        #endregion

        /// <summary>
        /// 调用Driver方法
        /// </summary>
        /// <param name="cls">接口名</param>
        /// <param name="mthod">方法名</param>
        /// <param name="parameters">传入参数</param>
        /// <returns>调用结果</returns>
        public MessageModel<object> Invoke(string cls, string mthod, object[] parameters)
        {
            // 获取执行参数的所有type
            Type[] parametersTypes = parameters == null ? new Type[] { } : parameters.Select(p => p.GetType()).ToArray();

            // 反射通过cls 获取 同名接口的 属性
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo propertyInfo = this.GetType().GetProperties(bindingFlags).Where(p => p.PropertyType.Name == cls).FirstOrDefault();

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

            // 反射执行完方法后转换成 WebMessageModel 类
            return acionInstance.GetType().GetMethod(mthod, parametersTypes).Invoke(acionInstance, parameters).JsonMapTo<MessageModel<object>>();
        }
    }
}
