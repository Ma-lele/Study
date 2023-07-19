using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Site;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SiteMapEquipController : ControllerBase
    {
        private readonly ISiteMapEquipService _siteMapEquipService;
        private readonly IUser _user;
        /// <summary>
        /// 设备地图点位
        /// </summary>
        /// <param name="siteMapEquipService"></param>
        public SiteMapEquipController(ISiteMapEquipService siteMapEquipService, IUser user)
        {
            _siteMapEquipService = siteMapEquipService;
            _user = user;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSiteMapEquipEntity entity)
        {
            if (entity == null)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;
            return ResponseOutput.Ok(await _siteMapEquipService.Add(entity));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCSiteMapEquipEntity entity)
        {
            if (entity == null || entity.SMEID <= 0)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;
            return ResponseOutput.Ok(await _siteMapEquipService.Update(entity));
        }

        /// <summary>
        /// 详细
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择需要查看的数据");
            }
            var Entitys = await _siteMapEquipService.Query(a => a.SITEID == SITEID);
            if (Entitys.Any())
            {
                return ResponseOutput.Ok(Entitys[0]);
            }
            return ResponseOutput.Ok();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SMEID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int SMEID)
        {
            if (SMEID <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }
            return (await _siteMapEquipService.DeleteById(SMEID)) ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 获取监测点下设备地图点位
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListBySite(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择需要查看的数据");
            }
            var Entitys = await _siteMapEquipService.getListBySite(SITEID);
            
            return ResponseOutput.Ok(Entitys);
        }
    }
}
