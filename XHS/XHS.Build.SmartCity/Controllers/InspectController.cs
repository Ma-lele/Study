using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Inspection;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 安全排查
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InspectController : ControllerBase
    {
        private readonly AqtToken _aqtToken;
        private readonly IInspectionService _inspectionService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iInspect"></param>
        public InspectController(IInspectionService inspectionService, AqtToken aqtToken)
        {
            _inspectionService = inspectionService;
            _aqtToken = aqtToken;
        }

        /// <summary>
        /// 安全排查 总数统计
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datamonth">年月</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCount(int GROUPID=0, string datamonth="")
        {
            var data = await _inspectionService.GetCountAsync(GROUPID, datamonth);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全排查 月评数据分析
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datayear">年份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetMonthReview(int GROUPID=0,int datayear=0)
        {
            var data = await _inspectionService.GetMonthReviewAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全排查 企业检查情况
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="yearmonth">年月</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetRoundCount(int GROUPID=0,string yearmonth="")
        {
            var data = await _inspectionService.GetRoundCountAsync(GROUPID, yearmonth);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全排查 安标考评结果分析
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datayear">年份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSafetyStandard(int GROUPID = 0, int datayear = 0)
        {
            var data = await _inspectionService.GetSafetyStandardAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全排查 每月自查数据分析
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datayear">年份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSelfInspect(int GROUPID = 0,int datayear=0)
        {
            var data = await _inspectionService.GetSelfInspectAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取项目自查基本信息
        /// </summary>
        /// <param name="pageIndex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pageSize">每页抓取数据条数</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetProjectSelfInspection(int pageIndex, int pageSize, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            string api = "ProjectInformation/GetProjectSelfInspection";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("pageIndex", pageIndex);
            keyValues.Add("pageSize", pageSize);
            keyValues.Add("belongedTo", belongedTo);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return ResponseOutput.Ok(JObject.Parse(result)) ;
            }
            return ResponseOutput.Ok(mJObj);
        }
    }
}
