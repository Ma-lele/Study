using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 摄像头Bean
    /// </summary> 
    public class BnCameraResult<T>
    {
        public string code { get; set; }

        public string msg { get; set; }
        public string hasptz { get; set; }
        public string hasplayback { get; set; }

        public string appKey { get; set; } //萤石云appkey
        public string accessToken { get; set; } //萤石云accessToken
        public string url { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> data { get; set; }
    }
}
