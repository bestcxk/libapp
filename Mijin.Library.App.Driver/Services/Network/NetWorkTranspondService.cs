using System.Collections.Generic;
using System.Linq;
using Bing.Extensions;
using Bing.Helpers;
using ImTools;
using Mijin.Library.App.Common.Domain;
using Mijin.Library.App.Common.Helper;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Services.Network
{
    public class NetWorkTranspondService : INetWorkTranspondService
    {
        public List<NetWorkTranspond> Transponds { get; set; } = new List<NetWorkTranspond>();

        const int startPort = 50000;


        public string GetVisitUrl(string url)
        {
            if (url.IsEmpty() || !url.Contains("http")) return url;


            var host_port = url.Split("//")[1].Split("/")[0];

            var tran = Transponds.FirstOrDefault(p => p.ToTargetString() == host_port);

            if (tran == null)

                return url;

            return url.Replace(tran.ToTargetString(), @$"127.0.0.1:{tran.localPort}");
        }

        public void StartOrUpdateListen(ClientSettings settings)
        {
            var urls = new List<string>()
            {
                settings.LibraryManageUrl,
                settings.ReaderActionUrl,
                settings.NoSelectOpenUrl,
                settings.LabelConvertUrl,
                settings.DoorControllerUrl,
            }.Where(p => !p.IsEmpty() && !p.Contains("file://") && !p.Contains(@$".\")).ToList();

            // 关闭已经取消的监听
            foreach (var netWorkTranspond in Transponds)
            {
                if (urls.Contains(netWorkTranspond.ToTargetString()))
                {
                    Transponds.Remove(netWorkTranspond);
                    netWorkTranspond.Dispose();
                }
            }

            for (int i = 0; i < urls.Count; i++)
            {
                var originUrl = urls[i];
                var url = Url.GetMainDomain(urls[i]).Replace("https://", "").Replace("http://", "").Split("/")[0];

                if (Transponds.Any(p => p.ToTargetString() == url))
                    continue;

                var splits = url.Split(":");
                var host = splits[0];

                var port = splits.Count() == 2 ? splits[1].ToInt() : 80;

                var transpond = new NetWorkTranspond(host, port);
                transpond.OriginHostPort = originUrl.Split("//")[1].Split("/").First();
                AddTranspond(transpond);
            }
        }

        private void AddTranspond(NetWorkTranspond transpond)
        {
            if (transpond.TargetHost == "localhost" || transpond.TargetHost == "127.0.0.1") return;
            transpond.localPort = !Transponds.IsEmpty() ? Transponds.Max(t => t.localPort) + 1 : startPort;
            var usingPorts = NetWorkHelper.GetUsingPorts().ToList();

            if (Transponds.Any(p=> p.TargetHost == transpond.TargetHost && p.TargetPort == transpond.TargetPort))
                return;

            while (usingPorts.Contains(transpond.localPort))
            {
                transpond.localPort++;
            }

            transpond.Start();

            Transponds.Add(transpond);
        }

        private void RemoveAndStopTranspond(string localIp, int localPort)
        {
            var transpond = Transponds.FirstOrDefault(t => t.localIp == localIp && t.localPort == localPort);
            Transponds.Remove(transpond);
            transpond.Stop(); ;
        }

        private void ClearAndStop()
        {
            foreach (var transpond in Transponds)
            {
                Transponds.Remove(transpond);
                transpond.Dispose();
            }
        }
    }
}
