﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Bing.Extensions;
using Bing.Helpers;
using Bing.IO;
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

    private Process cmd;

    private Task task;


    public void StartOrUpdateListen(ClientSettings settings)
    {
        RegisterCaddyIfNotService();
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
        }).Distinct().ToList();

        var str = @$"
{{
    admin off

}}";
        for (int i = 0; i < Transponds.Count; i++)
        {

            str += Transponds[i].FormatToCaddy(55000 + i);
        }
        FileHelper.Write("Caddyfile", str);

        RestartCaddyService();
    }

    public string GetVisitUrl(string url)
    {
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

    public void ClearAllListen()
    {
        StopCaddyService();
    }

    public void RegisterCaddyIfNotService()
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
                p.WaitForExit();
            }


        }
    }

    public void RestartCaddyService()
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
                if (s.Status == ServiceControllerStatus.Running || s.Status == ServiceControllerStatus.StartPending)
                    StopCaddyService();

                s.Start();
            }


        }
    }

    public void StopCaddyService()
    {
        ServiceController[] services = ServiceController.GetServices();
        var s = services.FirstOrDefault(s => s.ServiceName == "mijin_caddy_services");
        if (s is not null)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("将导致无法使用代理", "非管理员启动");
            }
            else if (s.Status != ServiceControllerStatus.Stopped && s.Status != ServiceControllerStatus.StopPending)
            {
                var p = ProcessHelper.StartCmd("nssm stop mijin_caddy_services confirm", "exit");
                try
                {
                    p.WaitForExit();
                }
                catch (Exception)
                {

                }
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