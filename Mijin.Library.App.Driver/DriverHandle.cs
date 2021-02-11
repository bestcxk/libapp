using Mijin.Library.App.Driver.LibrarySIP2.Interface;
using Mijin.Library.App.Driver.Lock.Interface;
using Mijin.Library.App.Driver.PosPrint.Interface;
using Mijin.Library.App.Driver.Reader;
using Mijin.Library.App.Driver.RFID;
using Mijin.Library.App.Driver.RFID.Model;
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

        public event Action<List<bool>> lockStatusEvent;
        public event Action<LabelInfo> OnTagEpcLog;
        public event Action<PeopleInOut> OnPeopleInOut;

        /// <summary>
        /// 获取操作方法的实例
        /// </summary>
        /// <param name="objInterfaceName">接口名</param>
        /// <returns>返回实例</returns>
        private object GetActionObj(string objInterfaceName) =>
            objInterfaceName switch
            {
                "ISIP2Client" => _sIP2Client,
                "ICabinetLock" => _cabinetLock,
                "IPosPrint" => _posPrint,
                "IdentityReader" => _identityReader,
                "IHFReader" => _hFReader,
                "IRfid" => _rfid,
                "IRfidDoor" => _rfidDoor,
                _ => null
            };

        #region 构造函数

        public DriverHandle(ISIP2Client sIP2Client, ICabinetLock cabinetLock, IPosPrint posPrint, IdentityReader identityReader, IHFReader HFReader, IRfid rfid, IRfidDoor rfidDoor)
        {

            _sIP2Client = sIP2Client;
            _cabinetLock = cabinetLock;
            _posPrint = posPrint;
            _identityReader = identityReader;
            _hFReader = HFReader;
            _rfid = rfid;
            _rfidDoor = rfidDoor;

            _cabinetLock.lockStatusEvent += lockStatusEvent;
            _rfid.OnTagEpcLog += OnTagEpcLog;

            _rfidDoor.OnPeopleInOut += OnPeopleInOut;
            _rfidDoor.OnTagEpcLog += OnTagEpcLog;

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

            // 通过类名(接口名)获取到实例
            var acionObj = GetActionObj(cls);

            // 匹配不到执行类
            if (acionObj == null)
            {
                return new MessageModel<object>()
                {
                    msg = "未匹配到执行的类名"
                };
            }

            // 反射执行完方法后转换成 WebMessageModel 类
            return acionObj.GetType().GetMethod(mthod, parametersTypes).Invoke(acionObj, parameters).JsonMapTo<MessageModel<object>>();
        }
    }
}
