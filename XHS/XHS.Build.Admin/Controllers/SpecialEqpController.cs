using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Device;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class SpecialEqpController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ISpecialEqpService _specialEqpService;
        private readonly ISpecialEqpRecordService _specialEqpRecordService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IDeviceService _deviceService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="specialEqpService"></param>
        /// <param name="hpSystemSetting"></param>
        public SpecialEqpController(IUser user, ISpecialEqpService specialEqpService, ISpecialEqpRecordService specialEqpRecordService, 
            IHpSystemSetting hpSystemSetting, IDeviceService deviceService)
        {
            _specialEqpService = specialEqpService;
            _specialEqpRecordService = specialEqpRecordService;
            _user = user;
            _hpSystemSetting = hpSystemSetting;
            _deviceService = deviceService;
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
            var data = await _specialEqpService.GetSiteSpecialEqpPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _specialEqpService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSpecialEqpEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _specialEqpService.Query(f => f.secode == input.secode && f.bdel == 0);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var rows = await _specialEqpService.Add(input);

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

            var entity = await _specialEqpService.QueryById(id);
            if (entity == null || entity.SEID <= 0)
            {
                return ResponseOutput.NotOk("数据不存在");
            }

            bool isDel = await _specialEqpService.DeleteById(id);
            if (isDel)
            {
                var delHistory = new GCDevDelHis {
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    DEVID = entity.SEID,
                    devcode = entity.secode,
                    devtype = entity.setype == 1 ? 2 : 6,
                    devtypename = entity.setype == 1 ? "塔吊监控" : "施工升降机",
                    @operator = _user.Name,
                    operatedate = DateTime.Now
                };
                await _deviceService.AddDevDelHis(delHistory);
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
        public async Task<IResponseOutput> Put(GCSpecialEqpEntity input)
        {
            if (input == null || input.SEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _specialEqpService.Query(f => f.secode == input.secode && f.bdel == 0 && f.SEID != input.SEID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }

            var recordExist = await _specialEqpRecordService.Query(f => 
            f.SEID == input.SEID && 
            f.SITEID != input.SITEID && 
            f.installdate <= DateTime.Now && 
            (f.uninstalldate > DateTime.Now || f.uninstalldate < DateTime.Parse("1950-01-01")));
            if (recordExist.Any())
            {
                return ResponseOutput.NotOk("该设备已绑定备案信息，不能修改监测对象");
            }

            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _specialEqpService.Update(input);
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
            return ResponseOutput.Ok(await _specialEqpService.QueryById(id));
        }

        /// <summary>
        /// 获取大运Token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GetWXDYToken(string appid = "sysfdas2fvdasf33dag", bool isForce = false,string SECRTE = "cag4adg412fa2dc2", string AES = "da2gaf4afdasfea1", string url = "")
        {
            string result = await _specialEqpService.GetWXDYToken(appid, isForce, SECRTE, AES, url);
            if (string.IsNullOrEmpty(result))
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 新增、删除、修改图片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetSEPic(SpecialEqpDocInputDto input)
        {
            string domain = _hpSystemSetting.getSettingValue(Const.Setting.S030);
            string folder = _hpSystemSetting.getSettingValue(Const.Setting.S056);

            string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string savePath = Path.Combine(domain, folder, input.SEID.ToString());
            UFile.CreateDirectory(savePath);

            List<GCSpecialEqpDoc> addList = new List<GCSpecialEqpDoc>();
            #region 文件填入实体
            foreach (var i in input.Notify.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSpecialEqpDoc doc = new GCSpecialEqpDoc
                {
                    SEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SEID = input.SEID,
                    filetype = 1,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            foreach (var i in input.Monitor.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSpecialEqpDoc doc = new GCSpecialEqpDoc
                {
                    SEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SEID = input.SEID,
                    filetype = 2,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            foreach (var i in input.Licenses.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSpecialEqpDoc doc = new GCSpecialEqpDoc
                {
                    SEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SEID = input.SEID,
                    filetype = 3,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            foreach (var i in input.Repair.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSpecialEqpDoc doc = new GCSpecialEqpDoc
                {
                    SEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SEID = input.SEID,
                    filetype = 4,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            foreach (var i in input.Dismantle.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSpecialEqpDoc doc = new GCSpecialEqpDoc
                {
                    SEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SEID = input.SEID,
                    filetype = 5,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            #endregion

            if (addList.Count > 0)
            {
                var addResult = await _specialEqpService.AddSEDoc(addList);
                if (addResult <= 0)
                {
                    return ResponseOutput.NotOk("新增失败");
                }
            }

            var delArray = input.NotifyDel.Concat(input.MonitorDel)
                                    .Concat(input.LicensesDel)
                                    .Concat(input.RepairDel)
                                    .Concat(input.DismantleDel)
                                    .ToArray();

            if (delArray.Length > 0)
            {
                for (int i = 0; i < delArray.Length; i++)
                {
                    delArray[i] = delArray[i].Split('/').LastOrDefault().Split('.').FirstOrDefault();
                }
                var delResult = await _specialEqpService.DeleteSEDoc(input.SEID, delArray);
                if (delResult <= 0)
                {
                    return ResponseOutput.NotOk("删除失败");
                }
            }

            return ResponseOutput.Ok("设置成功");
        }


        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="SEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSEPics(int SEID)
        {
            var pics = await _specialEqpService.GetSEPics(SEID);
            pics.ForEach(ii =>
            {
                ii.url = $"{ii.url.Remove(ii.url.LastIndexOf("/"))}.{ii.url.Split('.').LastOrDefault()}";
            });
            return ResponseOutput.Ok(pics);
        }
    }
}
