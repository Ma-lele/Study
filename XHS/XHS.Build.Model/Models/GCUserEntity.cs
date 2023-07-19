using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [SugarTable("T_GC_User")]
    public class GCUserEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public string USERID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public int ROLEID { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string UUID { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string pwd { get; set; }
        /// <summary>
        /// 状态 1:有效;0:无效
        /// </summary>
        public short status { get; set; } = 1;
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; } = 0;
        /// <summary>
        /// 手机
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 职位(考勤人员时，为建设方,施工方,设计方,勘察方,监理方)
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 0:绑定所有工地  1:绑定一个工地 2:绑定多个工地
        /// </summary>
        public short usersitetype { get; set; } = 2;
        /// <summary>
        /// 是否删除
        /// </summary>
        public short bdel { get; set; } = 0;

        
        public string userregion { get; set; }
    }

    /// <summary>
    /// 用户列表实体
    /// </summary>
    public class GCUserListEntity: GCUserEntity
    {
        /// <summary>
        /// 分组简称
        /// </summary>
        public string groupshortname { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string rolename { get; set; }
    }
}