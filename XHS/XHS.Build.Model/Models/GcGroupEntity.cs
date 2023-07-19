using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 分组实体
    /// </summary>    
    [SugarTable("T_GC_Group")]
    public class GcGroupEntity
    {
        /// <summary>
        /// 分组编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int GROUPID { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string groupname { get; set; }
        /// <summary>
        /// 分组简称
        /// </summary>
        public string groupshortname { get; set; }
        /// <summary>
        /// 分组简介
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public int city { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public int district { get; set; }
        /// <summary>
        /// 后缀，用于特殊画面处理，比如top_taizhou
        /// </summary>
        public string suffix { get; set; } = "";
        /// <summary>
        /// logo图片
        /// </summary>
        public string logo { get; set; } = "智慧工地云平台";
        /// <summary>
        /// 状态 0:正常  1:冻结
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 是否有tsp模块 0：无 1：有
        /// </summary>
        public int hastsp { get; set; } = 1;
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 特种设备平台对接用户名
        /// </summary>
        public string specialuser { get; set; } = "苏州新合盛";
        /// <summary>
        /// 特种设备平台对接密码
        /// </summary>
        public string specialpsw { get; set; } = "123";
        /// <summary>
        /// 车辆冲洗抓拍对接secret
        /// </summary>
        public string washsecret { get; set; } = "";
        /// <summary>
        /// 新合盛实名制的用户名和密码，||分隔
        /// </summary>
        public string attenduserpsd { get; set; }
        /// <summary>
        /// 竖线分隔经纬度，用于市平台地图打点
        /// </summary>
        public string lnglat { get; set; }
        /// <summary>
        /// 所属机构编号(用于对接省平台)
        /// </summary>
        public string belongto { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string CityName { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string DistrictName { get; set; }
    }

    public class GroupInputDto : GcGroupEntity
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }

    public class GroupSiteCount
    {
        public int GROUPID { get; set; }

        public string groupshortname { get; set; }

        public int count { get; set; }

        public string suffix { get; set; }

    }


    public class GroupHelmetBeaconCount
    {
        public int GROUPID { get; set; }

        public string groupshortname { get; set; }

        public string groupname { get; set; }

        public string hasfogkickline { get; set; }
        public int count { get; set; }
    }
}
