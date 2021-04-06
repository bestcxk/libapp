using MahApps.Metro.Controls;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using Mijin.Library.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public WebViewWindow _webView { get; }
        public ClientSettings _clientSettings { get; }
        public SettingsWindow _settingsWindow { get; }
        public ISystemFunc _systemFunc { get; }

        public MainWindow(WebViewWindow webView,SettingsWindow settingsWindow,ISystemFunc systemFunc)
        {
            //显示在显示器最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            InitializeComponent();
            _webView = webView;
            _clientSettings = systemFunc.ClientSettings;
            _webView = webView;
            _settingsWindow = settingsWindow;
            _systemFunc = systemFunc;
        }

        private void  MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            this._settingsWindow.ShowDialog();
            //显示在显示器最中间
        }

        private void GoWebView(object sender, RoutedEventArgs e)
        {
            Tile tile = sender as Tile;
            if (tile != null)
            {
                switch (tile.Title)
                {
                    case "后台管理系统":
                        this._webView.webView.Source = new Uri(_clientSettings.LibraryManageUrl);
                        break;
                    case "自助借阅":
                        this._webView.webView.Source = new Uri(_clientSettings.ReaderActionUrl);
                        break;
                    default:
                        return;
                }
                this._webView.Show();
                this.Hide();
            }
        }
    }
}
