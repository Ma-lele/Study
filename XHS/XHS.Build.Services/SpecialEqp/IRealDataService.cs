using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.SpecialEqp
{
    public interface IRealDataService
    {
        Task<PageOutput<SpecialEqpData>> GetPage(string secode,string starttime,string endtime,int pageindex=1,int pagesize=10);
    }
}
