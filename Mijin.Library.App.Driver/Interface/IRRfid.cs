using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IRRfid
    {
        event Action<WebViewSendModel<List<ScanDataModel>>> OnReadHFLabels;

        /// <summary>
        /// 多线程扫描间隔
        /// </summary>
        public int ScanSpaceMs { get; set; }

        MessageModel<string> AutoOpenComPort();

        MessageModel<string> SetActionLabelPara(long startReadBlock = 0, long ReadBlockCount = 0,
            long actionBlockSize = 0, long writeState = 0);

        MessageModel<List<ScanDataModel>> NewScan(long startBlockNum = -1, long readBlockCount = -1);
        MessageModel<string> Read(bool isAscii = false);
        MessageModel<string> Stop();
        MessageModel<ScanDataModel> ReadOnce(Int64 startBlockNum = -1, Int64 readBlockCount = -1);
        MessageModel<string> WriteLabel(string uidHex, byte[] data, long actionBlockSize = -1, long writeState = -1);
        MessageModel<string> WriteLabel(string uidHex, string hexData, long actionBlockSize = -1, long writeState = -1);
        MessageModel<string> SetEAS(List<long> uid, bool enabled);
        MessageModel<string> SetEAS(string uidHexStr, bool enabled);
        MessageModel<string> SetScanSpaceMs(long ms);

        MessageModel<string> WriteLabelByAscii(string uidHex, string asciiData, long actionBlockSize = -1,
            long writeState = -1);
    }
}