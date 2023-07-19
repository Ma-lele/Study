using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 新临边防护DTO
    /// </summary>
    public class AlertInputDto
    {
        public string deviceId { get; set; }
        public string alarmId { get; set; }
        public DateTime creationTime { get; set; }
    }
}
