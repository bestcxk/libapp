using MahApps.Metro.Controls;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Util;

namespace Mijin.Library.App.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public ISystemFunc _systemFunc { get; }

        public ClientSettings _clientSettings { get; set; }

        public SettingsWindow(ISystemFunc systemFunc)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen; // 显示在最中间
            InitializeComponent();
            _systemFunc = systemFunc;
            _clientSettings = _systemFunc.ClientSettings;
            this.DataContext = _clientSettings;

            this.IsVisibleChanged += SettingsWindow_IsVisibleChanged;

            // 添加缓存 comBox选项
            this.OnExitClearWebCacheCom.ItemsSource = new string[] { "保留", "清空cookie", "清空LocalState", "删除webview 下的Default文件", "删除整个webview 的Cache文件"};

            // 添加id comBox 选项
            List<int> idComSources = new List<int>();
            for (int i = 1; i <= 50; i++)
            {
                idComSources.Add(i);
            }
            this.idCom.ItemsSource = idComSources;
        }

        private void SettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true) // 每次显示时赋值
            {
                this.ReLoadSettings();
            }
        }

        private void ReLoadSettings()
        {
            this.idCom.SelectedIndex = _clientSettings.Id - 1;
            this.LibraryManageUrlText.Text = _clientSettings.LibraryManageUrl;
            this.ReaderActionUrlText.Text = _clientSettings.ReaderActionUrl;
            this.NoSelectOpenUrlText.Text = _clientSettings.NoSelectOpenUrl;
            this.WindowWidthText.Text = _clientSettings.WindowWidth.ToString();
            this.WindowHeightText.Text = _clientSettings.WindowHeight.ToString();
            this.OnExitClearWebCacheCom.SelectedIndex = (int)_clientSettings.OnExitClearWebCache;
            this.ShowWindowTitleBarCheck.IsChecked = _clientSettings.ShowWindowTitleBar;
            this.CanResizeCheck.IsChecked = _clientSettings.CanResize;
            this.IsDevCheck.IsChecked = _clientSettings.IsDev;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


        private void reLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ReLoadSettings();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            _clientSettings.Id = this.idCom.SelectedIndex +1;
            _clientSettings.LibraryManageUrl = this.LibraryManageUrlText.Text;
            _clientSettings.ReaderActionUrl = this.ReaderActionUrlText.Text;
            _clientSettings.NoSelectOpenUrl = this.NoSelectOpenUrlText.Text;
            _clientSettings.WindowWidth = this.WindowWidthText.Text.ToInt();
            _clientSettings.WindowHeight = this.WindowHeightText.Text.ToInt();
            _clientSettings.OnExitClearWebCache = (ClearWebViewCacheMode)this.OnExitClearWebCacheCom.SelectedIndex;
            _clientSettings.ShowWindowTitleBar = this.ShowWindowTitleBarCheck.IsChecked.ToBool();
            _clientSettings.CanResize = this.CanResizeCheck.IsChecked.ToBool();
            _clientSettings.IsDev = this.IsDevCheck.IsChecked.ToBool();

            _clientSettings.Write();

        }

        private void syncUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.LibraryManageUrlText.Text))
            {
                this.ReaderActionUrlText.Text = this.LibraryManageUrlText.Text + "/terminal";
            }
        }
    }
}
