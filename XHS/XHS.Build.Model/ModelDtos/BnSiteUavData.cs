using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnSiteUavData
    {
        
        public long UAVDATAID { get; set; }
        
        public int SITEID { get; set; }
        
        public float latitude { get; set; }
        
        public float longitude { get; set; }
        
        public float altitude { get; set; }
        
        public decimal pm25 { get; set; }
        
        public decimal pm10 { get; set; }
        
        public DateTime createddate { get; set; }
        
        public string username { get; set; }
        
        public DateTime operatedate { get; set; }
    }
}
