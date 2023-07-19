using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [SugarTable("T_CT_User")]
    public class CTUserEntity
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int USERID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string ROLEID { get; set; }
        /// <summary>
        /// UUID
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
        /// 状态 0:有效;1:无效 2:删除
        /// </summary>
        public short status { get; set; } = 0;
        /// <summary>
        /// 手机
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 所属区域
        /// </summary>
        public string userregion { get; set; } = "0";
        /// <summary>
        /// 操作人
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 验证码
        /// </summary>
        public string verifycode { get; set; }
        /// <summary>
        /// 验证码生成时间
        /// </summary>
        public DateTime? verifytime { get; set; }

    }

    /// <summary>
    /// 用户列表实体
    /// </summary>
    public class CTUserListEntity : CTUserEntity
    {
        /// <summary>
        /// 分组简称
        /// </summary>
        public string groupshortname { get; set; }
        /// <summary>
        /// 所属区域
        /// </summary>
        public string userregionname { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string rolename { get; set; }
    }
    /// <summary>
    /// 修改密码实体
    /// </summary>
    public class CTUserPwd
    {
        /// <summary>
        /// 新密码
        /// </summary>
        public string newpwd { get; set; }
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldpwd { get; set; }
    }
}