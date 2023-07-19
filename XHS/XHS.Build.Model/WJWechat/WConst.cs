using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.WJWechat
{
    /// <summary>
    /// 常量
    /// </summary>
    public partial class WConst
    {
        /// <summary>
        /// 错误
        /// </summary>
        public class Error
        {
            /// <summary>
            /// 获取access_token时AppSecret错误，或者access_token无效
            /// </summary>
            public const string CODE40001 = "40001";
            /// <summary>
            /// access_token超时
            /// </summary>
            public const string CODE42001 = "42001";
        }

        /// <summary>
        /// URL连接地址
        /// </summary>
        public class Url
        {
            /// <summary>
            /// 凭证获取地址
            /// </summary>
            public const string TOKEN = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            /// <summary>
            /// 菜单获取地址
            /// </summary>
            public const string MENU = @"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}";
            /// <summary>
            /// 模板消息发送地址
            /// </summary>
            public const string TEMPLATE_SEND = @"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
            /// <summary>
            /// 创建二维码地址
            /// </summary>
            public const string QRCODE_CREATE = @"https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";
            /// <summary>
            /// 生成二维码地址
            /// </summary>
            public const string QRCODE_SHOW = @"https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";
            /// <summary>
            /// 获取OAUTH凭证地址
            /// </summary>
            public const string OAUTH_TOKEN = @"https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            /// <summary>
            /// 获取微信用户信息地址
            /// </summary>
            public const string OAUTH_USER = @"https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";


        }
    }
}
