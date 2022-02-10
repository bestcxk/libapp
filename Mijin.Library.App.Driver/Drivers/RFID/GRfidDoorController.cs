using Mijin.Library.App.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Maps;
using Mijin.Library.App.Model.Setting;

namespace Mijin.Library.App.Driver
{
    public class GRfidDoorController : IGRfidDoorController
    {
        private List<DoorControllerModel> doors { get; set; } = new List<DoorControllerModel>();
        public event Action<WebViewSendModel<LabelInfo>> OnDoorReadUHFLabel;
        public event Action<WebViewSendModel<PeopleInOut>> OnDoorPeopleInOut;

        #region 连接通道门(ConnectDoors)
        public MessageModel<string> ConnectDoors(Dictionary<int, DoorSetting> connectDic)
        {
            var res = new MessageModel<string>();
            var failActionKey = new List<int>();
            if (connectDic.IsEmpty())
            {
                res.msg = "连接数据不可为空";
                return res;
            }

            // 移除已连接成功的模块
            var connectedKeys = doors.Select(c => c.DoorKey).ToList();
            connectedKeys.ForEach(key =>
            {
                connectDic.Remove(key);
            });

            // 遍历连接通道门
            foreach (var item in connectDic)
            {
                DoorControllerModel door = new DoorControllerModel()
                {
                    DoorKey = item.Key,
                    ConnectStr = item.Value.Address,
                    RfidDoor = new GRfidDoor()
                };

                door.RfidDoor.gpiInIndex = item.Value.InGpi;
                door.RfidDoor.gpiOutIndex = item.Value.OutGpi;

                var connectRes = door.RfidDoor.Connect("tcp", door.ConnectStr);
                if (!connectRes.success)
                {
                    failActionKey.Add(door.DoorKey);
                }
                else
                {
                    doors.Add(door);
                    door.RfidDoor.OnReadUHFLabel += (obj) =>
                    {
                        // 通道门统一事件状态码
                        obj.status = 1001;
                        obj.method = nameof(OnDoorReadUHFLabel);
                        obj.response.DoorKey = door.DoorKey;
                        OnDoorReadUHFLabel?.Invoke(obj);
                    };
                    door.RfidDoor.OnPeopleInOut += (obj) =>
                    {
                        var inCount = doors.Sum(d => d.RfidDoor.inCount);
                        var outCount = doors.Sum(d => d.RfidDoor.outCount);
                        // 通道门统一事件状态码
                        obj.status = 1001;
                        obj.response.InCount = inCount;
                        obj.response.OutCount = outCount;
                        obj.method = "OnDoorPeopleInOut";
                        OnDoorPeopleInOut?.Invoke(obj);
                    };
                }
            }

            if (failActionKey.IsEmpty())
            {
                res.msg = @$"连接所有门禁成功，已连接 {doors.Count} 个门禁";
                res.success = true;
            }
            else
            {
                res.msg = @$"连接部分门禁失败，失败门禁包含：{string.Join(",", failActionKey)}";
            }

            return res;
        }

        public MessageModel<string> ConnectDoors(string connectDicStr)
        {
            return ConnectDoors(Json.ToObject<Dictionary<int, DoorSetting>>(connectDicStr));
        }
        #endregion

        #region 获取通道门信息(GetConnectDoorInfos)
        public MessageModel<List<object>> GetConnectDoorInfos()
        {
            var res = new MessageModel<List<object>>();
            if (doors.IsEmpty())
            {
                res.msg = "连接通道门数量为0";
                return res;
            }
            res.response = doors.Select(d => new { DoorKey = d.DoorKey, ConnectStr = d.ConnectStr }).ToList<object>();
            res.msg = @$"获取成功，已连接上{doors.Count}个通道门";
            res.success = true;
            return res;

        }
        #endregion

        #region 设置通道门功率(SetDoorsPower)
        public MessageModel<string> SetDoorsPower(Dictionary<int, Dictionary<byte, byte>> powers)
        {
            var res = new MessageModel<string>();
            var failActionKey = new List<int>();
            if (powers.IsEmpty())
            {
                res.msg = "参数不可为空";
                return res;
            }
            if (doors.IsEmpty())
            {
                res.msg = "通道门连接数为空";
                return res;
            }

            foreach (var item in powers)
            {
                var doorController = doors.Where(d => d.DoorKey == item.Key).FirstOrDefault();
                if (doorController.IsNull())
                {
                    continue;
                }
                var powerRes = doorController.RfidDoor.SetPower(item.Value);
                if (!powerRes.success)
                {
                    failActionKey.Add(item.Key);
                }
            }
            if (failActionKey.IsEmpty())
            {
                res.msg = @$"设置所有门禁功率，已连接 {doors.Count} 个门禁功率";
                res.success = true;
            }
            else
            {
                res.msg = @$"设置部分门禁功率失败，失败门禁包含：{string.Join(",", failActionKey)}";
            }

            return res;

        }
        public MessageModel<string> SetDoorsPower(string powersStr)
        {
            return SetDoorsPower(Json.ToObject<Dictionary<int, Dictionary<byte, byte>>>(powersStr));
        }
        #endregion

        #region 获取通道门功率(GetDoorsPower)

        public MessageModel<Dictionary<int, Dictionary<byte, byte>>> GetDoorsPower()
        {
            var res = new MessageModel<Dictionary<int, Dictionary<byte, byte>>>();
            var powers = new Dictionary<int, Dictionary<byte, byte>>();
            var failActionKey = new List<int>();
            if (doors.IsEmpty())
            {
                res.msg = "通道门连接数为空";
                return res;
            }

            foreach (var door in doors)
            {
                var powerRes = door.RfidDoor.GetPower();
                if (!powerRes.success)
                {
                    failActionKey.Add(door.DoorKey);
                }
                else
                {
                    powers.Add(door.DoorKey, powerRes.response);
                }
            }

            if (failActionKey.IsEmpty())
            {
                res.msg = @$"获取所有门禁功率成功";
                res.success = true;
            }
            else
            {
                res.msg = @$"获取部分门禁功率失败，失败门禁包含：{string.Join(",", failActionKey)}";
            }
            res.response = powers;
            return res;
        }
        #endregion

        #region 开始监听所有通道门进出(StartAllDoorWatch)

        public MessageModel<string> StartAllDoorWatch(bool clear = false)
        {
            var res = new MessageModel<string>();
            var powers = new Dictionary<int, Dictionary<byte, byte>>();
            var errorDoorKey = -1;
            if (doors.IsEmpty())
            {
                res.msg = "通道门连接数为空";
                return res;
            }

            foreach (var door in doors)
            {
                var doorRes = door.RfidDoor.StartWatchPeopleInOut(clear);
                if (!doorRes.success)
                {
                    errorDoorKey = door.DoorKey;
                    break;
                }
            }
            if (errorDoorKey > -1)
            {
                foreach (var door in doors)
                {
                    door.RfidDoor.StopWatchPeopleInOut();
                }
                res.msg = @$"启动到{errorDoorKey}通道门时失败，已停止所有通道门，请重试开启";
            }
            else
            {
                res.msg = "启动所有通道门成功";
                res.success = true;
            }
            return res;
        }
        #endregion

        #region 停止监听所有通道门进出(StopAllDoorWatch)

        public MessageModel<string> StopAllDoorWatch()
        {
            var res = new MessageModel<string>();
            var powers = new Dictionary<int, Dictionary<byte, byte>>();
            var failActionKey = new List<int>();
            if (doors.IsEmpty())
            {
                res.msg = "通道门连接数为空";
                return res;
            }

            foreach (var door in doors)
            {
                var doorRes = door.RfidDoor.StopWatchPeopleInOut();
                if (!doorRes.success)
                {
                    failActionKey.Add(door.DoorKey);
                }
            }
            if (failActionKey.IsEmpty())
            {
                res.msg = @$"停止所有门禁成功";
                res.success = true;
            }
            else
            {
                res.msg = @$"停止部分门禁功率失败，失败门禁包含：{string.Join(",", failActionKey)}";
            }
            return res;
        }
        #endregion

        public MessageModel<string> CloseAll()
        {
            var res = new MessageModel<string>();

            if(doors.IsEmpty())
            {
                res.msg = "未连接上任何通道门";
                return res;
            }

            foreach (var door in doors)
            {
                door.RfidDoor.Close();
            }

            doors.Clear();

            res.success = true;
            res.msg = "关闭所有通道门连接成功";
            return res;

        }

    }

    public class DoorControllerModel
    {
        public int DoorKey { get; set; }
        public string ConnectStr { get; set; }
        public IRfidDoor RfidDoor { get; set; }
    }
}
