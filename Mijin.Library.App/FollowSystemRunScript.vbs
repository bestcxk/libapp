Set WshShell = WScript.CreateObject("WScript.Shell")
Startup = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
set oShellLink = WshShell.CreateShortcut(Startup & "\Mijin.Library.App.lnk")
oShellLink.TargetPath = createobject("Scripting.FileSystemObject").GetFolder(".").Path & "\Mijin.Library.App.exe"
oShellLink.Description = "快捷方式"
oShellLink.WorkingDirectory = createobject("Scripting.FileSystemObject").GetFolder(".").Path
oShellLink.Save