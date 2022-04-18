using Bing.IO;
using Mijin.Library.App.Common.EncryptApplocation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mijin.Library.App.Authorization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public Content content { get; set; } = new Content();
        private EncryptApp app = new EncryptApp();

        public AuthWindow()
        {
            InitializeComponent();
            this.DataContext = content;

            content.Code = app.GetEncryptStr();

            if (app.IsAuth())
            {
                SetAuth();
            }
            else
            {
                SetNoAuth();
            }

        }

        private void SetNoAuth()
        {
            content.IsAuth = "未授权";
            content.TextColor = "red";
        }

        private void SetAuth()
        {
            content.Exp = "永久";
            content.IsAuth = "已授权";
            content.TextColor = "green";

        }
        private void Authorization(object sender, RoutedEventArgs e)
        {

            var dir = new DirectoryInfo(Environment.CurrentDirectory);
#if (!DEBUG)
 if (!dir.GetFiles().Any(f => f.Name == "Mijin.Library.App.dll"))
            {
                MessageBox.Show("非程序根目录!");
                return;
            }
#endif

            if (app.Authorization(content.Key))
            {
                SetAuth();
                FileHelper.SaveFile("KeyCode", content.Key, Encoding.UTF8);
            }
            else
            {
                MessageBox.Show("激活码不正确");
            }
        }
    }

    public class Content : BindableBase
    {
        private string key;

        public string Key
        {
            get { return key; }
            set => SetProperty(ref key, value);
        }

        private string isAuth;

        public string IsAuth
        {
            get { return isAuth; }
            set
            {
                SetProperty(ref isAuth, value);
            }
        }

        private string expire;

        public string Exp
        {
            get { return expire; }
            set => SetProperty(ref expire, value);
        }

        private string code;

        public string Code
        {
            get { return code; }
            set => SetProperty(ref code, value);
        }

        private string textColor;

        public string TextColor
        {
            get { return textColor; }
            set => SetProperty(ref textColor, value);
        }
    }
}
