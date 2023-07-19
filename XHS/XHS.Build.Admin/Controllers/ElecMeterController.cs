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
using XHS.Build.Services.ElecMeter;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class ElecMeterController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IElecMeterService _elecMeterService;
        public ElecMeterController(IUser user, IElecMeterService specialEqpService)
        {
            _elecMeterService = specialEqpService;
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
            var data = await _elecMeterService.GetList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _elecMeterService.GetGroupElecMeterCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCElecMeterEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var emeters = await _elecMeterService.Query(x => x.emetercode == input.emetercode);
            if (emeters.Count > 0)
            {
                return ResponseOutput.NotOk("电表设备编号已存在");
            }
            input.@operator = _user.Name;
            var rows = await _elecMeterService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="EMID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int EMID)
        {
            var emeters = await _elecMeterService.Query(x => x.EMID == EMID);
            if (emeters.Count > 0)
            {
                var emeter = emeters.First();
                emeter.bdel = 1;
                emeter.operatedate = DateTime.Now;
                emeter.@operator = _user.Name;
                return ResponseOutput.Result(await _elecMeterService.Update(emeter, new List<string> { "bdel" , "operatedate", "operator" }, null, string.Format(" EMID='{0}' ", EMID)));
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
        public async Task<IResponseOutput> Put(GCElecMeterEntity input)
        {
            if (input == null || input.EMID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var emeters = await _elecMeterService.Query(x => x.emetercode == input.emetercode && x.EMID != input.EMID);
            if (emeters.Count > 0)
            {
                return ResponseOutput.NotOk("电表设备编号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _elecMeterService.Update(input);
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
            return ResponseOutput.Ok(await _elecMeterService.QueryById(id));
        }
    }
}
