using MahApps.Metro.Controls;
using Mijin.Library.App.Common;
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

namespace Mijin.Library.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            //显示在显示器最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            InitializeComponent();
        }

        private async void  MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
