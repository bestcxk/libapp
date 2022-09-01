using GDotnet.Reader.Api.DAL;
using IsUtil;
using IsUtil.Maps;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;

namespace Mijin.Library.App.Driver
{
    public class MultiGrfid : IMultiGrfid
    {
        public List<MultiGrfidProp> rfids = new List<MultiGrfidProp>();
        public event Action<WebViewSendModel<LabelInfo>> OnReadUHFLabel;

        ~MultiGrfid()
        {
            rfids?.ForEach(r =>
            {
                r.Rfid.Dispose();
            });
        }

        public MessageModel<string> Connected()
        {
            return Stop();
        }

        public MessageModel<bool> ConnectRfids(List<MultiGrfidProp> props)
        {
            props = props?.Where(p => rfids.All(r => r.ConnectStr != p.ConnectStr)).ToList();
            for (int i = 0; i < props.Count; i++)
            {
                var item = props[i];
                item.Rfid = new GRfid(null);
                var res = item.Rfid.Connect(item.ConnectStr.ToLower().Contains("com") ? "232" : "tcp", item.ConnectStr,
                    1500);
                if (!res.success)
                {
                    return new MessageModel<bool>()
                    {
                        msg = @$"{item.ConnectStr} 连接失败"
                    };
                }

                item.AntCount = item.Rfid.GetPower().response.Count; // 1: 8 ,2:8
                item.AntStartIndex = rfids.IsEmpty() ? 1 : rfids.Last().AntCount + rfids.Last().AntStartIndex; // 1: 1,2:9
                item.Rfid.OnReadUHFLabel += (labelInfo) =>
                {
                    labelInfo.response.AntId += (byte) (item.AntStartIndex - 1);
                    OnReadUHFLabel?.Invoke(labelInfo);
                };


                rfids.Add(item);
            }

            return new MessageModel<bool>()
            {
                success = true,
                msg = "连接成功"
            };
        }

        public MessageModel<bool> ConnectRfids(string propStr)
        {
            return ConnectRfids(Json.ToObject<List<MultiGrfidProp>>(propStr));
        }

        public MessageModel<bool> ReadByAntIdNoTid(List<string> antIdStrs)
        {
            var res = new MessageModel<bool>();
            if (rfids.IsEmpty())
            {
                res.msg = "操作失败，RFID队列内未包含任何连接";
                return res;
            }

            if (antIdStrs.IsEmpty())
            {
                res.msg = "天线AntId不可为空";
                return res;
            }

            var antIds = antIdStrs.Select(x => int.Parse(x)).ToList();

            foreach (var item in rfids)
            {
                var maxId = item.AntStartIndex + (item.AntCount - 1);

                var itemAntIds = antIds.Where(a => a >= item.AntStartIndex && a <= maxId).ToList();

                if (!itemAntIds.IsEmpty())
                {
                    var itemAntIdStr = itemAntIds.Select(antId => ((antId - item.AntStartIndex) + 1).ToString())
                        .ToList();
                    var readRes = item.Rfid.ReadByAntIdNoTid(itemAntIdStr);

                    if (!readRes.success)
                    {
                        Stop();
                        return readRes;
                    }
                }
            }

            res.msg = "操作成功";
            res.success = true;

            return res;
        }

        public MessageModel<bool> ReadByAntIdNoTid(string antIdStrs)
        {
            return ReadByAntIdNoTid(Json.ToObject<List<string>>(antIdStrs));
        }

        public MessageModel<string> SetAllAntPower(Int64 power)
        {
            var res = new MessageModel<string>();
            if (rfids.IsEmpty())
            {
                res.msg = "操作失败，RFID队列内未包含任何连接";
                return res;
            }

            res = Stop();

            if (!res.success) return res;

            foreach (var item in rfids)
            {
                var dic = item.Rfid.GetPower().response;

                foreach (var di in dic)
                {
                    dic[di.Key] = (byte) power;
                }

                var powerRes = item.Rfid.SetPower(dic);
                if (!powerRes.success)
                {
                    res.success = false;
                    res.msg = @$"设置{item.ConnectStr}中的天线失败";
                    return res;
                }
            }

            return res;
        }

        public MessageModel<bool> ReadByNoTid()
        {
            var res = new MessageModel<bool>();
            if (rfids.IsEmpty())
            {
                res.msg = "操作失败，RFID队列内未包含任何连接";
                return res;
            }

            foreach (var item in rfids)
            {
                var readRes = item.Rfid.ReadNoTid();
                if (!readRes.success)
                {
                    Stop();
                    return readRes;
                }
            }

            res.success = true;
            res.SetMsg();
            return res;
        }

        public MessageModel<string> Stop()
        {
            var res = new MessageModel<string>();
            if (rfids.IsEmpty())
            {
                res.msg = "操作失败，RFID队列内未包含任何连接";
                return res;
            }

            foreach (var item in rfids)
            {
                var stopRes = item.Rfid.Stop();
                if (!stopRes.success)
                {
                    res.msg = @$"停止{item.ConnectStr} 失败";
                    return res;
                }
            }

            res.success = true;
            res.msg = "操作成功";
            return res;
        }

        public MessageModel<string> Close()
        {
            foreach (var rfid in rfids)
            {
                rfid?.Rfid.Close();
            }

            rfids = new List<MultiGrfidProp>();
            return new MessageModel<string>()
            {
                success = true,
                msg = "操作成功"
            };
        }
    }

    public class MultiGrfidProp
    {
        public int AntCount { get; set; }
        public int AntStartIndex { get; set; }
        public string ConnectStr { get; set; }
        public IRfid Rfid { get; set; }
    }
}