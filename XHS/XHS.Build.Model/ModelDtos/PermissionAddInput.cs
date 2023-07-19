using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class PermissionAddInput
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
        public string ParentId { get; set; } = "0";

        /// <summary>
        /// icon图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// 是否是按钮
        /// </summary>
        public bool IsButton { get; set; }

        public bool Enabled { get; set; }
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
        public List<string> ApiIds { get; set; }

        public string SystemId { get; set; }

        public string MENUID { get; set; }
    }

    public class PermissionUpdateInput
    {
        public string Id { get; set; }

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
        /// 是否是按钮
        /// </summary>
        public bool IsButton { get; set; }

        public bool Enabled { get; set; }
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
        public List<string> ApiIds { get; set; }
        public string SystemId { get; set; }
        public string MENUID { get; set; }
    }
}
