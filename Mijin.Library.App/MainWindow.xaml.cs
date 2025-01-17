﻿using Bing.Extensions;
using MahApps.Metro.Controls;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using Mijin.Library.App.Setting;
using Mijin.Library.App.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Mijin.Library.App.Driver.Services.Network;
using Util.Helpers;
using Bing.Text;
using System.Windows.Automation;
using DryIoc;
using ClientSettings = Mijin.Library.App.Model.ClientSettings;
using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Model.Setting;

namespace Mijin.Library.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public WebViewWindow _webView { get; }
        public ClientSettings _clientSettings { get; }
        public SettingWindow _settingsWindow { get; }
        public ISystemFunc _systemFunc { get; }


        public MainWindow(IContainer serviceProvider) // WebViewWindow webView, SettingWindow settingsWindow, ISystemFunc systemFunc
        {
            //显示在显示器最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();
            _webView = serviceProvider.Resolve<WebViewWindow>();
            _settingsWindow = serviceProvider.Resolve<SettingWindow>();
            _systemFunc = serviceProvider.Resolve<ISystemFunc>();
            _clientSettings = _systemFunc.ClientSettings;
            //_webView = webView;
            //_clientSettings = systemFunc.ClientSettings;
            //_settingsWindow = settingsWindow;
            //_systemFunc = systemFunc;

            if (!_clientSettings.ShowTitleBarBtns)
            {
                this.doorBtn.Visibility = Visibility.Hidden;
                this.labelConvertBtn.Visibility = Visibility.Hidden;
            }


            Title = _clientSettings.Title?.App ?? "图书管理系统";
            manager.Title = _clientSettings.Title?.Manager ?? "后台管理";
            autoLend.Title = _clientSettings.Title?.Terminal ?? "自助借阅";

        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.versionLabel.Content = @$"版本号：{Assembly.GetExecutingAssembly().GetName().Version}";
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
                if (_clientSettings.Title?.Manager == tile.Title)
                    //this._webView.openUrl = _netWorkTranspondService.GetVisitUrl(_clientSettings.LibraryManageUrl);
                    this._webView.OpenUrl = _clientSettings.LibraryManageUrl;
                else
                    //this._webView.openUrl = _netWorkTranspondService.GetVisitUrl(_clientSettings.ReaderActionUrl);
                    this._webView.OpenUrl = _clientSettings.ReaderActionUrl;
                this._webView.Show();
                this.Hide();
            }
        }
        private void doorViewBtn(object sender, RoutedEventArgs e)
        {

            this.Hide();
            _webView.ShowDoorViewBtn(null, null);
        }

        private void labelConvert_Click(object sender, RoutedEventArgs e)
        {
            this._webView.OpenUrl = this._clientSettings.LabelConvertUrl.IsEmpty() ? Url.Combine(this._clientSettings.LibraryManageUrl, "labelSwitch") : this._clientSettings.LabelConvertUrl;
            //this._webView.openUrl = _netWorkTranspondService.GetVisitUrl(this._clientSettings.LabelConvertUrl.IsEmpty() ? Url.Combine(this._clientSettings.LibraryManageUrl, "labelSwitch") : this._clientSettings.LabelConvertUrl);
            this._webView.Show();
            this.Hide();
        }


        private void ShowCxQueryBtn(object sender, RoutedEventArgs e)
        {
            Process.Start("Mijin.Library.CxData.Query.exe");
        }
    }
}
