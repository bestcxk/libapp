using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Mijin.Library.App.Driver.Drivers.Sudo
{
	
    class ID2Parser
    {
        private byte[] mID2TxtRAW = new byte[256];
        private byte[] mID2PicRAW = new byte[1024];
        private byte[] mID2FPRAW = new byte[1024];
        private byte[] mID2AddRAW = new byte[73];

		public ID2Parser(byte[] data, int offset = 0)
		{
			decode(data, offset);
		}
		
		//[DllImport("WltRS.dll", CallingConvention = CallingConvention.StdCall)]
        [DllImport("WltRS.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        extern public static  int GetBmp(String file_name, int Port);

		public void decode(byte[] _raw, int offset)
		{
			if ((_raw[offset + 0] == 1) && (_raw[offset + 1] == 0))
			{
				// 文字
				Array.Copy(_raw, offset + 4, this.mID2TxtRAW, 0, 256);
			}
			if ((_raw[offset+ 2] == 4) && (_raw[offset + 3] == 0))
			{
				//照片
				Array.Copy(_raw, offset + 260, this.mID2PicRAW, 0, 1024);
				
			}
			if ((_raw[offset + 4] == 4) && (_raw[offset + 5] == 0) && (_raw[offset + 1286] == 67))
			{
				//指纹
				Array.Copy(_raw, offset + 1284, this.mID2FPRAW, 0, 1024);
			}
			if (_raw.Length - offset >= 2383 &&
			  (_raw[offset + 2310] == 0) &&
			  (_raw[offset + 2311] == 0) &&
			  (_raw[offset + 2312] == -112))
			{
				Array.Copy(_raw, offset +  2310, this.mID2AddRAW, 0, 73);
			}
		}

        public ID2Txt ParseText()
        {
            return new ID2Txt(this.mID2TxtRAW);
        }

        public byte[] ParsePic()
        {
            FileStream fs = File.Open(".\\pic.wlt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(this.mID2PicRAW, 0, this.mID2PicRAW.Length);
            fs.Close();
            int rst = GetBmp(".\\pic.wlt", 2);
            if (rst != 1) return null;
            File.Delete(".\\pic.wlt");
            if (!File.Exists(".\\pic.bmp"))
                return null;
            FileStream fs2 = File.Open(".\\pic.bmp", FileMode.Open, FileAccess.Read);
            byte[] d = new byte[fs2.Length];
            int len = fs2.Read(d, 0, d.Length);
            fs2.Close();
            File.Delete(".\\pic.bmp");
            return d;
        }
	}

	class ID2Txt
	{
        public String mName;
        public String mGender;
        public String mGenderIndex;
        public String mNational;
        public String mNationalIndex;
        public String mBirthYear;
        public String mBirthMonth;
        public String mBirthDay;
        public String mAddress;
        public String mID2Num;
        public String mIssue;
        public String mBegin;
        public String mEnd;

        public ID2Txt(byte[] data)
        {
            decode(data);
        }

        public void decode(byte[] _txt)
        {
            try
            {
                this.mName = Encoding.Unicode.GetString(_txt, 0, 30);
                
                this.mGenderIndex = Encoding.Unicode.GetString(_txt, 30, 2);
                
                this.mNationalIndex = Encoding.Unicode.GetString(_txt, 32, 4);
                
                this.mBirthYear = Encoding.Unicode.GetString(_txt, 36, 8);
                
                this.mBirthMonth = Encoding.Unicode.GetString(_txt, 44, 4);
                
                this.mBirthDay = Encoding.Unicode.GetString(_txt, 48, 4);
                
                this.mAddress = Encoding.Unicode.GetString(_txt, 52, 70);
                
                this.mID2Num = Encoding.Unicode.GetString(_txt, 122, 36);
                
                this.mIssue = Encoding.Unicode.GetString(_txt, 158, 30);
                
                this.mBegin = Encoding.Unicode.GetString(_txt, 188, 16);
                
                this.mEnd = Encoding.Unicode.GetString(_txt, 204, 16);
                
                this.mGender = GetGenderFromCode(this.mGenderIndex);
                
                this.mNational = GetNationalFromCode(this.mNationalIndex);
                
            }
            catch (Exception e)
            {
                
            }
        }
        private String GetGenderFromCode(String genderCode)
        {
            switch (int.Parse(genderCode))
            {
                case 0:
                    return "未知的性别";
                case 1:
                    return "男";
                case 2:
                    return "女";
                case 9:
                    return "未说明的性别";
            }
            return "未定义的性别";
        }
        private String GetNationalFromCode(String nationalCode)
        {
            int n = int.Parse(nationalCode);
            switch (n)
            {
                case 1:
                    return "汉";
                case 2:
                    return "蒙古";
                case 3:
                    return "回";
                case 4:
                    return "藏";
                case 5:
                    return "维吾尔";
                case 6:
                    return "苗";
                case 7:
                    return "彝";
                case 8:
                    return "壮";
                case 9:
                    return "布依";
                case 10:
                    return "朝鲜";
                case 11:
                    return "满";
                case 12:
                    return "侗";
                case 13:
                    return "瑶";
                case 14:
                    return "白";
                case 15:
                    return "土家";
                case 16:
                    return "哈尼";
                case 17:
                    return "哈萨克";
                case 18:
                    return "傣";
                case 19:
                    return "黎";
                case 20:
                    return "傈僳";
                case 21:
                    return "佤";
                case 22:
                    return "畲";
                case 23:
                    return "高山";
                case 24:
                    return "拉祜";
                case 25:
                    return "水";
                case 26:
                    return "东乡";
                case 27:
                    return "纳西";
                case 28:
                    return "景颇";
                case 29:
                    return "柯尔克孜";
                case 30:
                    return "土";
                case 31:
                    return "达斡尔";
                case 32:
                    return "仫佬";
                case 33:
                    return "羌";
                case 34:
                    return "布朗";
                case 35:
                    return "撒拉";
                case 36:
                    return "毛难";
                case 37:
                    return "仡佬";
                case 38:
                    return "锡伯";
                case 39:
                    return "阿昌";
                case 40:
                    return "普米";
                case 41:
                    return "塔吉克";
                case 42:
                    return "怒";
                case 43:
                    return "乌孜别克";
                case 44:
                    return "俄罗斯";
                case 45:
                    return "鄂温克";
                case 46:
                    return "崩龙";
                case 47:
                    return "保安";
                case 48:
                    return "裕固";
                case 49:
                    return "京";
                case 50:
                    return "塔塔尔";
                case 51:
                    return "独龙";
                case 52:
                    return "鄂伦春";
                case 53:
                    return "赫哲";
                case 54:
                    return "门巴";
                case 55:
                    return "珞巴";
                case 56:
                    return "基诺";
            }
            return "其他";
        }
    }
}
