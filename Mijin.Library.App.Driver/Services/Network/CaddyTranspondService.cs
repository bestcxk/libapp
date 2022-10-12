using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using Bing.Extensions;
using Bing.Helpers;
using Bing.IO;
using Bing.Threading.Asyncs;
using CliWrap;
using IsUtil.Helper;
using Mijin.Library.App.Common.Domain;
using Mijin.Library.App.Model;
using Url = IsUtil.Helpers.Url;

namespace Mijin.Library.App.Driver.Services.Network;

internal class Transpond
{
    public string OriginUrl { get; set; }
    public string ToUrl { get; set; }

    public string FormatToCaddy(int localPort)
    {
        var str = @$"
:{localPort} {{
	reverse_proxy {OriginUrl}
}}

";
        ToUrl = @$"127.0.0.1:{localPort}";
        return str;


    }

}


public class CaddyTranspondService : INetWorkTranspondService
{
    internal List<Transpond> Transponds { get; set; } = new List<Transpond>();

    public async Task StartOrUpdateListen(ClientSettings settings)
    {
        await RegisterCaddyIfNotService();
        var urls = new List<string>()
        {
            settings.LibraryManageUrl,
            settings.ReaderActionUrl,
            settings.NoSelectOpenUrl,
            settings.LabelConvertUrl,
            settings.DoorControllerUrl,
        }.Where(p => !p.IsEmpty() && !p.Contains("file://") && !p.Contains(@$".\") && !p.Contains("localhost") && !p.Contains("127.0.0.1")).ToList();


        Transponds = urls.Select(url => new Transpond()
        {
            OriginUrl = Url.GetIpPort(url)
        }).DistinctBy(d => d.OriginUrl).ToList();

        var str = @$"
{{
    admin off

}}";
        for (int i = 0; i < Transponds.Count; i++)
        {

            str += Transponds[i].FormatToCaddy(55000 + i);
        }
        FileHelper.Write("Caddyfile", str);

        await RestartCaddyService();
    }

    public string GetVisitUrl(string url)
    {

        if (new ClientSettings().DisibleProxy)
            return url;

        var transpond = Transponds.FirstOrDefault(t => t.OriginUrl == Url.GetIpPort(url));

        if (transpond is null)
            return url;


        var sp = url.Split(transpond.OriginUrl);

        if (sp.Length > 1)
        {
            var str = (sp.FirstOrDefault() ?? "") + transpond.ToUrl + sp.Last();
            return str;
        }

        return (sp.FirstOrDefault() ?? "") + Transponds.FirstOrDefault(t => t.OriginUrl == Url.GetIpPort(url))?.ToUrl;
    }

    public async Task ClearAllListen()
    {
        await StopCaddyService();
    }

    public async Task RegisterCaddyIfNotService()
    {
        ServiceController[] services = ServiceController.GetServices();
        var s = services.FirstOrDefault(s => s.ServiceName == "mijin_caddy_services");
        if (s is null)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("将导致无法使用代理", "非管理员启动");
            }
            else
            {
                var p = Process.Start("caddyRegiester.bat");
                await p.WaitForExitAsync();
            }


        }
    }

    public async Task RestartCaddyService()
    {
        ServiceController[] services = ServiceController.GetServices();
        var s = services.FirstOrDefault(s => s.ServiceName == "mijin_caddy_services");
        if (s is not null)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("将导致无法使用代理", "非管理员启动");
            }
            else
            {
                await StopCaddyService();
                await StartCaddyService();
            }
        }
    }


    static AsyncLock caddyLock = new AsyncLock();

    async Task StopCaddyService()
    {
        using (await caddyLock.LockAsync())
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("将导致无法使用代理", "非管理员启动");
                return;
            }
            var p = ProcessHelper.StartCmd("nssm stop mijin_caddy_services confirm", "exit");
            try
            {
                await p.WaitForExitAsync();
            }
            catch (Exception)
            {

            }
        }

    }

    async Task StartCaddyService()
    {
        using (await caddyLock.LockAsync())
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("将导致无法使用代理", "非管理员启动");
                return;
            }

            var p = ProcessHelper.StartCmd("nssm start mijin_caddy_services confirm", "exit");
            try
            {
                await p.WaitForExitAsync();
            }
            catch (Exception)
            {

            }
        }



    }

    private bool IsAdministrator()
    {
        System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        System.Security.Principal.WindowsPrincipal principal =
            new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }
}
