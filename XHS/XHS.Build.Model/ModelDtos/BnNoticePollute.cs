using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 污染超标公告Bean
    /// </summary>
    public class BnNoticePollute
    {
        
        public int GROUPID { get; set; }
        
        public int SITEID { get; set; }
        
        public string datatime { get; set; }
        
        public string datatype { get; set; }
        
        public int tsp { get; set; }
        
        public int pm2_5 { get; set; }
        
        public int pm10 { get; set; }
        
        public Decimal noise { get; set; }
        
        public string username { get; set; }
    }
}
