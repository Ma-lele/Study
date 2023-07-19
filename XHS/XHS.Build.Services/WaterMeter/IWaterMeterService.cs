using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.WaterMeter
{
    public interface IWaterMeterService:IBaseServices<GCWaterMeterEntity>
    {
        Task<PageOutput<GCWaterMeterPageListOutput>> GetList(int GROUPID, string keyword, int page, int size);

        Task<DataTable> GetGroupWaterMeterCount();

        Task<List<GCWaterMeterEntity>> GetListForJob();

    }
}
