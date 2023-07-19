using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 监测点施工阶段历史Bean   
    /// </summary>
    public class BnSitePhaseHis
    {
        public ulong PHASEHISID { get; set; }
        
        public int PHASEID { get; set; }
      
        public int SITEID { get; set; }
        
        public int photocount { get; set; }
     
        public string phasenamefrom { get; set; }
        
        public string phasenameto { get; set; }
        
        public string phasepercent { get; set; }
        
        public DateTime phasedate { get; set; }
   
        public string username { get; set; }
       
        public DateTime operatedate { get; set; }
    }
}
