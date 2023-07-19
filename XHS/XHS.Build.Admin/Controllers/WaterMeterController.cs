using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.WaterMeter;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class WaterMeterController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IWaterMeterService _waterMeterService;
        public WaterMeterController(IUser user, IWaterMeterService specialEqpService)
        {
            _waterMeterService = specialEqpService;
            _user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> List(int groupid = 0, string keyword = "", int page = 1, int size = 10)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _waterMeterService.GetList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _waterMeterService.GetGroupWaterMeterCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCWaterMeterEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var wmeters = await _waterMeterService.Query(x => x.wmetercode == input.wmetercode);
            if (wmeters.Count > 0)
            {
                return ResponseOutput.NotOk("水表设备编号已存在");
            }
            input.@operator = _user.Name;
            var rows = await _waterMeterService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="WMID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int WMID)
        {
            var wmeters = await _waterMeterService.Query(x => x.WMID == WMID);
            if (wmeters.Count > 0)
            {
                var wmeter = wmeters.First();
                wmeter.bdel = 1;
                wmeter.operatedate = DateTime.Now;
                wmeter.@operator = _user.Name;
                return ResponseOutput.Result(await _waterMeterService.Update(wmeter, new List<string> { "bdel" , "operatedate", "operator" }, null, string.Format(" WMID='{0}' ", WMID)));
            }
            else
                return ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCWaterMeterEntity input)
        {
            if (input == null || input.WMID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var wmeters = await _waterMeterService.Query(x => x.wmetercode == input.wmetercode && x.WMID != input.WMID);
            if (wmeters.Count > 0)
            {
                return ResponseOutput.NotOk("水表设备编号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _waterMeterService.Update(input);
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _waterMeterService.QueryById(id));
        }
    }
}
