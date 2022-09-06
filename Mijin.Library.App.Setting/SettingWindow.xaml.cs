using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bing.Threading.Asyncs;
using IsUtil;
using IsUtil.Helpers;
using MahApps.Metro.Controls;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using Newtonsoft.Json;

namespace Mijin.Library.App.Setting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingWindow : MetroWindow
    {
        public event Action OnSettingsChange;
        public ClientSettings _clientSettings { get; set; }

        private static readonly string startFilePath =
            System.IO.Path.Combine(@$"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp",
                "Mijin.Library.App.lnk");

        public SettingWindow()
        {
            // 显示在最中间
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            _clientSettings = new ClientSettings();
            this.DataContext = _clientSettings;

            // 添加缓存 comBox选项
            this.OnExitClearWebCacheCom.ItemsSource = System.Enum.GetValues(typeof(ClearWebViewCacheModeEnum));
            this.QrcodeDriverCom.ItemsSource = System.Enum.GetValues(typeof(QrcodeDriver));
            this.HFReaderSelect.ItemsSource = System.Enum.GetValues(typeof(HFReaderEnum));

            // 添加id comBox 选项
            List<int> cameraSources = new List<int>();
            for (int i = 0; i < 50; i++)
            {
                cameraSources.Add(i);
            }

            this.cameraIndex.ItemsSource = cameraSources;

             this.idCom.ItemsSource = cameraSources;



            RefreshIdList();
        }

        public SettingWindow(ISystemFunc systemFunc) : this()
        {
            _clientSettings = systemFunc.ClientSettings;
            RefreshIdList();
        }

        private static AsyncLock actionLock = new AsyncLock();

        public  Task RefreshIdList()
        {
            return RefreshIdDataList();

            //return Task.CompletedTask;

        }

        async Task RefreshIdDataList()
        {
            using (await actionLock.LockAsync())
            {

                if (!this.refreshIdBtn.IsEnabled)
                    return;
                this.refreshIdBtn.IsEnabled = false;


                List<int> idComSources = new List<int>();
                string ip = Url.GetIpPort(this.LibraryManageUrlText.Text?.Any() != true
                    ? _clientSettings.LibraryManageUrl
                    : this.LibraryManageUrlText.Text);

                // 网络请求
                if (ip?.Any() == true)
                {

                    var hasExp = await Task.Run(async () =>
                    {
                        try
                        {
                            using HttpClient httplClient = new HttpClient()
                            {
                                Timeout = TimeSpan.FromMilliseconds(1500)
                            };
                            httplClient.BaseAddress = new Uri(@$"http://{ip}");

                            var str = await httplClient.GetStringAsync("/api/LibrarySettings/GetLibrarySettings");

                            var librarySettings = JsonConvert.DeserializeObject<MessageModel<LibrarySettings>>(str);

                            idComSources.AddRange(librarySettings?.response?.Clients?.Select(c => c.Id.ToInt()));
                            return false;
                        }
                        catch (Exception)
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                            {
                                this.idHitLabel.Content = "获取Id列表失败，请检查后台管理URL";
                            });
                        }

                        try
                        {
                            using HttpClient httplClient = new HttpClient()
                            {
                                Timeout = TimeSpan.FromMilliseconds(1500)
                            };
                            httplClient.BaseAddress = new Uri(@$"http://{ip.Split(":").First()}:5001");


                            var str = await httplClient.GetStringAsync("/api/LibrarySettings/GetLibrarySettings");

                            var librarySettings = JsonConvert.DeserializeObject<MessageModel<LibrarySettings>>(str);

                            idComSources.AddRange(librarySettings?.response?.Clients?.Select(c => c.Id.ToInt()));
                            return false;
                        }
                        catch (Exception)
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                            {
                                this.idHitLabel.Content = "获取Id列表失败，请检查后台管理URL";
                            });
                            return true;
                        }

                        return false;
                    });

                    if (!hasExp)
                    {

                        if (idComSources?.Any() != true)
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                            {
                                this.idHitLabel.Content = "未在后台配置Id";
                            });

                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                            {
                                this.idHitLabel.Content = "";
                            });

                        }
                    }
                }

                if (idComSources?.Any() != true)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        idComSources.Add(i + 1);
                    }
                }

                this.idCom.ItemsSource = idComSources;


                //this._clientSettings.Id = this._clientSettings.Id == 0 ? idComSources?.FirstOrDefault() ?? this._clientSettings.Id : this._clientSettings.Id;

                //this.idCom.Text = this._clientSettings.Id.ToString();


                this.refreshIdBtn.IsEnabled = true;
            }
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
            OnSettingsChange?.Invoke();
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
                this.ReaderActionUrlText.Text = this.LibraryManageUrlText.Text +
                                                @$"{(this.LibraryManageUrlText.Text.Last() == '/' ? "" : "/")}terminal";
                this._clientSettings.ReaderActionUrl = this.ReaderActionUrlText.Text;
            }

            RefreshIdList();
        }

        private void CheckDomain_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDomain(this.LibraryManageUrlText.Text))
            {
                MessageBox.Show("Url不是正确的http 或 https 格式", "错误");
            }
            else
            {
                RefreshIdList();
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
                startInfo.Arguments =
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                        "FollowSystemRunScript.vbs");
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

        public static bool IsDomain(string str)
        {
            if (str?.Any() != true || str.Contains(";"))
            {
                return false;
            }

            if (Regex.IsMatch(str,
                    @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RefreshIdBtn_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshIdList();
        }
    }
}