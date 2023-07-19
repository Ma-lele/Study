using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 监测对象24小时污染数据Bean
    /// </summary>
    public class BnSite24Hours
    {
        
        public int SITEID { get; set; }
        
        public string datatime { get; set; }
        
        public Decimal tsp { get; set; }
        
        public Decimal pm2_5 { get; set; }
        
        public Decimal pm10 { get; set; }
        
        public Decimal noise { get; set; }
        
        public Decimal dampness { get; set; }
        
        public Decimal temperature { get; set; }
        
        public Decimal atmos { get; set; }
        
        public Decimal speed { get; set; }
        
        public string direction { get; set; }
        
        public Decimal tspmax { get; set; }
        
        public Decimal pm2_5max { get; set; }
        
        public Decimal pm10max { get; set; }
        
        public Decimal noisemax { get; set; }
    }
}
