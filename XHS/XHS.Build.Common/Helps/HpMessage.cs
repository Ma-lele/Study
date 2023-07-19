using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// 消息助手
    /// </summary>
    public partial class HpMessage 
    {
        private static readonly Dictionary<int, string> _messege = new Dictionary<int, string>{
            {0,"发生错误！"},
            {-1,"所选的设备无法使用，请刷新后再试！"},
            {-2,"手机号码重复，请重新输入！"},
            {-3,"用户名重复，请重新输入！"},
            {-4,"{0}必须大于{1}！"},
            {-5,"上传文件非法！"},
            {-6,"请上传JPG文件！"},
            {-7,"请上传mp4文件！"},
            {-8,"统一社会信用代码重复，请重新输入！"},
            {-21,"设备编号重复，请重新输入！"},
            {-22,"已有相同的序列号/信道，请重新输入！"},
            {-23,"正在处理其他报警！"},
            {-24,"停车场编号非法！"},
            {-25,"系统不支持该功能！"},
            {-26,"若要修改监测对象，请先解绑该特种设备的备案！"},
            {-27,"设备编号不匹配."},
            {-28,"资源数量超限."},
            {-29,"该设备不为从设备."},
            //30-35已占用
            {-994,"请求非法。"},
            {-995,"非法提交方式！请重新登录后再试。"},
            {-996,"没有访问权限！请重新登录后再试。"},
            {-997,"非法提交！请重新登录后再试。"},
            {-998,"会话过期！请重新登录后再试。"},
            {-999,"发生了不可预知的错误！请重新登录后再试。"},

            //API错误 
            {-20001,"appkey或secret非法."},
            {-20002,"token非法."},
            {-20003,"token过期."},
            {-20004,"重复提交."},
            {-20005,"来源不在IP白名单之列."},
            {-20006,"请求数据非法."},
            {-20007,"短时间内频繁获取授权."},
        };

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string getMessage(int key)
        {
            return getMessage(key, null);
        }


        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string getMessage(int key, string[] param)
        {
            if (_messege.ContainsKey(key))
            {
                if (param == null)
                    return _messege[key];
                else
                    return string.Format(_messege[key], param);
            }
            else
                return key.ToString();
        }
    }
}
