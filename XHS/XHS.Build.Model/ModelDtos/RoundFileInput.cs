using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 移动巡检上传图片
    /// </summary>
    public class RoundFileInput
    {
        public int SITEID { get; set; }
        public long ROUNDID { get; set; }
        public string username { get; set; }
        public string filename { get; set; }
        public string fileString { get; set; }
        public int filesize { get; set; }
    }
}
