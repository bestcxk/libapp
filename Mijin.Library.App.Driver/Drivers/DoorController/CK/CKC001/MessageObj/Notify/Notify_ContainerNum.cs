﻿using PublicAPI.CKC001.MessageObj.MsgObj;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.CKC001.MessageObj.Notify
{
    public class Notify_ContainerNum: NotifyBase
    {
        public byte getNewAddress { get;internal set; }
        internal Notify_ContainerNum(MsgObjBase msg,string ip)
        {
            base.NotifyAdditiveAttributeA(msg, ip);
        }
    }
}
