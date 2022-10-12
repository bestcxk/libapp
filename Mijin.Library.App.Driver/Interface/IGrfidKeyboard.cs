using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Interface;

public interface IGrfidKeyboard : IRfid
{
    /// <summary>
    /// 设置读取标签事件名（必须先设置）
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    MessageModel<string> SetOnReadUHFLabelEventName(string eventName);


    /// <summary>
    /// 设置是否开始监听rfid并模拟键盘输出,
    /// </summary>
    /// <param name="putkey">true:开，false：关</param>
    /// <param name="ms">每次模拟键盘输出时的间隔毫秒</param>
    /// <returns></returns>
    MessageModel<string> SetPrintKeyboard(bool putkey, long ms, bool enabledLF = true);
}

public interface IGrfidKeyboard1 : IGrfidKeyboard
{
}