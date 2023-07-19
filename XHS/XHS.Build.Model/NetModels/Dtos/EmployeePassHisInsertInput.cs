using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    public  class EmployeePassHisInsertInput
    {
        public string ID { get; set; }
        public string attendprojid { get; set; }
        public string empname { get; set; }
        public decimal temperature { get; set; }
        public bool passflag { get; set; }
        public DateTime passdate { get; set; }
    }
}
