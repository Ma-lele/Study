using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Site")]
    public class GCSiteEntity
    {
        private string _id;
        /// <summary>
        /// UID
        /// </summary>
        public string UID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    return MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SITEID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 父site编号，0为普通site，0以外为子site（扬尘监测点）
        /// </summary>
        public int PARENTSITEID { get; set; } = 0;
        /// <summary>
        /// 监测对象名称
        /// </summary>
        public string sitename { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int sitetype { get; set; }
        /// <summary>
        /// 所属区域
        /// </summary>
        public string sitearea { get; set; } = "";
        /// <summary>
        /// 地址
        /// </summary>
        public string siteaddr { get; set; } = "";
        /// <summary>
        /// 经度
        /// </summary>
        public float sitelng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float sitelat { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string contact { get; set; } = "";
        /// <summary>
        /// 项目经理电话
        /// </summary>
        public string tel { get; set; } = "";
        /// <summary>
        /// 建设单位
        /// </summary>
        public string constructor { get; set; } = "";
        /// <summary>
        /// 施工单位
        /// </summary>
        public string builder { get; set; } = "";
        /// <summary>
        /// 楼层面积
        /// </summary>
        public string floorarea { get; set; } = "";
        /// <summary>
        /// 施工面积
        /// </summary>
        public string buildingarea { get; set; } = "";
        /// <summary>
        /// 施工类型
        /// </summary>
        public string constructtype { get; set; } = "";
        /// <summary>
        /// 设备供应商
        /// </summary>
        public string serviceprovider { get; set; } = "";
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? startdate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? enddate { get; set; }
        /// <summary>
        /// 状态 0:正常  1:冻结 2：关闭 3：移除
        /// </summary>
        public int status { get; set; } = 0;
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; } = "";
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否监测TSP
        /// </summary>
        public short btsp { get; set; } = 0;
        /// <summary>
        /// 是否监测噪声
        /// </summary>
        public short bnoise { get; set; } = 0;
        /// <summary>
        /// 是否监测视频
        /// </summary>
        public short bvideo { get; set; } = 0;
        /// <summary>
        /// 是否监测风向
        /// </summary>
        public short bwind { get; set; } = 0;
        /// <summary>
        /// 是否监测温度
        /// </summary>
        public short btemphum { get; set; } = 0;
        /// <summary>
        /// 是否监测PM2.5
        /// </summary>
        public short bpm25 { get; set; } = 0;
        /// <summary>
        /// 是否监测PM10
        /// </summary>
        public short bpm10 { get; set; } = 0;
        /// <summary>
        /// 是否监测气压
        /// </summary>
        public short bpressure { get; set; } = 0;
        /// <summary>
        /// 施工阶段
        /// </summary>
        public string constructphase { get; set; } = "";
        /// <summary>
        /// 施工进度百分比
        /// </summary>
        public string phasepercent { get; set; } = "";
        /// <summary>
        /// 项目编号
        /// </summary>
        public string sitecode { get; set; } = "";
        /// <summary>
        /// 工程造价
        /// </summary>
        public string sitecost { get; set; } = "";
        /// <summary>
        /// 项目经理身份证
        /// </summary>
        public string contactidcard { get; set; } = "";
        /// <summary>
        /// 停车场唯一编号(冲洗抓拍)
        /// </summary>
        public string parkkey { get; set; } = "";
        /// <summary>
        /// 停车场唯一编号(未冲洗抓拍)
        /// </summary>
        public string parkkeynowash { get; set; } = "";
        /// <summary>
        /// 是否要推到市局
        /// </summary>
        public int bpush { get; set; }
        /// <summary>
        /// 是否市管项目 1：是  0：否
        /// </summary>
        public int bcityctrl { get; set; }
        /// <summary>
        /// 对接考勤系统的项目编号
        /// </summary>
        public string attendprojid { get; set; } = "";
        /// <summary>
        /// 1：群耀  2：大运  3：比利时 4：都驰 5:新合盛
        /// </summary>
        public string attendprojtype { get; set; } = "";
        /// <summary>
        /// 对接无锡考勤系统的项目编号
        /// </summary>
        public string attendprojidwuxi { get; set; } = "";
        /// <summary>
        /// 对接安全帽识别的项目编号
        /// </summary>
        public string helmetprojid { get; set; } = "";
        /// <summary>
        /// 对接人车分流的项目编号
        /// </summary>
        public string trespasserprojid { get; set; } = "";
        /// <summary>
        /// 对接陌生人的项目编号
        /// </summary>
        public string strangerprojid { get; set; } = "";
        /// <summary>
        /// 对接火警识别的项目编号
        /// </summary>
        public string fireprojid { get; set; } = "";
        /// <summary>
        /// 对接升降机人数识别的项目编号
        /// </summary>
        public string liftoverprojid { get; set; } = "";
        /// <summary>
        /// 晨会交底对接项目编号
        /// </summary>
        public string amdiscloseprojid { get; set; } = "";
        /// <summary>
        /// 非法车辆对接的项目编号
        /// </summary>
        public string illegalcarprojid { get; set; } = "";

        /// <summary>
        /// 烟雾对接的项目编号
        /// </summary>
        public string smokeprojid { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string siterate { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string szhelmetnamepwd { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public DateTime? apiupdatedate { get; set; }

        /// <summary>
        /// 所属市
        /// </summary>
        public string sitecity { get; set; }
        /// <summary>
        /// 所属区
        /// </summary>
        public string siteregion { get; set; }
        /// <summary>
        /// 所属街道
        /// </summary>
        public string siteblock { get; set; }
        /// <summary>
        /// 消防对接单位ID
        /// </summary>
        public string firectrlprojid { get; set; } = "";
        /// <summary>
        /// 密闭运输绑定的工地ID
        /// </summary>
        public string airtightprojid { get; set; } = "";
        /// <summary>
        /// 围挡喷淋绑定的工地ID
        /// </summary>
        public string sprayprojid { get; set; } = "";
        /// <summary>
        /// 反光衣绑定的工地ID
        /// </summary>
        public string vestprojid { get; set; } = "";
        /// <summary>
        /// 裸土覆盖绑定的工地ID
        /// </summary>
        public string soilprojid { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string recordNumber { get; set; }
        /// <summary>
        /// 动态考核的appkey
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// 动态考核的secret
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 是否是示范片区（暂不使用）
        /// </summary>
        public short bexample { get; set; } = 0;
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string siteajcode { get; set; }
        /// <summary>
        /// 项目状态：1.待审、2.在建、3.停工、4.终止安监、5.竣工
        /// </summary>
        public int projstatus { get; set; }
        /// <summary>
        /// 是否对接过区平台（灌南对接过）
        /// </summary>
        public short isself { get; set; } = 0;
        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime? planstartdate { get; set; }
        /// <summary>
        /// 计划结束日期
        /// </summary>
        public DateTime? planenddate { get; set; }
        /// <summary>
        /// 是否绿色示范工地
        /// </summary>
        public short isgreen { get; set; } = 0;
        /// <summary>
        /// 集成商
        /// </summary>
        public string integrator { get; set; }
        /// <summary>
        /// 动态考核url
        /// </summary>

        [SugarColumn(IsIgnore = true)]
        public string url { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DustSiteAddInput
    {
        /// <summary>
        /// 
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PARENTSITEID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sitename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string siteshortname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sitetype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string siteaddr { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public float sitelng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float sitelat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serviceprovider { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startdate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? enddate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @operator { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public DateTime operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public int bpush { get; set; }
        /// <summary>
        /// 所属市
        /// </summary>
        public string sitecity { get; set; } = "0";
        /// <summary>
        /// 所属区
        /// </summary>
        public string siteregion { get; set; } = "0";
        /// <summary>
        /// 所属街道
        /// </summary>
        public string siteblock { get; set; } = "0";
    }

    /// <summary>
    /// 工地及五方人员实体
    /// </summary>
    public class GCSiteAndCompanyEntity : GCSiteEntity
    {
        /// <summary>
        /// 五方列表
        /// </summary>
        public List<GCSiteCompanyEntity> sitecompany { get; set; }
    }

}
