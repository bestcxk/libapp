using System;
using System.Text;
using IsUtil.Helpers;
using Mijin.Library.App.Driver.Drivers.Lock;
using Xunit;
using Console = System.Console;

namespace Mijin.Library.App.Tests.Driver;

public class CkLock_Test
{
    private CkLock _ckLock;

    public CkLock_Test()
    {
        _ckLock = new CkLock();
    }
    
    /// <summary>
    /// 开启指定锁控
    /// </summary>
    [Fact]
    public void OpenLockTest()
    {
        var openSerialPortResult = _ckLock.OpenSerialPort("COM8", 9600);

        var openBoxResult = _ckLock.OpenBox(1);
    }

    /// <summary>
    /// 获取锁控板状态
    /// </summary>
    [Fact]
    public void GetLockStatusTest()
    {
        var openSerialPortResult = _ckLock.OpenSerialPort("COM8", 9600);

        var lockStatusResult = _ckLock.GetLockStatus();
    }
    
    /// <summary>
    /// 锁控板状态事件
    /// </summary>
    [Fact]
    public void LockEventTest()
    {
        var openSerialPortResult = _ckLock.OpenSerialPort("COM8", 9600);

        _ckLock.OnLockEvent += model =>
        {
            Console.WriteLine();
        };

        while (true)
        {
        }
    }

}