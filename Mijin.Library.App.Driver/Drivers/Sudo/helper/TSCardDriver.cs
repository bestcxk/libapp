using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.Sudo
{
    public static class TSCardDriver
    {
        [DllImport("TSCardDriver.dll", EntryPoint = "iInitParms", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int iInitParms(int ReaderNo, string Parm2, string Parm3, byte[] bytes);

        [DllImport("TSCardDriver.dll", EntryPoint = "iReadCardBasExt", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int iReadCardBasExt(int type, byte[] bytes);
    }
}
