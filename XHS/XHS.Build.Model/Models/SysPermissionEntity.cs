using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 菜单模块表
    /// </summary>
    [SugarTable("T_SysPermission")]
    public class SysPermissionEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上级id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// icon图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// web页面是否可选 可操作
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 是否是按钮
        /// </summary>
        public bool IsButton { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// js方法名
        /// </summary>
        public string Func { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide { get; set; }
        /// <summary>
        /// 关联api接口
        /// </summary>
        public string ApiId { get; set; }
        /// <summary>
        /// 关联系统
        /// </summary>
        public string SystemId { get; set; }
        /// <summary>
        /// 保留老系统字段
        /// </summary>
        public string MENUID { get; set; }

        public bool iskeepalive { get; set; }

        public bool HideTab { get; set; }



        [SugarColumn(IsIgnore = true)]
        public List<string> PidArr { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<string> PnameArr { get; set; } = new List<string>();
        [SugarColumn(IsIgnore = true)]
        public List<string> PCodeArr { get; set; } = new List<string>();
        [SugarColumn(IsIgnore = true)]
        public string MName { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool hasChildren { get; set; } = true;

        /// <summary>
        /// 关联api接口(一对多数组)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<string> ApiIds { get; set; }
    }
}
