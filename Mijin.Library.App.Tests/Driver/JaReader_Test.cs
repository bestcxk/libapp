using Mijin.Library.App.Driver.Drivers.Reader;
using Mijin.Library.App.Driver.Interface;
using Xunit;

namespace Mijin.Library.App.Tests.Driver;

public class JaReader_Test
{
    private IHFReader _reader;

    public JaReader_Test()
    {
        _reader = new MiniBlackHFReader(null);
    }

    /// <summary>
    /// 自动连接测试
    /// </summary>
    [Fact]
    public void ConnectTest()
    {
        var res = _reader.Init();
        Assert.True(res.response);
    }

    /// <summary>
    /// 读卡号测试
    /// </summary>
    [Fact]
    public void ReadCardNoTest()
    {
        var res = _reader.ReadCardNo();
        Assert.True(res.success);
    }

    /// <summary>
    /// 读块数据测试
    /// </summary>
    [Fact]
    public void ReadBlockTest()
    {
        var res = _reader.ReadBlock(0, 0);
        Assert.True(res.success);
    }
}