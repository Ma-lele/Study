using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Exam;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common;
using System;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 培训考试
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly IUser _user;
        private readonly IHpSystemSetting _hpSystemSetting;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="examService"></param>
        /// <param name="user"></param>
        public ExamController(IExamService examService, IUser user, IHpSystemSetting hpSystemSetting)
        {
            _hpSystemSetting = hpSystemSetting;
            _examService = examService;
            _user = user;
        }

        /// <summary>
        /// 获取考题
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getRandomQuestionList")]
        public async Task<IResponseOutput> GetRandomQuestionList()
        {
            var s151 = _hpSystemSetting.getSettingValue(Const.Setting.S151);
            int pageSize = 20;
            if (!string.IsNullOrEmpty(s151))
            {
                pageSize = Int32.Parse(s151);
            }
            return ResponseOutput.Ok(await _examService.GetRandomQuestionList(1, pageSize));
        }


        /// <summary>
        /// 获取考题
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <param name="examtype">考试类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getRandomQuestionListByTag")]
        public async Task<IResponseOutput> GetRandomQuestionListByTag(int SITEID, string examtype)
        {
            var s151 = _hpSystemSetting.getSettingValue(Const.Setting.S151);
            int pageSize = 20;
            if (!string.IsNullOrEmpty(s151))
            {
                pageSize = Int32.Parse(s151);
            }
            return ResponseOutput.Ok(await _examService.GetRandomQuestionListByTag(SITEID, examtype,1, pageSize));
        }

        /// <summary>
        /// 获取用户考试记录
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpGet]
        [Route("getExamResultList")]
        public async Task<IResponseOutput> GetExamResultListByUser()
        {
            return ResponseOutput.Ok(await _examService.GetExamResultListByUser());
        }

        /// <summary>
        /// 获取考试类别
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpGet]
        [Route("getExamTag")]
        public async Task<IResponseOutput> getExamTag()
        {
            return ResponseOutput.Ok(await _examService.GetTagList());
        }

        /// <summary>
        /// 提交考试结果
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="examtype">考试类型</param>
        /// <param name="usetime">考试用时（秒）</param>
        /// <param name="score">得分</param>
        /// <param name="totalcount">总题数</param>
        /// <param name="wrongcount">答错题数</param>
        /// <param name="rightcount">答对题数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("doExamResultSave")]
        public async Task<IResponseOutput> DoExamResultSave(int SITEID, string examtype, int usetime, float score, int totalcount, int wrongcount, int rightcount)
        {
            return ResponseOutput.Ok(await _examService.DoExamResultSave(SITEID,examtype, usetime, score, totalcount, wrongcount, rightcount));
        }

    }
}