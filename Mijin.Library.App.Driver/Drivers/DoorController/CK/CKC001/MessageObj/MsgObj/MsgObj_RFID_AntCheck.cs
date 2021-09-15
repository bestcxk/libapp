namespace PublicAPI.CKC001.MessageObj.MsgObj
{
    public class MsgObj_RFID_AntCheck:MsgObjBase
    {
        byte ant;
        public byte set_ant { set => ant = value; }
        public MsgObj_RFID_AntCheck()
        {
            base.CmdType = PublicAPI.CKC001.Others.eCmdType.RFID;
            base.CmdTag = (byte)PublicAPI.CKC001.Others.eRFID.e_Ant_check;
        }
        internal override void SendPacked()
        {
            base.CmdData = new byte[2] { 0x05,ant };
        }
    }
}
