using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;
using XHS.Build.Model.NetModels.Dtos;

namespace XHS.Build.Services.ElecMeter
{
    public interface IElecMeterService:IBaseServices<GCElecMeterEntity>
    {
        Task<PageOutput<GCElecMeterPageListOutput>> GetList(int GROUPID, string keyword, int page, int size);

        Task<DataTable> GetGroupElecMeterCount();

        Task<List<GCElecMeterEntity>> GetListForJob();

        Task<int> RtdInsert(EmeterDataInput input);

        Task<List<GCElecMeterEntity>> GetDistinctElecMeterList(DateTime updatetime);


        Task<DataTable> GetElecMeterListBySiteId(int siteid);

        Task<DataTable> GetElecRtdData(int siteid, string emetercode);


        Task<DataTable> GetElecHisData(int siteid, string emetercode, DateTime time);

        //电表告警 设备占比
        Task<DataTable> GetSiteElecWarnChart(int siteid, DateTime startdate, DateTime enddate);


        //电表告警类型占比
        Task<DataTable> GetSiteElecWarnTypeChart(int siteid, DateTime startdate, DateTime enddate);


        //电表告警列表
        Task<DataTable> GetSiteElecWarnList(int siteid, string emetercode, DateTime startdate, DateTime enddate, int pageindex=1, int pagesize=10);

        Task<DataTable> GetSiteElecLatestWarnList(int siteid, string emetercode);
    }
}
