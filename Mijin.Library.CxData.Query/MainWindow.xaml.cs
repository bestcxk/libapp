using System;
using System.Collections.Generic;
using System.Data;
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
using Bing.Extensions;
using Bing.Extensions.Datas;
using Mijin.Library.App.Driver;
using Newtonsoft.Json;

namespace Mijin.Library.CxData.Query;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
#if DEBUG
    private string connectSql =
        "Server=192.168.0.81;Database=mj;User=thirdlib;Password=Thirdlib@123;MultipleActiveResultSets=True;";
#else
    private string connectSql =
 "Server=11.176.26.71;Database=mj;User=thirdlib;Password=Thirdlib@123;MultipleActiveResultSets=True;";
#endif

    public class WindowData
    {
    }

    public MainWindow()
    {
        InitializeComponent();


        this.ConnectSqlTextBox.Text = connectSql;
        this.Sum.Text = "50";

        setConnectedStr(this.ConnectSqlTextBox.Text);


        //Insert();
    }

    public void setConnectedStr(string str)
    {
        // 配置连接字符串
        SQLHelper.ConnStrs.Remove("cx");

        SQLHelper.ConnStrs.Add("cx", str);
    }

    //查询
    public DataTable Find(int num, string sql = null)
    {

        try
        {
            return SQLHelper.QueryDataTable("cx",
               sql ?? $"SELECT top {num} * FROM visit_data_log ORDER BY visit_date_time desc", null, CommandType.Text);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.StackTrace, "查询数据失败");
            return null;
        }
    }

    /// <summary>
    /// 插入假数据
    /// </summary>
    public void Insert()
    {
        for (int i = 0; i < 1000; i++)
        {
            var date = DateTime.Now.AddMilliseconds(i * 100);
            CxEntity entity = new CxEntity()
            {
                user_id = i.ToString(),
                direction = "0",
                sn_name = "0",
                user_name = "0",
                card_id = "0",

                visit_date_time = date.ToString("yyyy-MM-dd HH:mm:ss"),
                visit_date = date.ToString("yyyy-MM-dd"),
                visit_time = date.ToString("HH:mm:ss"),
            };
            entity.WriteToDb();
        }
    }


    //设置中文标题
    public void Change_DataGrid_ColumnName_Ex(DataTable dgv)
    {
        dgv.Columns[0].ColumnName = "编号";
        dgv.Columns[1].ColumnName = "认证记录日期及时间";
        dgv.Columns[2].ColumnName = "认证记录日期";
        dgv.Columns[3].ColumnName = "认证记录时间";
        dgv.Columns[4].ColumnName = "方向";
        dgv.Columns[5].ColumnName = "设备名称";
        dgv.Columns[6].ColumnName = "设备序列号";
        dgv.Columns[7].ColumnName = "人员名称";
        dgv.Columns[8].ColumnName = "卡号（军官证号）";
    }

    private  void QueryBtn_Click(object sender, RoutedEventArgs e)
    {
        var button = (sender as Button)!;

        setConnectedStr(this.ConnectSqlTextBox.Text);

        button.Content = "查询中···";
        button.IsEnabled = false;

        if (Sum.Text.IsEmpty())
            Sum.Text = "50";

        var data = Find(int.Parse(Sum.Text));

        if(data != null)
        {
            Change_DataGrid_ColumnName_Ex(data);
            Show.ItemsSource = data.DefaultView;

            var dataTable =
                SQLHelper.QueryDataTable("cx", "select count(user_id) from visit_data_log", null, CommandType.Text);
            var count = dataTable.Rows[0].ItemArray.First();
            CountLabel.Content = count?.ToString();
        }
        

        button.Content = "查询";
        button.IsEnabled = true;
    }

    private void QuerySqlBtn_Click(object sender, RoutedEventArgs e)
    {
        var button = (sender as Button)!;

        setConnectedStr(this.ConnectSqlTextBox.Text);

        button.Content = "查询中···";
        button.IsEnabled = false;

        var data = Find(int.Parse(Sum.Text), QuerySqlInput.Text);

        if (data != null)
        {
            Change_DataGrid_ColumnName_Ex(data);
            Show.ItemsSource = data.DefaultView;

            var dataTable =
                SQLHelper.QueryDataTable("cx", "select count(user_id) from visit_data_log", null, CommandType.Text);
            var count = dataTable.Rows[0].ItemArray.First();
            CountLabel.Content = count?.ToString();
        }

        button.Content = "查询";
        button.IsEnabled = true;
    }
}