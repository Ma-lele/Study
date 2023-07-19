using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Site;
using XHS.Build.Services.RealName;
using Newtonsoft.Json.Linq;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common;
using XHS.Build.Services.Group;
using Util;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IUser _user;
        private readonly IQunYaoRealNameService _qunYaoRealNameService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IGroupService _groupService;
        private readonly XHSRealnameToken _jwtToken;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteService"></param>
        /// <param name="user"></param>
        /// <param name="qunYaoRealNameService"></param>
        /// <param name="hpSystemSetting"></param>
        /// <param name="groupService"></param>
        /// <param name="jwtToken"></param>
        public SiteController(ISiteService siteService, IUser user, IQunYaoRealNameService qunYaoRealNameService, IHpSystemSetting hpSystemSetting, IGroupService groupService, XHSRealnameToken jwtToken)
        {
            _siteService = siteService;
            _user = user;
            _qunYaoRealNameService = qunYaoRealNameService;
            _hpSystemSetting = hpSystemSetting;
            _groupService = groupService;
            _jwtToken = jwtToken;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="type"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid, int type, string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<GCSiteEntity, bool>> whereExpression = a => a.status != 3;
            if (_user.GroupId > 0)
            {
                whereExpression = whereExpression.And(a => a.GROUPID == _user.GroupId);
            }
            else
            {
                if (groupid > 0)
                {
                    whereExpression = whereExpression.And(a => a.GROUPID == groupid);
                }
            }

            if (type == 1)
            {
                whereExpression = whereExpression.And(a => a.PARENTSITEID == a.SITEID);
            }
            else if (type == 2)
            {
                whereExpression = whereExpression.And(a => a.PARENTSITEID != a.SITEID);
            }
            else
            {
                return ResponseOutput.NotOk("查询类型错误");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.sitename.Contains(keyword) || b.siteshortname.Contains(keyword));
            }
            var dbList = await _siteService.QueryPage(whereExpression, page, size, " status,GROUPID,startdate desc");
            var data = new PageOutput<GCSiteEntity>()
            {
                data = dbList.data,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSiteEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.sitename) || string.IsNullOrEmpty(entity.siteshortname) || string.IsNullOrEmpty(entity.siteaddr) || entity.GROUPID <= 0 || string.IsNullOrEmpty(entity.sitecity) || entity.sitetype <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            entity.operatedate = DateTime.Now;
            entity.@operator = _user.Name;
            var rows = await _siteService.Insert(entity);

            entity.SITEID = rows;
            entity.PARENTSITEID = rows;
            await _siteService.Update(entity, new List<string>() { "PARENTSITEID" });
            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Stop(int siteid)
        {
            if (siteid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要停用的数据");
            }
            var entity = await _siteService.QueryById(siteid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到相关数据");
            }
            entity.status = 1;
            entity.operatedate = DateTime.Now;
            entity.@operator = _user.Name;
            bool isDel = await _siteService.Update(entity, new List<string>() { "status", "operatedate", "operator" });
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("停用失败");
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Recover(int siteid)
        {
            if (siteid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要恢复的数据");
            }
            var entity = await _siteService.QueryById(siteid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到相关数据");
            }
            entity.status = 0;
            entity.operatedate = DateTime.Now;
            entity.@operator = _user.Name;
            bool isDel = await _siteService.Update(entity, new List<string>() { "status", "operatedate", "operator" });
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("恢复失败");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Complete(int siteid)
        {
            if (siteid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要关闭的数据");
            }
            var entity = await _siteService.QueryById(siteid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到相关数据");
            }

            int result = await _siteService.doSiteComplete(siteid, _user.Name);
            if (result > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("关闭失败");
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Remove(int siteid)
        {
            if (siteid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要移除的数据");
            }
            var entity = await _siteService.QueryById(siteid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到相关数据");
            }

            int result = await _siteService.doSiteRemove(siteid, _user.Name);
            if (result > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("移除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCSiteEntity entity)
        {
            if (entity == null || entity.SITEID <= 0 || string.IsNullOrEmpty(entity.sitename) || string.IsNullOrEmpty(entity.siteshortname) || string.IsNullOrEmpty(entity.siteaddr) || entity.GROUPID <= 0 || string.IsNullOrEmpty(entity.sitecity) || entity.sitetype <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            entity.operatedate = DateTime.Now;
            bool suc = await _siteService.Update(entity);
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
        /// 获取全部站点
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Login]
        public async Task<IResponseOutput> Sites(int groupid = 0, int type = 0)
        {
            Expression<Func<GCSiteEntity, bool>> whereExpression = a => a.status != 3;
            if (groupid > 0)
            {
                whereExpression = whereExpression.And(a => a.GROUPID == groupid);
            }
            if (type == 1)
            {
                whereExpression = whereExpression.And(a => a.PARENTSITEID == a.SITEID);
            }
            else if (type == 2)
            {
                whereExpression = whereExpression.And(a => a.PARENTSITEID != a.SITEID);
            }

            var dbList = await _siteService.Query(whereExpression, " status,GROUPID,startdate desc");
            return ResponseOutput.Ok(dbList);
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _siteService.QueryById(id));
        }

        /// <summary>
        /// 获取简介
        /// </summary>
        /// <param name="groupid">分组编号</param>
        /// <param name="siteid">监测对象编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSummary(int groupid, int siteid)
        {

            var summary = await _siteService.GetSummary(groupid, siteid);
            return ResponseOutput.Ok(summary);
        }

        /// <summary>
        /// 保存简介
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果</returns>
        [HttpPut]
        public async Task<IResponseOutput> SaveSummary(GCSiteSummaryEntity entity)
        {
            int result = await _siteService.SaveSummary(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }


        /// <summary>
        /// 新增、删除、修改图片
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetPic(SitePicInputDto input)
        {
            string domain = _hpSystemSetting.getSettingValue(Const.Setting.S030);
            string folder = _hpSystemSetting.getSettingValue(Const.Setting.S058);

            string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string savePath = Path.Combine(domain, folder, input.SITEID.ToString());
            UFile.CreateDirectory(savePath);

            List<GCSiteDoc> addList = new List<GCSiteDoc>();
            foreach (var i in input.rendering.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSiteDoc doc = new GCSiteDoc
                {
                    SITEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SITEID = input.SITEID,
                    filetype = 1,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }
            foreach (var i in input.dust.Where(ii => ii.status == "ready"))
            {
                string tmpFileName = i.filename;
                if (UFile.IsExistFile(tmpPath + "/" + tmpFileName))
                {
                    UFile.Move(tmpPath + "/" + tmpFileName, savePath + "/" + tmpFileName);
                }
                GCSiteDoc doc = new GCSiteDoc
                {
                    SITEDOCID = Guid.Parse(i.filename.Split('.').FirstOrDefault()),
                    SITEID = input.SITEID,
                    filetype = 2,
                    filename = i.name,
                    filesize = UFile.GetFileSize(savePath + "/" + tmpFileName),
                    updater = _user.Name,
                    updatedate = DateTime.Now
                };
                addList.Add(doc);
            }

            if (addList.Count > 0)
            {
                var addResult = await _siteService.AddSiteDoc(addList);
                if (addResult <= 0)
                {
                    return ResponseOutput.NotOk("新增失败");
                }
            }



            var delArray = input.dustDel.Concat(input.renderingDel).ToArray();
            if (delArray.Length > 0)
            {
                for (int i = 0; i < delArray.Length; i++)
                {
                    delArray[i] = delArray[i].Split('/').LastOrDefault().Split('.').FirstOrDefault();
                }
                var delResult = await _siteService.DeleteSiteDoc(input.SITEID, delArray);
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
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPic(int siteid)
        {
            var pics = await _siteService.GetPic(siteid);
            pics.ForEach(ii =>
            {
                ii.url = $"{ii.url.Remove(ii.url.LastIndexOf("/"))}.{ii.url.Split('.').LastOrDefault()}";
            });
            return ResponseOutput.Ok(pics);
        }

        /// <summary>
        /// 获取各种实名制对应的项目列表
        /// </summary>
        /// <param name="type">1：群耀，2：大运，5：新合盛实名制</param>
        /// <param name="GROUPID">分组编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAttendProjList(int type, int GROUPID)
        {
            var retString = "";
            switch (type)
            {
                case 1:
                    //1：群耀
                    retString = await _qunYaoRealNameService.GetProjList();
                    if (string.IsNullOrEmpty(retString))
                    {
                        return ResponseOutput.NotOk("返回内容为空");
                    }
                    JObject retObj = new JObject();
                    retObj = JObject.Parse(retString);
                    if (retObj["ResultState"].ToString() == "1")
                    {
                        return ResponseOutput.Ok(retObj["data"]);
                    }
                    else
                    {
                        return ResponseOutput.NotOk("获取失败");
                    }

                case 2:
                    //2：大运
                    var s118 = _hpSystemSetting.getSettingValue(Const.Setting.S118);
                    if (string.IsNullOrEmpty(s118))
                    {
                        return ResponseOutput.NotOk("未获取到请求地址信息");
                    }
                    retString = HttpNetRequest.HttpGet(s118 + "queryAllCSites");
                    if (string.IsNullOrEmpty(retString))
                    {
                        return ResponseOutput.NotOk("返回内容为空");
                    }
                    JObject mJObj = new JObject();
                    mJObj = JObject.Parse(retString);
                    if (mJObj["resultMessage"].ToString() == "success")//成功
                    {
                        return ResponseOutput.Ok(mJObj["data"]);
                    }
                    else
                    {
                        return ResponseOutput.NotOk((string)mJObj.GetValue("resultMessage"));
                    }
                case 4:
                    //4：都驰
                    return ResponseOutput.Ok("");
                case 5:
                    //5：新合盛实名制
                    string projapi = "construction-site-api/projects-by-keyword";
                    string url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
                    if (string.IsNullOrEmpty(url))
                    {
                        return ResponseOutput.NotOk("实名制接口地址未配置。");
                    }
                    string keysecret = await _groupService.GetAttendUserPsd(GROUPID);
                    if (string.IsNullOrEmpty(keysecret))
                    {
                        return ResponseOutput.NotOk("实名制接口SECRET未配置。");
                    }
                    var user = keysecret.Split("||")[0];
                    var pwd = keysecret.Split("||")[1];
                    var result = _jwtToken.JsonRequest(url, user, pwd, projapi, "{}");
                    return ResponseOutput.Ok(JObject.Parse(result).GetValue("data"));

                default:
                    return ResponseOutput.NotOk("该类型实名制还未对接，敬请期待。");
            }
        }

        /// <summary>
        /// 第三方绑定信息保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> SaveBind(GCSiteBindEntity entity)
        {
            entity.apiupdatedate = DateTime.Now;
            int result = await _siteService.doSaveBind(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }

        /// <summary>
        /// 根据分组获取市区街道
        /// </summary>
        /// <param name="GROUPID">分组编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAreas(int GROUPID)
        {
            var areas = await _groupService.GetAreas(GROUPID);
            return ResponseOutput.Ok(areas);
        }

        /// <summary>
        /// 获取工地和五方信息
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SiteCompany(int SITEID)
        {
            return ResponseOutput.Ok(await _siteService.getSiteCompany(SITEID));
        }

        /// <summary>
        /// 修改监测对象及五方信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> SiteCompanyUpdate(GCSiteAndCompanyEntity entity)
        {
            if (entity == null || entity.SITEID <= 0 || string.IsNullOrEmpty(entity.sitename)
                || string.IsNullOrEmpty(entity.siteshortname) || string.IsNullOrEmpty(entity.siteaddr)
                || entity.GROUPID <= 0 || string.IsNullOrEmpty(entity.sitecity) || entity.sitetype <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            entity.@operator = _user.Name;
            entity.operatedate = DateTime.Now;

            foreach (var company in entity.sitecompany)
            {
                company.SITEID = entity.SITEID;
                company.GROUPID = entity.GROUPID;
            }
            int result = await _siteService.doSiteCompanyUpdate(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }

        /// <summary>
        /// 插入监测对象及五方信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SiteCompanyInsert(GCSiteAndCompanyEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.sitename)
                || string.IsNullOrEmpty(entity.siteshortname) || string.IsNullOrEmpty(entity.siteaddr)
                || entity.GROUPID <= 0 || string.IsNullOrEmpty(entity.sitecity) || entity.sitetype <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            entity.@operator = _user.Name;
            entity.operatedate = DateTime.Now;

            int result = await _siteService.doSiteCompanyInsert(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }

        /// <summary>
        /// 获取单个工地的负责人列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SiteUserList(int SITEID)
        {
            return ResponseOutput.Ok(await _siteService.getSiteUserList(SITEID));
        }

        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> SaveSiteUser(SiteUserInput input)
        {
            input.@operator = _user.Name;
            int result = await _siteService.saveSiteUser(input);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }

        /// <summary>
        /// 获取安监通里有，site里没有的项目列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SiteUninput()
        {
            return ResponseOutput.Ok(await _siteService.GetSiteUninput());
        }

        /// <summary>
        /// 获取安监通的项目信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAqtProj(string siteajcode)
        {
            return ResponseOutput.Ok(await _siteService.GetAqtProj(siteajcode));
        }

        /// <summary>
        /// 获取未绑定扬尘设备的监测对象列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetNoDeviceList(int groupid)
        {
            return ResponseOutput.Ok(await _siteService.GetSiteNoDevice(groupid));
        }
    }
}
