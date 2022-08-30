nssm stop mijin_caddy_services confirm
nssm remove mijin_caddy_services confirm
nssm install mijin_caddy_services %cd%\caddyRun.bat
nssm set mijin_caddy_services AppDirectory  %cd%

