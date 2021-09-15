namespace PublicAPI.CKC001.MessageObj.MsgObj
{
    public class MsgObj_shelf_led_open : MsgObjBase
    {
        ///private char led_num;
        /// <summary>
        /// true 开报警  false关报警灯
        /// </summary>
        byte led_num;
        public byte set_led_num { set => led_num = value; }
        //public bool setAlarmStatus { internal get => lightStatus; set => lightStatus = value; }
        public MsgObj_shelf_led_open()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.LED;
            base.CmdTag = 0x07;//开灯
        }
        public void MsgObj_shelf_led_colse()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.LED;
            base.CmdTag = 0x08;//关灯  
        }
        internal override void SendPacked()
        {
            base.CmdData = new byte[1] { led_num };
        }
    }

    public class MsgObj_shelf_led_close : MsgObjBase
    {
        ///private char led_num;
        /// <summary>
        /// true 开报警  false关报警灯
        /// </summary>
        byte led_num_2;
        public byte set_led_num_2 { set => led_num_2 = value; }
        //public bool setAlarmStatus { internal get => lightStatus; set => lightStatus = value; }
        public MsgObj_shelf_led_close()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.LED;
            base.CmdTag = 0x08;//开灯
        }
        internal override void SendPacked()
        {
            base.CmdData = new byte[1] { led_num_2 };
        }
    }
}