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
using XHS.Build.Services.Truck;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class TruckController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ITruckService _truckService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="TruckService"></param>
        public TruckController(IUser user, ITruckService TruckService)
        {
            _truckService = TruckService;
            _user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> List(string keyword = "", int page = 1, int size = 10)
        {
            var data = await _truckService.GetList(keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        ///// <summary>
        ///// 分组
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IResponseOutput> Groups()
        //{
        //    return ResponseOutput.Ok(await _truckService.GetGroupTruckCount());
        //}


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSiteTruckEntity input)
        {
            if (input == null || input.SITEID <= 0 || input.truckno == null || input.truckno == string.Empty)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var trucks = await _truckService.Query(x => x.truckno == input.truckno && x.SITEID == input.SITEID);
            if (trucks.Count > 0)
            {
                return ResponseOutput.NotOk("该车牌号已存在");
            }
            input.@operator = _user.Name;
            var rows = await _truckService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="STID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int STID)
        {
            var result = await _truckService.DeleteById(STID);
            if (result)
            {
                return ResponseOutput.Ok(1);
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
        public async Task<IResponseOutput> Put(GCSiteTruckEntity input)
        {
            if (input == null|| input.STID <=0 || input.SITEID <= 0 || input.truckno == null || input.truckno == string.Empty)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var truck = await _truckService.Query(x => x.truckno == input.truckno && x.SITEID == input.SITEID && x.STID != input.STID);
            if (truck.Count > 0)
            {
                return ResponseOutput.NotOk("该车牌号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _truckService.Update(input);
            if (suc)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="STID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(int STID)
        {
            return ResponseOutput.Ok(await _truckService.QueryById(STID));
        }
    }
}
