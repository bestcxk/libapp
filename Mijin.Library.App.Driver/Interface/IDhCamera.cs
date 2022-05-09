using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Interface
{
    /// <summary>
    /// 大华摄像头接口
    /// </summary>
    public interface IDhCamera
    {
        /// <summary>
        /// 人流量统计事件
        /// </summary>
        event Action<(int In, int Out)> OnPeopleInOut;

        /// <summary>
        /// 截取人脸事件
        /// </summary>
        event Action<Image> OnGetFaceImage;

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="netPort">端口</param>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        void Login(string ip, string netPort, string name, string password);

        /// <summary>
        /// 截取人脸
        /// </summary>
        void GetFace();

        /// <summary>
        /// 人流量统计
        /// </summary>
        void HumanSum();
    }
}
