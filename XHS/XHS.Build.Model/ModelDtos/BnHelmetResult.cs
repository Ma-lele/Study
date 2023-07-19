using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 摄像头Bean
    /// </summary> 
    public class BnHelmetResult<T>
    {
        public string beaconcode { get; set; }

        public string beaconname { get; set; }

        public string equiptype { get; set; }

        public int beaconcount { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> data { get; set; }
    }
}
