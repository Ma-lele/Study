using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.WJWechat
{
    public class BnTplBase
    {
        /// <summary>
        /// 发送用户的openid
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 模板ID
        /// </summary>
        public string templateid { get; set; }
        /// <summary>
        /// 前言
        /// </summary>
        public string first { get; set; }
        /// <summary>
        /// 监测点位置
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// 报警时间
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 报警类型
        /// </summary>
        public string TYPE { get; set; }
        /// <summary>
        /// 报警内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}
