using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Attend;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 考勤
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendController : ControllerBase
    {
        private readonly IAttendService _attendService;
        private readonly IHpAttend _hpAttend;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendService"></param>
        public AttendController(IAttendService attendService, IHpAttend hpAttend)
        {
            _attendService = attendService;
            _hpAttend = hpAttend;
        }

        /// <summary>
        /// 插入考勤记录带照片
        /// </summary>
        /// <param name="SITEID">SITEID</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">维度</param>
        /// <param name="address">地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="fileString">文件</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("attendWithPhoto")]
        public async Task<IResponseOutput> AttendWithPhoto([FromForm]int SITEID, [FromForm] float longitude, [FromForm] float latitude, [FromForm] string address, [FromForm] string filename, [FromForm] string fileString, [FromForm] string remark)
        {
            if (SITEID < 1 || longitude.Equals(0) || latitude.Equals(0) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(filename))
                return ResponseOutput.NotOk("请输入正确的考勤信息");

            long result = 0;
            try
            {
                result =await _hpAttend.doUpload(SITEID, longitude, latitude, address, filename, fileString, remark);
                return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk();
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("插入考勤信息失败");
            }
        }

        /// <summary>
        /// 插入考勤记录
        /// </summary>
        /// <param name="siteid">考勤种类 0：签到，1：上班，2：下班</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">维度</param>
        /// <param name="address">地址</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("attend")]
        public async Task<IResponseOutput> Attend([FromForm] int siteid, [FromForm] float longitude, [FromForm] float latitude, [FromForm] string address, [FromForm] string remark)
        {
            return ResponseOutput.Ok(await _attendService.DoInsert(siteid,longitude,latitude,address,remark));
        }

        /// <summary>
        /// 获取考勤数据
        /// </summary>
        /// <param name="ATTENDID">考勤ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getlistbyid")]
        public async Task<IResponseOutput> GetListById(ulong ATTENDID)
        {
            List<BnAttend> result = new List<BnAttend>();
            DataTable dt = await _attendService.GetListById(ATTENDID);
            if (dt == null || dt.Rows.Count.Equals(0))
                return ResponseOutput.Ok(result);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BnAttend ba = new BnAttend();
                ba.ATTENDID = Convert.ToUInt64(dt.Rows[i]["ATTENDID"]);
                ba.USERID = Convert.ToInt32(dt.Rows[i]["USERID"]);
                ba.attenddate = Convert.ToDateTime(dt.Rows[i]["attenddate"]);
                ba.address = Convert.ToString(dt.Rows[i]["address"]);

                result.Add(ba);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 插入考勤便签
        /// </summary>
        /// <param name="attenddate">考勤日期</param>
        /// <param name="operatenote">便签内容</param>
        /// <returns></returns>
        [HttpGet]
        [Route("note")]
        public async Task<IResponseOutput> Note(DateTime attenddate, string operatenote)
        {
            return ResponseOutput.Ok(await _attendService.DoNoteRegist(attenddate,operatenote));
        }

        /// <summary>
        /// 获取考勤
        /// </summary>
        /// <param name="operatedate">操作日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Getnotelist")]
        public async Task<IResponseOutput> GetNoteList(DateTime operatedate)
        {
            List<BnAttendNote> result = new List<BnAttendNote>();
            DataTable dt = await _attendService .GetNoteList(operatedate);
            if (dt == null || dt.Rows.Count.Equals(0))
                return ResponseOutput.Ok(result);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BnAttendNote ban = new BnAttendNote();
                ban.ATTENDNOTEID = Convert.ToInt32(dt.Rows[i]["ATTENDNOTEID"]);
                ban.USERID = Convert.ToInt32(dt.Rows[i]["USERID"]);
                ban.attenddate = Convert.ToDateTime(dt.Rows[i]["attenddate"]);
                ban.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);
                ban.operatenote = Convert.ToString(dt.Rows[i]["operatenote"]);

                result.Add(ban);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取服务器时间和是否签到过
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("attendtime")]
        public async Task<IResponseOutput> GetAttendTime()
        {
            DataTable dt = await _attendService.spUserAttendListForUser(DateTime.Now);
            if(dt!=null && dt.Rows!=null && dt.Rows.Count > 0)
            {
                return ResponseOutput.Ok(new { ServiceTime=DateTime.Now, Attented=true });
            }
            else
            {
                return ResponseOutput.Ok(new { ServiceTime = DateTime.Now, Attented = false });
            }
        }

        /// <summary>
        /// 获取打卡记录
        /// </summary>
        /// <param name="billdate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userattends")]
        public async Task<IResponseOutput> UserAttends(DateTime billdate)
        {
            return ResponseOutput.Ok(await _attendService.spUserAttendListForUser(billdate));
        }
    }
}