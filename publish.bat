color B

rd /s /q ..\publish_File\App
  
dotnet restore

dotnet build

cd Mijin.Library.App.Daemon

dotnet publish -f net5.0 -c Release -r win-x64 -p:UseAppHost=true -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false -o ..\..\publish_File\App\


cd ..\Mijin.Library.App

dotnet publish -f net5.0-windows -c Release -r win-x64 -p:UseAppHost=true -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained false -o ..\..\publish_File\App\

xcopy /e /y ..\DLL\* ..\..\publish_File\App\ 

echo "Successfully!!!! ^ please see the file publish_File\App"

cmd