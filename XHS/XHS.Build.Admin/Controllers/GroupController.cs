using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Dictionary;
using XHS.Build.Services.Group;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGCRegionService _gCRegionService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        public GroupController(IGroupService groupService, IGCRegionService gCRegionService, IUser user, IMapper mapper)
        {
            _groupService = groupService;
            _gCRegionService = gCRegionService;
            _user = user;
            _mapper = mapper;
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<GcGroupEntity, bool>> whereExpression = a => true;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.groupname.Contains(keyword) || b.groupshortname.Contains(keyword));
            }
            if (_user.GroupId > 0)
            {
                whereExpression = whereExpression.And(b => b.GROUPID == _user.GroupId);
            }
            var dbList = await _groupService.QueryPage(whereExpression, page, size, " status, groupid desc");
            if (dbList.data != null)
            {
                var cityList = await _gCRegionService.Query();
                dbList.data.ToList().ForEach(item =>
                {
                    item.CityName = item.city <= 0 ? "" : cityList.FirstOrDefault(c => c.RegionId == item.city).RegionName;
                    item.DistrictName = item.district <= 0 ? "" : cityList.FirstOrDefault(c => c.RegionId == item.district).RegionName;
                });
            }

            var output = _mapper.Map<List<GroupInputDto>>(dbList.data);
            output.ForEach(ii =>
            {
                ii.longitude = string.IsNullOrWhiteSpace(ii.lnglat) ? "" : ii.lnglat.Split('|')[0];
                ii.latitude = string.IsNullOrWhiteSpace(ii.lnglat) ? "" : ii.lnglat.Split('|')[1];
            });

            var data = new PageOutput<GroupInputDto>()
            {
                data = output,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        [Login]
        [HttpGet]
        public async Task<IResponseOutput> GetAll()
        {
            var result = await _groupService.GetAll(_user.GroupId);
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GroupInputDto input)
        {
            if (input == null || string.IsNullOrEmpty(input.groupname))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var group = _mapper.Map<GcGroupEntity>(input);
            group.lnglat = $"{input.longitude}|{input.latitude}"; 

            var rows = await _groupService.Add(group);

            return rows > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(string groupid)
        {
            if (string.IsNullOrEmpty(groupid))
            {
                return ResponseOutput.NotOk("请选择需要删除的信息");
            }
            var entity = await _groupService.QueryById(groupid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到可删除的信息");
            }
            var suc = await _groupService.DeleteById(groupid);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GroupInputDto input)
        {
            if (input == null || input.GROUPID <= 0 || string.IsNullOrEmpty(input.groupname))
            {
                return ResponseOutput.NotOk("请填写需要修改的信息");
            }
            input.operatedate = DateTime.Now;

            var entity = _mapper.Map<GcGroupEntity>(input);
            entity.lnglat = $"{input.longitude}|{input.latitude}"; 
            var suc = await _groupService.Update(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新信息失败");
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Status(string groupid)
        {
            if (string.IsNullOrEmpty(groupid))
            {
                return ResponseOutput.NotOk("请选择需要修改的信息");
            }
            var entity = await _groupService.QueryById(groupid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到需要修改的信息");
            }
            entity.status = entity.status == 0 ? 1 : 0;
            var suc = await _groupService.Update(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新信息失败");
        }

        /// <summary>
        /// 检测对象分组及数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GroupSiteCount()
        {
            return ResponseOutput.Ok(await _groupService.GetGroupSiteCountAll());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GroupSiteCountOutZero()
        {
            return ResponseOutput.Ok(await _groupService.GetGroupSiteCountOutZero());
        }


        /// <summary>
        /// 设置扬尘告警线
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> SetLine(JObject input)
        {
            string pm10warnline = input["pm10warnline"] == null ? "" : input["pm10warnline"].ToString();
            string pm2_5warnline = input["pm2_5warnline"] == null ? "" : input["pm2_5warnline"].ToString();
            string tspwarnline = input["tspwarnline"] == null ? "" : input["tspwarnline"].ToString();
            int groupid = input["GroupId"] == null ? 0 : int.Parse(input["GroupId"].ToString());
            if (groupid <= 0)
            {
                return ResponseOutput.NotOk("设置失败，请至少选择一行");
            }

            List<GCGroupSettingEntity> insertdata = new List<GCGroupSettingEntity>();
            List<GCGroupSettingEntity> updata = new List<GCGroupSettingEntity>();
            List<GCGroupSettingEntity> temp = new List<GCGroupSettingEntity> {
                new GCGroupSettingEntity
                {
                    GROUPID = groupid,
                    key = "pm10warnline",
                    value = pm10warnline
                },
                 new GCGroupSettingEntity
                {
                    GROUPID = groupid,
                    key = "pm2_5warnline",
                    value = pm2_5warnline
                },
                new GCGroupSettingEntity
                {
                    GROUPID = groupid,
                    key = "tspwarnline",
                    value = tspwarnline
                }
            };

            var exists = await _groupService.GetGroupSetting(groupid);
            foreach (var i in temp)
            {
                if (exists.Find(ii => ii.key == i.key) != null)
                {
                    updata.Add(i);
                }
                else
                {
                    insertdata.Add(i);
                }
            }

            var result = await _groupService.SetLine(updata, insertdata);

            return result ? ResponseOutput.Ok("设置成功") : ResponseOutput.NotOk("设置失败");
        }


        /// <summary>
        /// 获取组设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSetting(int groupid)
        {
            string[] list = { "pm10warnline", "pm2_5warnline", "tspwarnline" };

            var result = await _groupService.GetGroupSetting(groupid);
            result = result.Where(ii => list.Contains(ii.key)).ToList();
            return ResponseOutput.Ok(result);
        }
    }
}
