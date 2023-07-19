using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.NetModels
{
    public class EmployeeEntity
    {
        public string ID { get; set; }
        public int GROUPID { get; set; }
        public string realname { get; set; }
        public string mobile { get; set; }
        public string sex { get; set; }
        public DateTime birthday { get; set; }
        public string address { get; set; }
        public string ethnic { get; set; }
        public DateTime idstartdate { get; set; }
        public DateTime idenddate { get; set; }
        public string publisher { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string image { get; set; }
        public string jsonall { get; set; }
        public string @operator { get; set; }
    }
}
