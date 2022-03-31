using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;

namespace Mijin.Library.App.Common.Helper
{
    public class NetWorkHelper
    {

        public static IEnumerable<int> GetUsingPorts() => IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            .Select(p => p.Port);

        public static bool PortInUse(int port)
        {
            bool inUse = false;


            foreach (int usingPort in GetUsingPorts())
            {
                if (usingPort == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;  // 返回true说明端口被占用

        }

    }
}
