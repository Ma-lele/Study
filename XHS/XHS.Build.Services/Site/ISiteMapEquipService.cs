using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Site
{
    public interface ISiteMapEquipService : IBaseServices<GCSiteMapEquipEntity>
    {
        /// <summary>
        /// 获取监测点下设备地图点位
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getListBySite(int SITEID);
    }
}
