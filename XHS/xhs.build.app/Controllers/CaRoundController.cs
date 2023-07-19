using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.AssetInspection;
using XHS.Build.Services.File;
using XHS.Build.Services.Inspection;
using XHS.Build.Services.SystemSetting;
using static XHS.Build.Model.Models.CARound;
using static XHS.Build.Model.Models.FileEntity;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 资产巡检
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CaRoundController : ControllerBase
    {
        private readonly ICaRoundService _caRoundService;
        private readonly IHpFileDoc _hpfiledoc;
        private readonly IFileService _fileservice;
        private readonly IUser _user;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IAssetInspectionService _assetInspectionService;
        public CaRoundController(ICaRoundService caRoundService, IAssetInspectionService assetInspectionService,IFileService fileService, IHpSystemSetting hpSystemSetting, IHpFileDoc hpFileDoc, IUser user)
        {
            _caRoundService = caRoundService;
            _assetInspectionService = assetInspectionService;
            _fileservice = fileService;
            _hpfiledoc = hpFileDoc;
            _hpSystemSetting = hpSystemSetting;
            _user = user;
        }

        /// <summary>
        /// 资产巡检插入
        /// </summary>
        /// <param name="param">资产巡检参数</param>
        /// <returns>资产巡检单号</returns>
        [HttpPost]
        public async Task<IResponseOutput> Create(CARoundEntityParam param)
        {
            if (param.TEID < 1 )
                return ResponseOutput.NotOk("请输入正确的参数");
            if (string.IsNullOrEmpty(param.checkOklist))
            {
                if (param.checklist == null || param.checklist.Count == 0)
                {
                    return ResponseOutput.NotOk("该租户暂未创建检查项，请联系管理员。");
                }
            }
            try
            {
               
                 string imgs = "A"+ DateTime.Now.ToString("yyMMddHHmmss");
                string status = "4";
                if (param.checklist != null && param.checklist.Count > 0)
                {
                    status = "1";
                }
                List<SugarParameter> dbparam = new List<SugarParameter>();
                dbparam.Add(new SugarParameter("@GROUPID", _user.GroupId));
                dbparam.Add(new SugarParameter("@SITEID", param.SITEID));
                dbparam.Add(new SugarParameter("@USERID", _user.Id));
                dbparam.Add(new SugarParameter("@TEID", param.TEID));
                dbparam.Add(new SugarParameter("@status", status));
                dbparam.Add(new SugarParameter("@fellow", param.fellow));
                dbparam.Add(new SugarParameter("@limitdate", param.limitdate));
                string roundcode = "";
                string rounddata = await _caRoundService.doInsert(dbparam);
                if (!string.IsNullOrEmpty(rounddata))
                {
                   string roundid = rounddata.Split(",")[0];
                    roundcode = rounddata.Split(",")[1];
                    if (!string.IsNullOrEmpty(param.checkOklist))
                    {
                        dbparam = new List<SugarParameter>();
                        dbparam.Add(new SugarParameter("@GROUPID", _user.GroupId));
                        dbparam.Add(new SugarParameter("@SITEID", param.SITEID));
                        dbparam.Add(new SugarParameter("@USERID", _user.Id));
                        dbparam.Add(new SugarParameter("@ROUNDID", roundid));
                        dbparam.Add(new SugarParameter("@CLIDs", param.checkOklist));
                        await _caRoundService.doDetailOkInsert(dbparam);
                    }
                    if (param.checklist !=null && param.checklist.Count > 0)
                    {
                        for(int i=0;i< param.checklist.Count; i++)
                        {
                            CARoundDetailEntity entity = param.checklist[i];
                            dbparam = new List<SugarParameter>();
                            dbparam.Add(new SugarParameter("@GROUPID", _user.GroupId));
                            dbparam.Add(new SugarParameter("@SITEID", param.SITEID));
                            dbparam.Add(new SugarParameter("@USERID", _user.Id));
                            dbparam.Add(new SugarParameter("@ROUNDID", roundid));
                            dbparam.Add(new SugarParameter("@CLID", entity.CLID));
                            dbparam.Add(new SugarParameter("@remark", entity.remark));
                            dbparam.Add(new SugarParameter("@imgs", imgs + entity.CLID));
                            
                           int code = await _caRoundService.doDetailInsert(dbparam);
                           FileEntityParam param1 = new FileEntityParam();

                           // JArray mJObj = JArray.Parse(entity.files);
                            string[] files = entity.files.Split(",");
                            for ( int j = 0;j< files.Length;j++)
                            {
                                string file = files[j];
                                param1 = new FileEntityParam();
                                param1.GROUPID = _user.GroupId;
                                param1.SITEID = param.TEID;
                                param1.linkid = roundcode;
                                param1.filetype = FileEntity.FileType.CaRound;
                                param1.exparam = imgs + entity.CLID;
                                await _hpfiledoc.doUpdate(file, param1, _user.Name);
                            }
                        }
                    }
                   
                }
                
                return ResponseOutput.Ok(roundcode);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("添加资产巡检记录失败");
            }
            
        }



        /// <summary>
        /// 获取检查项列表
        /// </summary>
        /// <param name="teid">租户ID</param>
        ///  <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCheckListItems(int teid)
        {
            return ResponseOutput.Ok(await _caRoundService.GetCheckListByTenant(teid));            
        }

        /// <summary>
        /// 获取租户类型
        /// </summary>
        [HttpGet]
        public async Task<IResponseOutput> GetTenantTypeList()
        {
            Expression<Func<CATenantType, bool>> whereExpression = ii => ii.Status == 0;
            PageOutput<CATenantType> data = await _assetInspectionService.GetTenantTypeList(whereExpression, 1, 100);
            return ResponseOutput.Ok(data.data);
        }

     
        /// <summary>
        /// 删除巡检
        /// </summary>
        /// <param name="roundcode">资产巡检单号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Delete([FromForm] string roundcode)
        {
            if (string.IsNullOrEmpty(roundcode))
                return ResponseOutput.NotOk("请输入正确的参数");

            int result;
           
            result = await _caRoundService.doDelete(roundcode);
            if (result > 0)
            {
                await _fileservice.doDeleteByLinkid(roundcode);
            }
           
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 资产巡检问题验收
        /// </summary>
        /// <param name="roundcode">资产巡检单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="limitdate">限时整改日期</param>
        /// <param name="deductpoint">整改扣分</param>
        /// <param name="remark">备注</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ToCheck(CARoundDetailEntity entity)
        {
            if (string.IsNullOrEmpty(entity.roundcode) || string.IsNullOrEmpty(entity.remark))
                return ResponseOutput.NotOk<int>("请填写处理信息");

            if (entity.isqualified == 1)
            {
                return await Finish(entity);
            }
            int result;
            if (string.IsNullOrEmpty(entity.remark))
            {
                entity.remark = "";
            }
            try
            {
                string imgs = "A" + DateTime.Now.ToString("yyMMddHHmmss");
                List<SugarParameter>  dbparam = new List<SugarParameter>();
                dbparam.Add(new SugarParameter("@USERID", _user.Id));
                dbparam.Add(new SugarParameter("@roundcode", entity.roundcode));
                dbparam.Add(new SugarParameter("@RDID", entity.RDID));
                dbparam.Add(new SugarParameter("@remark", entity.remark));
                dbparam.Add(new SugarParameter("@imgs", imgs+ entity.CLID));
                result = await _caRoundService.doRemarkAdd(dbparam);
                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    string[] files = entity.files.Split(",");
                    for (int j = 0; j < files.Length; j++)
                    {
                        string file = files[j];
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = entity.TEID;
                        param2.linkid = entity.roundcode;
                        param2.filetype = FileEntity.FileType.CaRound;
                        param2.exparam = imgs + entity.CLID;
                        await _hpfiledoc.doUpdate(file, param2, _user.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 资产巡检问题处理
        /// </summary>
        /// <param name="roundcode">资产巡检单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="solvedremark">处理日志</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ToDeal(CARoundDetailEntity entity)
        {
            if (string.IsNullOrEmpty(entity.roundcode) || string.IsNullOrEmpty(entity.remark))
            {

                return ResponseOutput.NotOk<int>("请填写处理信息");
            }
            if (string.IsNullOrEmpty(entity.remark))
            {
                entity.remark = "";
            }
            int result = 0;
            try
            {
                //  string[] str = list.ToArray();
                string imgs = "B" + DateTime.Now.ToString("yyMMddHHmmss");
                List<SugarParameter> dbparam = new List<SugarParameter>();
                //param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
                //dbparam.Add(new SugarParameter("@USERID", _user.Id));
                dbparam.Add(new SugarParameter("@roundcode", entity.roundcode));
                dbparam.Add(new SugarParameter("@RDID", entity.RDID));
                dbparam.Add(new SugarParameter("@remark", entity.remark));
                dbparam.Add(new SugarParameter("@imgs", imgs + entity.CLID));
                result = await _caRoundService.doSolve(dbparam);
                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    string[] files = entity.files.Split(",");
                    for (int j = 0; j < files.Length; j++)
                    {
                        string file = files[j];
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = entity.TEID;
                        param2.linkid = entity.roundcode;
                        param2.filetype = FileEntity.FileType.CaRound;
                        param2.exparam = imgs + entity.CLID;
                        await _hpfiledoc.doUpdate(file, param2, _user.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok<int>(result);
        }

        /// <summary>
        /// 结束资产巡检问题
        /// </summary>
        /// <param name="roundcode">资产巡检单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="deductpoint">整改扣分</param>
        /// <param name="remark">解决日志</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        private async Task<IResponseOutput> Finish(CARoundDetailEntity entity)
        {
            if (string.IsNullOrEmpty(entity.roundcode))
                return ResponseOutput.NotOk<int>("请填写处理信息");
            
            int result;
            if (string.IsNullOrEmpty(entity.remark))
            {
                entity.remark = "";
            }
            try
            {
                string imgs = "A" + DateTime.Now.ToString("yyMMddHHmmss");
                List<SugarParameter> dbparam = new List<SugarParameter>();
                //param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
                dbparam.Add(new SugarParameter("@USERID", _user.Id));
                dbparam.Add(new SugarParameter("@roundcode", entity.roundcode));
                dbparam.Add(new SugarParameter("@RDID", entity.RDID));
                dbparam.Add(new SugarParameter("@remark", entity.remark));
                dbparam.Add(new SugarParameter("@imgs", imgs + entity.CLID));
                result = await _caRoundService.doFinish(dbparam);
                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    string[] files = entity.files.Split(",");
                    for (int j = 0; j < files.Length; j++)
                    {
                        string file = files[j];
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = entity.TEID;
                        param2.linkid = entity.roundcode;
                        param2.filetype = FileEntity.FileType.CaRound;
                        param2.exparam = imgs + entity.CLID;
                        await _hpfiledoc.doUpdate(file, param2, _user.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取资产巡检信息
        /// </summary>
        /// <param name="roundcode">资产巡检单号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOne(string roundcode)
        {
            return ResponseOutput.Ok(await _caRoundService.getOne(roundcode));
        }


        /// <summary>
        /// 资产巡检单（待验收或租户别）
        /// </summary>
        /// <param name="teid">租户ID</param>
        /// <param name="status">状态 0：所有1：待整改2：待验收3：整改合格4：检查合格</param>
        /// <param name="keyword">巡检单号</param>
        /// <param name="isoverhouronly">超期巡检单 0：所有 1：只显示超期巡检单</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页记录数默认10</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOrderList(DateTime startdate, DateTime enddate, int teid = 0, int isoverhouronly = 0, int status = 0, string keyword = "", int pageindex = 1, int pagesize = 10)
        {
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            if (pagesize < 1)
            {
                pagesize = 10;
            }
            return ResponseOutput.Ok(await _caRoundService.GetOrderList(startdate, enddate, teid, isoverhouronly,status, keyword, pageindex, pagesize));
        }

        /// <summary>
        /// 资产巡检统计列表
        /// </summary>
        /// <param name="siteid">项目ID</param>
        /// <param name="teid">租户ID</param>
        /// <param name="keyword">租户名称模糊查询</param>
        /// <param name="sort">综合排序</param>
        /// <param name="tenanttype">租户类型</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页记录数默认10</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOrderTenantList(DateTime startdate, DateTime enddate, int siteid = 0, int teid = 0, string keyword = "", int sort = 1, int tenanttype = 0, int pageindex = 1, int pagesize = 10)
        {
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            if (pagesize < 1)
            {
                pagesize = 10;
            }
            return ResponseOutput.Ok(await _caRoundService.GetOrderTenantList(startdate, enddate,siteid, teid, keyword, sort, tenanttype, pageindex, pagesize));
        }
    }
}