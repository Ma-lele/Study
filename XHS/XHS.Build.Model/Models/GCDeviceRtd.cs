using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    
    [SugarTable("T_GC_DeviceRtd")]
    public class GCDeviceRtd
    {
        public string devicecode { get; set; }
        public int SITEID { get; set; }
        public decimal lng { get; set; }
        public decimal lat { get; set; }
        public decimal tsp { get; set; }
        public decimal pm2_5 { get; set; }
        public decimal pm10 { get; set; }
        public decimal atmos { get; set; }
        public int direction { get; set; }
        public decimal noise { get; set; }
        public decimal dampness { get; set; }
        public decimal temperature { get; set; }
        public decimal speed { get; set; }
        public DateTime updatetime { get; set; }
    }
}
