using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.ModelDtos
{
    public class SiteDto
    {
        /// <summary>
        /// 所属机构编码
        /// </summary>
        [Required(ErrorMessage = "所属机构编码不能为空！")]
        public string belongto { get; set; }
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        [Required(ErrorMessage = "安全监督备案号不能为空！")]
        public string recordNumber { get; set; }
        public string siteajcode { get; set; }
        /// <summary>
        /// 项目名
        /// </summary>
        [Required(ErrorMessage = "项目名不能为空！")]
        public string sitename { get; set; }
        /// <summary>
        /// 项目地址
        /// </summary>
        [Required(ErrorMessage = "项目地址不能为空！")]
        public string  siteaddr { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [Required(ErrorMessage = "经度不能为空！")]
        public Decimal sitelng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [Required(ErrorMessage = "纬度不能为空！")]
        public Decimal sitelat { get; set; }

        public string @operator { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        [Required(ErrorMessage = "项目经理不能为空！")]
        public string contact { get; set; }
        public int isself { get; set; } = 9;

        public int isgreen { get; set; } = 9;
        /// <summary>
        /// 项目经理电话
        /// </summary>
        [Required(ErrorMessage = "项目经理电话不能为空！")]
        public string tel { get; set; }
        /// <summary>
        /// 占地面积
        /// </summary>
        [Required(ErrorMessage = "占地面积不能为空！")]
        public int floorarea { get; set; }
        /// <summary>
        /// 建筑面积
        /// </summary>
        [Required(ErrorMessage = "建筑面积不能为空！")]
        public int buildingarea { get; set; }

        public DateTime? startdate { get; set; }

        public DateTime? enddate { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        [Required(ErrorMessage = "计划开始时间不能为空！")]
        public DateTime planstartdate { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        [Required(ErrorMessage = "计划结束时间不能为空！")]
        public DateTime planenddate { get; set; }
        public int bcityctrl { get; set; } = 0;

        public int projstatus { get; set; } = 2;
        public string sitecode { get; set; }
        public float sitecost { get; set; }
        /// <summary>
        /// 建设单位社会统一信用代码
        /// </summary>
       // [Required(ErrorMessage = "建设单位社会统一信用代码不能为空！")]
        public string constructorcode { get; set; }
        /// <summary>
        /// 建设单位公司名
        /// </summary>
        //[Required(ErrorMessage = "建设单位公司名不能为空！")]
        public string constructorname { get; set; }
        /// <summary>
        /// 建设单位联系人
        /// </summary>
       // [Required(ErrorMessage = "建设单位联系人不能为空！")]
        public string constructorcontact { get; set; }
        /// <summary>
        /// 建设单位电话
        /// </summary>
       // [Required(ErrorMessage = "建设单位电话不能为空！")]
        public string constructortel { get; set; }
        /// <summary>
        /// 施工单位社会统一信用代码
        /// </summary>
        [Required(ErrorMessage = "施工单位社会统一信用代码不能为空！")]
        public string buildercode { get; set; }
        /// <summary>
        /// 施工单位公司名
        /// </summary>
        [Required(ErrorMessage = "施工单位公司名不能为空！")]
        public string buildername { get; set; }
        /// <summary>
        /// 施工单位联系人
        /// </summary>
        [Required(ErrorMessage = "施工单位联系人不能为空！")]
        public string buildercontact { get; set; }
        /// <summary>
        /// 施工单位电话
        /// </summary>
        [Required(ErrorMessage = "施工单位电话不能为空！")]
        public string buildertel { get; set; }

        public string supervisorcode { get; set; }
        public string supervisorname { get; set; }
        public string supervisorcontact { get; set; }
        public string supervisortel { get; set; }
        public string surveycode { get; set; }
        public string surveyname { get; set; }
        public string surveycontact { get; set; }
        public string surveytel { get; set; }
        public string designcode { get; set; }
        public string designname { get; set; }
        public string designcontact { get; set; }
        public string designtel { get; set; }
    }
}
