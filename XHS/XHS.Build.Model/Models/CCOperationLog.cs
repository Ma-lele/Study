using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CC_OperationLog")]
    public class CCOperationLog
    {
        public int LOGID { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeName type { get; set; }
        public string url { get; set; }
        public string @params { get; set; }
        public string ip { get; set; }
        public string operation { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }

    public class LogCountDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TypeName type { get; set; } = TypeName.其他;
        public int Count { get; set; }
    }

    public enum TypeName
    {
        登录 = 0,
        添加 = 1,
        修改 = 2,
        删除 = 3,
        批处理 = 4,
        移动端 = 5,
        系统异常 = 6,
        数据接收服务端 = 7,
        其他 = 9,
        全部 = 999
    }
}
