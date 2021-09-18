using MahApps.Metro.Controls;
using Microsoft.Win32;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using IsUtil;
using IsUtil.Helpers;

namespace Mijin.Library.App.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public ISystemFunc _systemFunc { get; }

        public ClientSettings _clientSettings { get; set; }

        private static readonly string startFilePath =
            System.IO.Path.Combine(@$"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp", "Mijin.Library.App.lnk");

        public SettingsWindow(ISystemFunc systemFunc)
        {
            // 显示在最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            _systemFunc = systemFunc;
            _clientSettings = _systemFunc.ClientSettings;
            this.DataContext = _clientSettings;

            this.IsVisibleChanged += SettingsWindow_IsVisibleChanged;

            // 添加缓存 comBox选项
            this.OnExitClearWebCacheCom.ItemsSource = new string[] { "保留", "清空cookie", "清空LocalState", "删除webview 下的Default文件", "删除整个webview 的Cache文件" };

            this.HFReaderSelect.ItemsSource = System.Enum.GetNames(typeof(HFReaderEnum));

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
            // 每次显示时赋值
            if ((bool)e.NewValue == true)
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
            this.ShowWindowTitleBarCheck.IsOn = _clientSettings.ShowWindowTitleBar;
            this.CanResizeCheck.IsOn = _clientSettings.CanResize;
            this.IsDevCheck.IsOn = _clientSettings.IsDev;
            this.WindowOverhead.IsOn = _clientSettings.WindowOverhead;
            this.CannotClosed.IsOn = _clientSettings.CannotClosed;
            this.FollowSystemRunCheck.IsOn = _clientSettings.FollowSystemRun;
            this.ShowTitleBarBtnsCheck.IsOn = _clientSettings.ShowTitleBarBtns;
            this.HFReaderSelect.SelectedIndex = (int)_clientSettings.HFReader;
            this.HFOriginalCardCheck.IsOn = _clientSettings.HFOriginalCard;
            this.UHFEventIsOldNameCheck.IsOn = _clientSettings.UHFEventIsOldName;
            this.IsM513IdentityReaderCheck.IsOn = _clientSettings.IsM513IdentityReader;
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

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            this.saveLoading.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    _clientSettings.Id = this.idCom.SelectedIndex + 1;
                    _clientSettings.LibraryManageUrl = this.LibraryManageUrlText.Text;
                    _clientSettings.ReaderActionUrl = this.ReaderActionUrlText.Text;
                    _clientSettings.NoSelectOpenUrl = this.NoSelectOpenUrlText.Text;
                    _clientSettings.WindowWidth = this.WindowWidthText.Text.ToInt();
                    _clientSettings.WindowHeight = this.WindowHeightText.Text.ToInt();
                    _clientSettings.OnExitClearWebCache = (ClearWebViewCacheModeEnum)this.OnExitClearWebCacheCom.SelectedIndex;
                    _clientSettings.ShowWindowTitleBar = this.ShowWindowTitleBarCheck.IsOn;
                    _clientSettings.CanResize = this.CanResizeCheck.IsOn;
                    _clientSettings.IsDev = this.IsDevCheck.IsOn;
                    _clientSettings.WindowOverhead = this.WindowOverhead.IsOn;
                    _clientSettings.CannotClosed = this.CannotClosed.IsOn;
                    _clientSettings.FollowSystemRun = this.FollowSystemRunCheck.IsOn;
                    _clientSettings.ShowTitleBarBtns = this.ShowTitleBarBtnsCheck.IsOn;
                    _clientSettings.HFReader = (HFReaderEnum)this.HFReaderSelect.SelectedIndex;
                    _clientSettings.HFOriginalCard = this.HFOriginalCardCheck.IsOn;
                    _clientSettings.UHFEventIsOldName = this.UHFEventIsOldNameCheck.IsOn;
                    _clientSettings.IsM513IdentityReader = this.IsM513IdentityReaderCheck.IsOn;
                    // 开启启动项设置
                    try
                    {
                        FollowSystemStart(_clientSettings.FollowSystemRun);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.ToString());
                        //MessageBox.Show(e.StackTrace);
                        //MessageBox.Show("设置开机启动项失败！请以管理员启动进行设置！", "提示");
                    }


                    _clientSettings.Write();
                }));
                System.Threading.Thread.Sleep(500);
            });

            this.saveLoading.Visibility = Visibility.Hidden;
            button.IsEnabled = true;
        }

        private void syncUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.LibraryManageUrlText.Text))
            {
                this.ReaderActionUrlText.Text = this.LibraryManageUrlText.Text + @$"{(this.LibraryManageUrlText.Text.Last() == '/' ? "" : "/")}terminal";
            }
        }

        /// <summary>
        /// 设置开机自启
        /// </summary>
        private void FollowSystemStart(bool onoff)
        {
            #region 注册表注册 （测试无效，保留代码）
            //if (onoff)
            //{
            //    #region 设置开机自启
            //    string strName = System.IO.Path.Combine(System.Environment.CurrentDirectory, "start.cmd");//获取要自动运行的应用程序名
            //    if (!System.IO.File.Exists(strName))//判断要自动运行的应用程序文件是否存在
            //        return;
            //    string strnewName = strName.Substring(strName.LastIndexOf("\\") + 1);//获取应用程序文件名，不包括路径
            //    RegistryKey registry = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//检索指定的子项
            //    if (registry == null)//若指定的子项不存在
            //        registry = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
            //    registry.SetValue(strnewName, strName);//设置该子项的新的“键值对”
            //    #endregion
            //}
            //else
            //{
            //    #region 取消开机自启
            //    string strName = System.IO.Path.Combine(System.Environment.CurrentDirectory, "start.cmd");//获取要自动运行的应用程序名
            //    if (!System.IO.File.Exists(strName))//判断要取消的应用程序文件是否存在
            //        return;
            //    string strnewName = strName.Substring(strName.LastIndexOf("\\") + 1);///获取应用程序文件名，不包括路径
            //    RegistryKey registry = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//读取指定的子项
            //    if (registry == null)//若指定的子项不存在
            //        registry = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
            //    registry.DeleteValue(strnewName, false);//删除指定“键名称”的键/值对
            //    #endregion
            //}
            #endregion
            if (onoff)
            {
                //var process = new Process
                //{
                //    StartInfo =
                //     {
                //         WorkingDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                //         FileName = "FollowSystemRunScript.vbs",
                //         Verb = "runas"
                //     }
                //};
                //process.Start();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "wscript.exe";
                startInfo.Arguments = System.IO.Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "FollowSystemRunScript.vbs");
                var p = Process.Start(startInfo);
                p.WaitForExit();
            }
            else
            {
                if (System.IO.File.Exists(startFilePath))
                    FileHelper.FileDel(startFilePath);
            }

        }

        private void SetAutoLend_Click(object sender, RoutedEventArgs e)
        {
            this.NoSelectOpenUrlText.Text = _clientSettings.ReaderActionUrl;
            this.WindowWidthText.Text = "0";
            this.WindowHeightText.Text = "0";
            this.ShowWindowTitleBarCheck.IsOn = false;
            this.CanResizeCheck.IsOn = false;
            this.IsDevCheck.IsOn = false;
            this.WindowOverhead.IsOn = true;
            this.CannotClosed.IsOn = true;
            this.FollowSystemRunCheck.IsOn = true;
            this.ShowTitleBarBtnsCheck.IsOn = false;
        }
    }
}
