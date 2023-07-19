using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Event
{
    public class EventService : BaseServices<BaseEntity>, IEventService
    {
        private readonly IBaseRepository<BaseEntity> _envRepository;

        public EventService(IBaseRepository<BaseEntity> envRepository)
        {
            _envRepository = envRepository;
            _envRepository.CurrentDb = "XHS_Analyse";

        }

        public  Task<DataTable> GetList(string recordNumber, int eventlevel, int status, DateTime? operatedate)
        {
            return _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spEventlist", new { recordNumber = recordNumber, eventlevel = eventlevel, status= status, operatedate= operatedate });
        }


        /// <summary>
        /// 行政处罚数据追加
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> PenaltyInsert(PenaltyDataInput dto)
        {
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.NeetReturnValue();
            await _envRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spPenaltyInsert", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 事件追加
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> AddEvent(EventDataInput dto)
        {
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Remove("remark");
            sp.NeetReturnValue();
            await _envRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spEventAdd", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 事件关闭
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> CloseEvent(EventDataInput dto)
        {
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Remove("SPID");
            sp.Remove("SPtype");
            sp.Remove("limitdate");
            sp.NeetReturnValue();
            await _envRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spEventClose", sp.Params);
            return sp.ReturnValue;
        }


        /// <summary>
        /// 标前标后人员对比数据
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> BidDataPush(BidDataInput dto)
        {
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.NeetReturnValue();            
            await _envRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spBidDataAdd", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 人员结果数据追加
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> PersonnelResultInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _envRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spPersonnelResultInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<DataTable> UpEvent(string starttime)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUpEvent", new { starttime = starttime });
        }


        public async Task<List<AYEventType>> GetParentEventType()
        {
            return await _envRepository.Db.Queryable<AYEventType>().Where(ii => ii.ETID.Length == 2).ToListAsync();
        }
    }
}