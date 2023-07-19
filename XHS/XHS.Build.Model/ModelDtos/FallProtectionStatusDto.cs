using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class FallProtectionStatusDto
    {
        public string deviceId { get; set; }
        public DateTime creationTime { get; set; }
        /// <summary>
        /// 0：离线  1：在线
        /// </summary>
        public int onlineStatus { get; set; }
        public int battery { get; set; }
        public string @long { get; set; }
        public string lat { get; set; }
    }
}
