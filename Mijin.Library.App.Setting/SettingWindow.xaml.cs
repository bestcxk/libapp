using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using IsUtil;
using IsUtil.Helpers;
using MahApps.Metro.Controls;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Setting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingWindow : MetroWindow
    {

        public ClientSettings _clientSettings { get; set; }
        public ISystemFunc _systemFunc { get; }

        private static readonly string startFilePath =
            System.IO.Path.Combine(@$"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp", "Mijin.Library.App.lnk");

        public SettingWindow(ISystemFunc systemFunc)
        {
            // 显示在最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            _clientSettings = systemFunc.ClientSettings;

            this.DataContext = _clientSettings;

            // 添加缓存 comBox选项
            this.OnExitClearWebCacheCom.ItemsSource = System.Enum.GetValues(typeof(ClearWebViewCacheModeEnum));
            this.QrcodeDriverCom.ItemsSource = System.Enum.GetValues(typeof(QrcodeDriver));
            this.HFReaderSelect.ItemsSource = System.Enum.GetValues(typeof(HFReaderEnum));

            // 添加id comBox 选项
            List<int> idComSources = new List<int>();
            List<int> cameraSources = new List<int>();
            for (int i = 0; i < 50; i++)
            {
                idComSources.Add(i + 1);
                cameraSources.Add(i);
            }
            this.idCom.ItemsSource = idComSources;
            this.cameraIndex.ItemsSource = cameraSources;
            _systemFunc = systemFunc;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Process.GetProcessesByName("Mijin.Library.App").Any())
            {
                Environment.Exit(0);
            }
            else
            {
                this.Hide();
                e.Cancel = true;
            }

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
                    // 开启启动项设置
                    try
                    {
                        FollowSystemStart(_clientSettings.FollowSystemRun);
                    }
                    catch (Exception e)
                    {
                    }
                    //_systemFunc.ClientSettings.SetPropValue(_clientSettings, true);
                    _clientSettings.Write();

                }));
            });
            await Task.Delay(500);
            this.saveLoading.Visibility = Visibility.Hidden;
            button.IsEnabled = true;
        }

        /// <summary>
        /// 同步自助借阅Url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void syncUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.LibraryManageUrlText.Text))
            {
                this.ReaderActionUrlText.Text = this.LibraryManageUrlText.Text + @$"{(this.LibraryManageUrlText.Text.Last() == '/' ? "" : "/")}terminal";
                this._clientSettings.ReaderActionUrl = this.ReaderActionUrlText.Text;
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

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Process.GetProcessesByName("Mijin.Library.App.Daemon").FirstOrDefault()?.Kill();
            Process.GetProcessesByName("Mijin.Library.App").FirstOrDefault()?.Kill();
            Environment.Exit(0);
        }
    }
}
