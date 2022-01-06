using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Helpers;

namespace Mijin.Library.App.Model
{
    
    /// <summary>
    /// 客户端设置
    /// </summary>
    public class ClientSettings : baseClientSettings
    {
        public ClientSettings()
        {
            // 获取base类的属性
            var settings = ReadToBaseModel();

            // 有值就反射赋值
            if (settings != null)
            {
                var type = typeof(baseClientSettings);
                foreach (var propertyInfo in type.GetProperties())
                {
                    propertyInfo.SetValue(this, propertyInfo.GetValue(settings));
                }
            }
        }

        /// <summary>
        /// 写入到本地文件 appsettings.json
        /// </summary>
        /// Exception 写入失败
        /// <returns></returns>
        public void Write()
        {
           FileHelper.WriteFile("./appsettings.json", Json.ToJson(this),Encoding.UTF8);
        }

        /// <summary>
        /// 读本地文件 appsettings.json ,如果没有则返回null
        /// </summary>
        /// <returns></returns>
        public baseClientSettings ReadToBaseModel()
        {
            // 是否含有 appsettings.json 文件
            if (File.Exists("./appsettings.json"))
            {
                try
                {
                    var settingsFile = FileHelper.ReadFile("./appsettings.json",Encoding.UTF8);
                    var data =  Json.ToObject<baseClientSettings>(settingsFile);
                    return data;
                }
                catch (Exception e)
                {
                    string addStr = "\r\n\r\n" + "//ErrorMsg：序列化失败，请检查Json 格式";
                    //FileHelper.FileAdd("./appsettings.json", addStr, Encoding.UTF8);
                    FileHelper.WriteFile("./appsettingsErrorMsg", e.ToString(),Encoding.UTF8);
                    return null;
                }
                //读取并序列化appsettings
            }
            else
            {
                return null;
            }
        }

    }
}

