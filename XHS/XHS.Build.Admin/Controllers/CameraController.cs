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
using XHS.Build.Services.Cameras;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 摄像头
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class CameraController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ICameraService _camerasService;
        /// <summary>
        /// 摄像头
        /// </summary>
        /// <param name="cameraService"></param>
        /// <param name="user"></param>
        public CameraController(ICameraService cameraService, IUser user)
        {
            _camerasService = cameraService;
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
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _camerasService.GetSiteCameraPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _camerasService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCCameraEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var Dbs= await _camerasService.Query(a => a.cameracode == input.cameracode && a.channel == input.channel && a.cameratype == input.cameratype);
            if (Dbs.Any())
            {
                return ResponseOutput.NotOk("已存在设备编号");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var rows = await _camerasService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _camerasService.DeleteById(id);
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("删除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCCameraEntity input)
        {
            if (input == null || input.CAMERAID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var Dbs = await _camerasService.Query(a => a.cameracode == input.cameracode && a.channel == input.channel && a.cameratype == input.cameratype && a.CAMERAID != input.CAMERAID);
            if (Dbs.Any())
            {
                return ResponseOutput.NotOk("已存在设备编号");
            }

            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _camerasService.Update(input);
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
            return ResponseOutput.Ok(await _camerasService.QueryById(id));
        }

        /// <summary>
        /// 冻结/解冻
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> UpdateStatus(GCCameraEntity input)
        {
            input.@operator = _user.Name;
            input.operatedate = DateTime.Now;
            //string strwhere = "";
            //if (input.bdel == 1)//冻结
            //{
            //    strwhere = string.Format(" CAMERAID={0} and bdel=0", input.CAMERAID.ToString());
            //}
            //else//解冻
            //{
            //    strwhere = string.Format(" CAMERAID={0} and bdel=1", input.CAMERAID.ToString());
            //}
            var suc = await _camerasService.Update(input,
                new List<string>() { "CAMERAID", "operator", "operatedate", "bdel" },
                null,
                string.Format(" CAMERAID={0}", input.CAMERAID.ToString()));
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("状态更新失败");
            }
        }
    }
}
