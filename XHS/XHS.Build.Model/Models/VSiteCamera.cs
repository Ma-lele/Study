using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 摄像头视图
    /// </summary>
    public class VSiteCamera
    {
        /// <summary>
        /// 摄像头编号
        /// </summary>
        public int CAMERAID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
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
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 监测对象名
        /// </summary>
        public string sitename { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
        /// <summary>
        /// 摄像头名
        /// </summary>
        public string cameraname { get; set; }
        /// <summary>
        /// 摄像头类型
        /// </summary>
        public int cameratype { get; set; }
        /// <summary>
        /// 摄像头编号
        /// </summary>
        public string cameracode { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public string channel { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int bdel { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime operatedate { get; set; }
        /// <summary>
        /// 监测对象区域
        /// </summary>
        public int sitearea { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string cameraparam { get; set; }
        /// <summary>
        /// 关联设备编号
        /// </summary>
        public string devcode { get; set; } 
    }

    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Camera")]
    public class GCCameraEntity
    {

        /// <summary>
        /// 摄像头编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int CAMERAID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; } = 0;
        /// <summary>
        /// 1:萤石云 2:中兴全球眼3:海康7200,4:hls直播,5:华为千里眼,6:阿启视,7:海康8700hls,8:海康8200,9:大华8,10:大华9,11:南京城建,12:渣土车监控,13:国控
        /// </summary>
        public int cameratype { get; set; } = 1;
        /// <summary>
        /// 摄像头名称
        /// </summary>
        public string cameraname { get; set; }
        /// <summary>
        /// 摄像头编号
        /// </summary>
        public string cameracode { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public int channel { get; set; } = 1;
        /// <summary>
        /// 0：正常 1：无效 2：拆除
        /// </summary>
        public int bdel { get; set; } = 0;
        /// <summary>
        /// 0:离线,1:在线
        /// </summary>
        public int bonline { get; set; } = 1;
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string cameraparam { get; set; } = "";
        /// <summary>
        /// 用第一条数据的时间作为开始时间
        /// </summary>
        public DateTime? camerastart { get; set; }
        /// <summary>
        /// 关联设备编号
        /// </summary>
        public string devcode { get; set; }
    }
}
    

