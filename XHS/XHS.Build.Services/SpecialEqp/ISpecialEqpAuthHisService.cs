using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public interface ISpecialEqpAuthHisService : IBaseServices<GCSpecialEqpAuthHisEntity>
    {
        /// <summary>
        /// 获取特种设备的上下机数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <param name="billdate"></param>
        /// <returns></returns>
        Task<DataTable> GetListAsync(int SITEID, string secode, DateTime billdate);
    }
}
