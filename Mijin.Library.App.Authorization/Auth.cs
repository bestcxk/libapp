using Bing.IO;
using IsUtil.Helpers;
using Mijin.Library.App.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mijin.Library.App.Authorization
{
    public class Auth
    {
        private static EncryptApp app = null;

        private static readonly object _lock = new object();

        public static EncryptApp App
        {
            get
            {
                if (app == null)
                {
                    lock (_lock)
                    {
                        if (app == null)
                        {
                            lock (_lock)
                            {
                                app = new EncryptApp();
                            }
                        }
                    }
                }


                return app;
            }
            private set
            {
            }
        }


        public static bool IsAuth()
        {
            var key = Bing.IO.FileHelper.ReadToString("KeyCode", Encoding.UTF8)?.Replace("-", "")?.ToUpper();
            var enCode = App.GetEncryptStr();
            var enKey = App.GetDeEncryptStr(enCode).Replace("-", "").ToUpper();
            return enKey == key;
        }
    }
}
