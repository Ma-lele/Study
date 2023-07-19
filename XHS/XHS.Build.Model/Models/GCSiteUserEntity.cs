using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{


    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SiteUser")]
    public class GCSiteUserEntity
    {
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string USERID { get; set; }
    }

    /// <summary>
    /// 共通部分
    /// </summary>
    public class SiteUserComm
    {
        /// <summary>
        /// 是否接收告警（实际发送告警时不看这个字段）
        /// </summary>
        public string bwarns { get; set; }
        /// <summary>
        /// 1:离线报警
        /// </summary>
        public string troubles { get; set; }
        /// <summary>
        /// 111:离线超过XX小时警告2
        /// </summary>
        public string trouble2s { get; set; }
        /// <summary>
        /// 112:离线超过XX小时警告3
        /// </summary>
        public string trouble3s { get; set; }
        /// <summary>
        /// 2:超标报警
        /// </summary>
        public string pms { get; set; }
        /// <summary>
        /// 32:车辆未冲洗报警
        /// </summary>
        public string unwashs { get; set; }
        /// <summary>
        /// 31:车辆冲洗设备离线报警
        /// </summary>
        public string washoffs { get; set; }
        /// <summary>
        /// 311:冲洗设备离线超过XX小时警告2
        /// </summary>
        public string washoff2s { get; set; }
        /// <summary>
        /// 312:冲洗设备离线超过XX小时警告3
        /// </summary>
        public string washoff3s { get; set; }
        /// <summary>
        /// 4:特种设备报警
        /// </summary>
        public string specs { get; set; }
        /// <summary>
        /// 42:防倾翻报警
        /// </summary>
        public string tipovers { get; set; }
        /// <summary>
        /// 43:特种设备未安装提醒
        /// </summary>
        public string nospec1s { get; set; }
        /// <summary>
        /// 44:钢丝绳报警
        /// </summary>
        public string cable1s { get; set; }
        /// <summary>
        /// 45:卸料报警
        /// </summary>
        public string unloads { get; set; }
        /// <summary>
        /// 46:升降机人数超载
        /// </summary>
        public string overloads { get; set; }
        /// <summary>
        /// 5:临边围挡报警
        /// </summary>
        public string fences { get; set; }
        /// <summary>
        /// 61:安全帽佩戴识别
        /// </summary>
        public string helmets { get; set; }
        /// <summary>
        /// 62:陌生人进场识别
        /// </summary>
        public string strangers { get; set; }
        /// <summary>
        /// 63:人车分流识别
        /// </summary>
        public string trespassers { get; set; }
        /// <summary>
        /// 64:堆场火灾
        /// </summary>
        public string fires { get; set; }
        /// <summary>
        /// 64:堆场火灾
        /// </summary>
        public string cameraoffs { get; set; }
        /// <summary>
        /// 发送微信预警的同时发送短信预警
        /// </summary>
        public string bsmss { get; set; }
        /// <summary>
        /// 是否有权限启动雾炮
        /// </summary>
        public string bstartfogs { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 区域闯入
        /// </summary>
        public string invades { get; set; }
        /// <summary>
        /// 黄土未覆盖
        /// </summary>
        public string soils { get; set; }
        /// <summary>
        /// 密闭运输
        /// </summary>
        public string airtights { get; set; }
        /// <summary>
        /// 反光衣
        /// </summary>
        public string vests { get; set; }
    }

    /// <summary>
    /// 保存用实体
    /// </summary>
    public class SiteUserInput : SiteUserComm
    {
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string USERIDS { get; set; }

    }
    /// <summary>
    /// 保存用实体
    /// </summary>
    public class UserSiteInput : SiteUserComm
    {
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public string SITEIDS { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int USERID { get; set; }

    }
}
