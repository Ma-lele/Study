using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Event
{
    public interface IEventService : IBaseServices<BaseEntity>
    {

        Task<DataTable> GetList(string recordNumber, int eventlevel, int status, DateTime? operatedate);

        Task<int> PenaltyInsert(PenaltyDataInput dto);

        Task<int> AddEvent(EventDataInput dto);

        Task<int> CloseEvent(EventDataInput dto);

        Task<int> BidDataPush(BidDataInput dto);

        Task<int> PersonnelResultInsert(List<SugarParameter> param);
        Task<DataTable> UpEvent(string starttime);

        Task<List<AYEventType>> GetParentEventType();

    }
}
