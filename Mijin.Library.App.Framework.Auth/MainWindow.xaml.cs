using Bing.Extensions;
using Bing.Helpers;
using Mijin.Library.App.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Mijin.Library.App.Framework.Auth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EncryptApp app = new EncryptApp();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenKey(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.code.Text))
            {
                MessageBox.Show("请在密钥中填写被授权的授权码");
                return;
            }

            this.key.Text = app.GetDeEncryptStr(this.code.Text);


        }
    }
}
