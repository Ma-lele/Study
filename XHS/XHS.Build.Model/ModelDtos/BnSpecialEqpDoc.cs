using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 特种设备文件Bean
    /// </summary> 
    public class BnSpecialEqpDoc
    {
        public string SEDOCID { get; set; }

        public int filetype { get; set; }

        public string filename { get; set; }

        public string fileex { get; set; }

        public DateTime updatedate { get; set; }
    }
}
