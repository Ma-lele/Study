using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using System;
using XHS.Build.Services.Event;
using XHS.Build.Model.NetModels.Dtos;
using System.Collections.Generic;
using XHS.Build.Common.Auth;
using XHS.Build.Net.Attributes;
using XHS.Build.Common.Sqlsugar;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 事件接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserKey _userKey;
        public EventController(IEventService eventService, IUserKey userKey)
        {
            _eventService = eventService;
            _userKey = userKey;
        }

        /// <summary>
        /// 事件数据取得接口
        /// </summary>
        /// <param name="recordNumber"></param>
        /// <param name="warnlevel"></param>
        /// <param name="status"></param>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetEventList(string recordNumber,int warnlevel=0,int status=0,DateTime? operatedate=null)
        {
            return ResponseOutput.Ok(await _eventService.GetList(recordNumber, warnlevel, status, operatedate));            
        }

        /// <summary>
        /// 事件追加接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddEvent(EventDataInput dto)
        {
            dto.@operator = _userKey.Name;
           
            //List<SugarParameter> param = new List<SugarParameter>();
            //param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
            //param.Add(new SugarParameter("@eventcode", dto.eventcode));
            //param.Add(new SugarParameter("@handler", dto.handler));
            //param.Add(new SugarParameter("@SPID", dto.SPID));
            //param.Add(new SugarParameter("@SPtype", dto.SPtype));
            //param.Add(new SugarParameter("@limitdate", dto.limitdate));
            //param.Add(new SugarParameter("@operator", _userKey.Name));
            int retDB = await _eventService.AddEvent(dto);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 事件关闭接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CloseEvent(EventDataInput dto)
        {
            dto.@operator = _userKey.Name;
            //List<SugarParameter> param = new List<SugarParameter>();
            //param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
            //param.Add(new SugarParameter("@eventcode",  dto.eventcode));
            //param.Add(new SugarParameter("@handler", dto.handler));
            //param.Add(new SugarParameter("@remark", dto.remark));
            //param.Add(new SugarParameter("@operator", _userKey.Name));
            int retDB = await _eventService.CloseEvent(dto);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 行政处罚数据上传接口
        /// </summary>
        /// <param name="recordNumber"></param>
        /// <param name="warnlevel"></param>
        /// <param name="status"></param>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Penalty(PenaltyDataInput dto)
        {

            dto.@operator = _userKey.Name;
            //List<SugarParameter> param = new List<SugarParameter>();
            //param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
            //param.Add(new SugarParameter("@penaltycode", dto.penaltycode));
            //param.Add(new SugarParameter("@content", dto.content));
            //param.Add(new SugarParameter("@penaltydate", dto.penaltydate));
            //param.Add(new SugarParameter("@operator", _userKey.Name));
            int retDB = await _eventService.PenaltyInsert(dto);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 标前标后人员对比数据上传接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> BidDataPush(BidDataInput dto)
        {
            dto.@operator = _userKey.Name;
            //List<SugarParameter> param = new List<SugarParameter>();
            //param.Add(new SugarParameter("@belongto", dto.belongto));
            //param.Add(new SugarParameter("@recordnumber", dto.recordNumber));
            //param.Add(new SugarParameter("@projectname", dto.projectname));
            //param.Add(new SugarParameter("@projectcode", dto.projectcode));
            //param.Add(new SugarParameter("@companytype", dto.companytype));
            //param.Add(new SugarParameter("@bidprojectname", dto.bidprojectname));
            //param.Add(new SugarParameter("@bidcompanyname", dto.bidcompanyname));
            //param.Add(new SugarParameter("@bidperson", dto.bidperson));
            //param.Add(new SugarParameter("@ajprojectname", dto.ajprojectname));
            //param.Add(new SugarParameter("@ajstatus", dto.ajstatus));
            //param.Add(new SugarParameter("@buildcompanyname", dto.buildcompanyname));
            //param.Add(new SugarParameter("@buildperson", dto.buildperson));
            //param.Add(new SugarParameter("@buildresult", dto.buildresult));
            //param.Add(new SugarParameter("@buildcheck", dto.buildcheck));
            //param.Add(new SugarParameter("@ajcompanyname", dto.ajcompanyname));
            //param.Add(new SugarParameter("@ajperson", dto.ajperson));
            //param.Add(new SugarParameter("@ajresult", dto.ajresult));
            //param.Add(new SugarParameter("@ajcheck", dto.ajcheck));
            //param.Add(new SugarParameter("@operator", _userKey.Name));
            int retDB = await _eventService.BidDataPush(dto);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }
    }
}
