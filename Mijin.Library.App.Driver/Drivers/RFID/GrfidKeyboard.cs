using Bing.Extensions;
using Bing.Helpers;
using GDotnet.Reader.Api.DAL;
using IsUtil;
using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.RFID;



public class GrfidKeyboard : GRfid, IRfid, IGrfidKeyboard, IGrfidKeyboard1
{
    public virtual event Action<WebViewSendModel<LabelInfo>> OnReadUHFLabel;
    public GrfidKeyboard(ISystemFunc systemFunc) : base(systemFunc)
    {
    }
    public KeyboardSettings keyboardSettings { get; set; } = new KeyboardSettings()
    {

    };
    const string ALERT = "0101";
    const string NO_ALERT = "0202";
    const string HF_LABEL = "0303";
    const string IS_LAYER = "0808";
    const string MIJIN_TAG = "AB66";

    private string eventName = "OnReadUHFLabel";

    protected override void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
    {
        // 回调内部如有阻塞，会影响API正常使用
        // 标签回调数量较多，请将标签数据先缓存起来再作业务处理
        if (null != msg && 0 == msg.logBaseEpcInfo.Result)
        {
            // 只读一次的缓存label
            if (_readOnce)
            {
                _tempLabel = new LabelInfo(msg.logBaseEpcInfo);
                this.Stop(); // 达到次数后停止读标签
            }
            else
            {
                OnReadUHFLabel.Invoke(new WebViewSendModel<LabelInfo>()
                {
                    msg = "获取成功",
                    success = true,
                    response = new LabelInfo(msg.logBaseEpcInfo),
                    method = eventName
                });
            }
        }
        base.OnEncapedTagEpcLog(msg);

        if (!keyboardSettings.StartPutKeyboard)
            return;

        var epc = msg.logBaseEpcInfo.Epc;
        if (epc.IsEmpty())
            return;
        var isTagLabel = epc.Substring(epc.Length - 4, 4) == MIJIN_TAG;

        if (isTagLabel != true)
            return;

        try
        {
            var serialLen = epc.Substring(4, 2).ToInt();
            var serial = epc.Substring(6, serialLen);

            Task.Run(async () =>
            {
                await keyboardSettings.PutValue(serial);
            });
        }
        catch (Exception)
        {

        }






    }

    public MessageModel<string> SetPrintKeyboardIntervalMs(Int64 ms)
    {
        keyboardSettings.PrintKeyboardIntervalMs = (int)ms;

        return new MessageModel<string>()
        {
            success = true,
            msg = "设置成功"
        };

    }

    public MessageModel<string> SetOnReadUHFLabelEventName(string eventName)
    {
        this.eventName = eventName;
        return new MessageModel<string>()
        {
            success = true,
            msg = "设置成功"
        };
    }

    public MessageModel<string> SetPrintKeyboard(bool putkey)
    {
        keyboardSettings.StartPutKeyboard = putkey;
        return new MessageModel<string>()
        {
            success = true,
            msg = "设置成功"
        };
    }
}




public class KeyboardSettings
{
    // 模拟键盘输出开关
    public bool StartPutKeyboard { get; set; }
    // 单个按键输出检测
    public int PrintKeyboardSignleIntervalMs { get; set; } = 10;
    // 输出完后，下次再次输出的间隔
    public int PrintKeyboardIntervalMs { get; set; } = 1000;

    // 最后一次模拟键盘输出的值
    public string LastPutValue { get; set; }

    private DateTime LastPutTime { get; set; } = DateTime.Now;

    public async Task PutValue(string str)
    {
        if (!LastPutValue.IsEmpty() && LastPutValue == str || str.IsEmpty())
            return;

        if ((DateTime.Now - LastPutTime).TotalMilliseconds < PrintKeyboardIntervalMs)
            return;

        var bytes = str.Select(v => (byte)(v.ToInt() + 48)).ToArray();
        foreach (var b in bytes)
        {
            KeyBoard.keyPress(b);
            await Task.Delay(PrintKeyboardSignleIntervalMs);
        }
        LastPutValue = str;
        LastPutTime = DateTime.Now;
    }
}

public class KeyBoard
{
    public const byte vKeyLButton = 0x1;    // 鼠标左键
    public const byte vKeyRButton = 0x2;    // 鼠标右键
    public const byte vKeyCancel = 0x3;     // CANCEL 键
    public const byte vKeyMButton = 0x4;    // 鼠标中键
    public const byte vKeyBack = 0x8;       // BACKSPACE 键
    public const byte vKeyTab = 0x9;        // TAB 键
    public const byte vKeyClear = 0xC;      // CLEAR 键
    public const byte vKeyReturn = 0xD;     // ENTER 键
    public const byte vKeyShift = 0x10;     // SHIFT 键
    public const byte vKeyControl = 0x11;   // CTRL 键
    public const byte vKeyAlt = 18;         // Alt 键  (键码18)
    public const byte vKeyMenu = 0x12;      // MENU 键
    public const byte vKeyPause = 0x13;     // PAUSE 键
    public const byte vKeyCapital = 0x14;   // CAPS LOCK 键
    public const byte vKeyEscape = 0x1B;    // ESC 键
    public const byte vKeySpace = 0x20;     // SPACEBAR 键
    public const byte vKeyPageUp = 0x21;    // PAGE UP 键
    public const byte vKeyEnd = 0x23;       // End 键
    public const byte vKeyHome = 0x24;      // HOME 键
    public const byte vKeyLeft = 0x25;      // LEFT ARROW 键
    public const byte vKeyUp = 0x26;        // UP ARROW 键
    public const byte vKeyRight = 0x27;     // RIGHT ARROW 键
    public const byte vKeyDown = 0x28;      // DOWN ARROW 键
    public const byte vKeySelect = 0x29;    // Select 键
    public const byte vKeyPrint = 0x2A;     // PRINT SCREEN 键
    public const byte vKeyExecute = 0x2B;   // EXECUTE 键
    public const byte vKeySnapshot = 0x2C;  // SNAPSHOT 键
    public const byte vKeyDelete = 0x2E;    // Delete 键
    public const byte vKeyHelp = 0x2F;      // HELP 键
    public const byte vKeyNumlock = 0x90;   // NUM LOCK 键

    //字母键A到Z
    public const byte vKeyA = 65;
    public const byte vKeyB = 66;
    public const byte vKeyC = 67;
    public const byte vKeyD = 68;
    public const byte vKeyE = 69;
    public const byte vKeyF = 70;
    public const byte vKeyG = 71;
    public const byte vKeyH = 72;
    public const byte vKeyI = 73;
    public const byte vKeyJ = 74;
    public const byte vKeyK = 75;
    public const byte vKeyL = 76;
    public const byte vKeyM = 77;
    public const byte vKeyN = 78;
    public const byte vKeyO = 79;
    public const byte vKeyP = 80;
    public const byte vKeyQ = 81;
    public const byte vKeyR = 82;
    public const byte vKeyS = 83;
    public const byte vKeyT = 84;
    public const byte vKeyU = 85;
    public const byte vKeyV = 86;
    public const byte vKeyW = 87;
    public const byte vKeyX = 88;
    public const byte vKeyY = 89;
    public const byte vKeyZ = 90;

    //数字键盘0到9
    public const byte vKey0 = 48;    // 0 键
    public const byte vKey1 = 49;    // 1 键
    public const byte vKey2 = 50;    // 2 键
    public const byte vKey3 = 51;    // 3 键
    public const byte vKey4 = 52;    // 4 键
    public const byte vKey5 = 53;    // 5 键
    public const byte vKey6 = 54;    // 6 键
    public const byte vKey7 = 55;    // 7 键
    public const byte vKey8 = 56;    // 8 键
    public const byte vKey9 = 57;    // 9 键


    public const byte vKeyNumpad0 = 0x60;    //0 键
    public const byte vKeyNumpad1 = 0x61;    //1 键
    public const byte vKeyNumpad2 = 0x62;    //2 键
    public const byte vKeyNumpad3 = 0x63;    //3 键
    public const byte vKeyNumpad4 = 0x64;    //4 键
    public const byte vKeyNumpad5 = 0x65;    //5 键
    public const byte vKeyNumpad6 = 0x66;    //6 键
    public const byte vKeyNumpad7 = 0x67;    //7 键
    public const byte vKeyNumpad8 = 0x68;    //8 键
    public const byte vKeyNumpad9 = 0x69;    //9 键
    public const byte vKeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)键
    public const byte vKeyAdd = 0x6B;        // PLUS SIGN(+) 键
    public const byte vKeySeparator = 0x6C;  // ENTER 键
    public const byte vKeySubtract = 0x6D;   // MINUS SIGN(-) 键
    public const byte vKeyDecimal = 0x6E;    // DECIMAL POINT(.) 键
    public const byte vKeyDivide = 0x6F;     // DIVISION SIGN(/) 键


    //F1到F12按键
    public const byte vKeyF1 = 0x70;   //F1 键
    public const byte vKeyF2 = 0x71;   //F2 键
    public const byte vKeyF3 = 0x72;   //F3 键
    public const byte vKeyF4 = 0x73;   //F4 键
    public const byte vKeyF5 = 0x74;   //F5 键
    public const byte vKeyF6 = 0x75;   //F6 键
    public const byte vKeyF7 = 0x76;   //F7 键
    public const byte vKeyF8 = 0x77;   //F8 键
    public const byte vKeyF9 = 0x78;   //F9 键
    public const byte vKeyF10 = 0x79;  //F10 键
    public const byte vKeyF11 = 0x7A;  //F11 键
    public const byte vKeyF12 = 0x7B;  //F12 键


    // <param name="bVk" >按键的虚拟键值</param>
    // <param name= "bScan" >扫描码，一般不用设置，用0代替就行</param>
    // <param name= "dwFlags" >选项标志：0：表示按下，2：表示松开</param>
    // <param name= "dwExtraInfo">一般设置为0</param>
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public static void keyPress(byte keyName)//定义“按一下”方法
    {
        KeyBoard.keybd_event(keyName, 0, 0, 0);
        KeyBoard.keybd_event(keyName, 0, 2, 0);
    }
}


