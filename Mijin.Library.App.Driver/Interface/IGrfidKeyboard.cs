using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Interface;

public interface IGrfidKeyboard
{
    MessageModel<string> SetOnReadUHFLabelEventName(string eventName);
    MessageModel<string> SetPrintKeyboard(bool putkey);
    MessageModel<string> SetPrintKeyboardIntervalMs(long ms);
}

public interface IGrfidKeyboard1 : IGrfidKeyboard
{
}