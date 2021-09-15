namespace PublicAPI.CKC001.MessageObj.MsgObj
{
    public class MsgObj_RS485_lock:MsgObjBase
    {
        private char openLock;
        /// <summary>
        /// true 开报警  false关报警灯
        /// </summary>
        byte lock_num;
        public byte set_lock_num { set => lock_num = value; }
        //public bool setAlarmStatus { internal get => lightStatus; set => lightStatus = value; }
        public MsgObj_RS485_lock()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.rs485_lock;
            base.CmdTag = 0x01;//开锁
        }        
        internal override void SendPacked()
        {
            base.CmdData = new byte[1] { lock_num };
        }
    }

    public class MsgObj_RS485_lock_status_get : MsgObjBase
    {
        private char openLock;
        /// <summary>
        /// true 开报警  false关报警灯
        /// </summary>
        byte lock_num;
        public byte set_lock_num { set => lock_num = value; }
        //public bool setAlarmStatus { internal get => lightStatus; set => lightStatus = value; }
        public MsgObj_RS485_lock_status_get()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.rs485_lock;
            base.CmdTag = 0x02;//状态获取
        }       
    }
}
