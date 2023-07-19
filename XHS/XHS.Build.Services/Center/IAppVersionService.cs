using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Center
{
    public interface IAppVersionService:IBaseServices<BaseEntity>
    {
    

        /// <summary>
        /// 获取监测点下临边围挡
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> GetAppVersion(string domain, string type);

    }
}
