using Bing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsUtil
{
    public class ImageHelper
    {
        /// <summary>
        /// 根据base64字符串返回一个封装好的GDI+位图。
        /// </summary>
        /// <param name="base64string">可转换成位图的base64字符串。</param>
        /// <returns>Bitmap对象，若base64string为空则返回null</returns>
        public static Bitmap GetImageFromBase64(string base64string)
        {
            if (base64string.IsEmpty())
            {
                return null;
            }

            base64string = base64string.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "")
                .Replace("data:image/jpeg;base64,", ""); //将base64头部信息替换

            byte[] b = Convert.FromBase64String(base64string);
            MemoryStream ms = new MemoryStream(b);
            Bitmap bitmap = new Bitmap(ms);
            return bitmap;
        }

        /// <summary>
        /// 将图片转换成base64字符串。
        /// </summary>
        /// <param name="imagefile">需要转换的图片文件。</param>
        /// <returns>base64字符串,若不存在文件和转换错误则返回null</returns>
        public static string GetBase64FromImage(string imagefile)
        {
            string strbaser64 = "";

            if (!File.Exists(imagefile))
            {
                return null;
            }

            try
            {
                Bitmap bmp = new Bitmap(imagefile);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int) ms.Length);
                    ms.Close();

                    strbaser64 = Convert.ToBase64String(arr);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return strbaser64;
        }

        /// <summary>
        /// Bitmap 转 base64字符串
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static string ToBase64Str(Bitmap bmp)
        {
            if (bmp.IsNull())
            {
                return null;
            }

            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int) ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ImgToBase64String 转换失败 Exception:" + ex.Message);
                return "";
            }
        }
    }
}