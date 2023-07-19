using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.File;
using XHS.Build.Services.Inspection;
using XHS.Build.Services.SystemSetting;
using static XHS.Build.Model.Models.FileEntity;
using static XHS.Build.Model.Models.InspectionEntity;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 移动执法
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;
        private readonly IHpFileDoc _hpfiledoc;
        private readonly IFileService _fileservice;
        private readonly IUser _user;
        private readonly IHpSystemSetting _hpSystemSetting;
        public InspectionController(IInspectionService inspectionService, IFileService fileService, IHpSystemSetting hpSystemSetting, IHpFileDoc hpFileDoc, IUser user)
        {
            _inspectionService = inspectionService;
            _fileservice = fileService;
            _hpfiledoc = hpFileDoc;
            _hpSystemSetting = hpSystemSetting;
            _user = user;
        }

        /// <summary>
        /// 移动执法插入
        /// </summary>
        /// <param name="param">移动执法参数</param>
        /// <returns>移动执法单号</returns>
        [HttpPost]
        public async Task<IResponseOutput> Create([FromForm] InspectionEntityParam param)
        {
            if (param.SITEID < 1 )
                return ResponseOutput.NotOk("请输入正确的参数");
            if (string.IsNullOrEmpty(param.remark))
            {
                param.remark = "";
            }
            try
            {
                int level = 1;
                string s164 = _hpSystemSetting.getSettingValue(Const.Setting.S164);
                if (!string.IsNullOrEmpty(s164))
                {
                    level = int.Parse(s164);
                    if (level < 1)
                    {
                        level = 1;
                    }
                }
                 string imgs = "A"+ DateTime.Now.ToString("yyMMddHHmmss");
                 string items = param.items;
                string itemanalyse = "";
                if (!string.IsNullOrEmpty(items) && items != "[]")
                {
                    string[] itemslist = items.Split("],[");
                    for(int i=0;i< itemslist.Length; i++)
                    {
                        string item = itemslist[i];
                        item = item.Replace("[", "").Replace("]", "").Replace("\"", "");
                        string[] itemlist = item.Split(",");
                        if(level > itemlist.Length)
                        {
                            level = itemlist.Length;
                        }
                        if(!itemanalyse.Contains(itemlist[level - 1] + "::")) {
                            itemanalyse = itemanalyse + itemlist[level - 1] + "::";
                        }
                }
                    itemanalyse = itemanalyse.Substring(0, itemanalyse.LastIndexOf("::"));
                }

                 object dbparam = new { GROUPID = _user.GroupId, SITEID = param.SITEID, USERID = _user.Id,
                                    items = param.items,
                                    itemnum = param.itemnum,
                                    itemanalyse = itemanalyse,
                                    insppoint = param.insppoint,
                                    insptype = param.insptype,
                                    remark = param.remark,
                                    imgs = imgs,
                                    CHARGERID = param.CHARGERID,
                                    insplevel = param.insplevel,
                                    limitdate = param.limitdate
                 };
                
                string inspcode = await _inspectionService.doInsert(dbparam);
                if (!string.IsNullOrEmpty(inspcode))
                {
                    FileEntityParam param1 = new FileEntityParam();
                   
                    JArray mJObj = JArray.Parse(param.files);
                    foreach (string file in mJObj)
                    {
                        param1 = new FileEntityParam();
                        param1.GROUPID = _user.GroupId;
                        param1.SITEID = param.SITEID;
                        param1.linkid = inspcode;
                        param1.filetype = FileEntity.FileType.Inspection;
                        param1.exparam = imgs;
                        await _hpfiledoc.doUpdate(file, param1, _user.Name);
                    }
                }
                
                return ResponseOutput.Ok(inspcode);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("添加移动执法记录失败");
            }
            
        }


        /// <summary>
        /// 移动执法更新
        /// </summary>
        /// <param name="param">移动执法参数</param>
        /// <returns>移动执法单号</returns>
        [HttpPost]
        public async Task<IResponseOutput> Update([FromForm] InspectionEntityParam param)
        {
            try
            {
               if(string.IsNullOrEmpty(param.remark)){
                    param.remark = "";
                }
                //  string[] str = list.ToArray();
                //string imgs = string.Join(",", list.ToArray());
                int level = 1;
                string s164 = _hpSystemSetting.getSettingValue(Const.Setting.S164);
                if (!string.IsNullOrEmpty(s164))
                {
                    level = int.Parse(s164);
                    if (level < 1)
                    {
                        level = 1;
                    }
                }
                string imgs = "A" + DateTime.Now.ToString("yyMMddHHmmss");
                string items = param.items;
                string itemanalyse = "";
                if (!string.IsNullOrEmpty(items) && items != "[]")
                {
                    string[] itemslist = items.Split("],[");
                    for (int i = 0; i < itemslist.Length; i++)
                    {
                        string item = itemslist[i];
                        item = item.Replace("[", "").Replace("]", "").Replace("\"", "");
                        string[] itemlist = item.Split(",");
                        if (level > itemlist.Length)
                        {
                            level = itemlist.Length;
                        }
                        if (!itemanalyse.Contains(itemlist[level - 1] + "::"))
                        {
                            itemanalyse = itemanalyse + itemlist[level - 1] + "::";
                        }
                    }
                    itemanalyse = itemanalyse.Substring(0, itemanalyse.LastIndexOf("::"));
                }
             
                object dbparam = new {
                    inspcode = param.inspcode,
                    USERID = _user.Id,
                    items = param.items,
                    itemnum = param.itemnum,
                    itemanalyse = itemanalyse,
                    insppoint = param.insppoint,
                    insptype = param.insptype,
                    remark = param.remark,
                    imgs = imgs,
                    CHARGERID = param.CHARGERID,
                    insplevel = param.insplevel,
                    limitdate = param.limitdate
                };

                int count = await _inspectionService.doUpdate(dbparam);
                if (count > 0) { 
                    DBParam param1 = new DBParam();
                    JArray deletefilesarr = JArray.Parse(param.deletefiles);
                    foreach (string delfile in deletefilesarr)
                    {
                        await _fileservice.doDelete(delfile.Substring(0, delfile.LastIndexOf(".")));
                    }
                    
                    FileEntityParam param2 = new FileEntityParam();
                    JArray mJObj = JArray.Parse(param.files);
                    foreach (string file in mJObj)
                    {
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = param.SITEID;
                        param2.linkid = param.inspcode;
                        param2.filetype = FileEntity.FileType.Inspection;
                        param2.exparam = imgs;
                        await _hpfiledoc.doUpdate(file, param2, _user.Name);
                    }
                    
                }
                else
                {
                    return ResponseOutput.NotOk("记录失败,可能已生成整改单。");
                }
                return ResponseOutput.Ok(count);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("更新移动执法记录失败");
            }

        }

        /// <summary>
        /// 获取问题类型
        /// </summary>
        [HttpGet]
        public async Task<IResponseOutput> GetItemList()
        {
            string url = _hpSystemSetting.getSettingValue(Const.Setting.S030)+"/"+_hpSystemSetting.getSettingValue(Const.Setting.S017) + "/inspectionitems.json";
            if (System.IO.File.Exists(url))
            {
                FileStream fileStream = new FileStream(url, FileMode.Open);
                StreamReader file_content = new StreamReader(fileStream);
                //读取文件内容
                string file_content_str = file_content.ReadToEnd();
              
                JArray mJObj = JArray.Parse(file_content_str);
                file_content.Close();
                fileStream.Close();
                if (mJObj != null)
                {
                    return ResponseOutput.Ok(mJObj);
                }
                else
                {
                    return ResponseOutput.NotOk("未取到数据");
                }
            }
            else
            {
                return ResponseOutput.NotOk("未取到数据");
            }
        }

        /// <summary>
        /// 创建整改单（整改单限时整改日期修改）
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <returns>移动执法单号</returns>
        [HttpPost]
        public async Task<IResponseOutput> ReformAdd([FromForm] string inspcode)
        {
            if (string.IsNullOrEmpty(inspcode))
                return ResponseOutput.NotOk("请输入正确的参数");

            try
            {
                object param = new { inspcode = inspcode, USERID = _user.Id};

                int ret = await _inspectionService.reformAdd(param);
                return ResponseOutput.Ok(ret);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("添加移动执法记录失败");
            }

        }
        /// <summary>
        /// 删除巡检
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Delete([FromForm] string inspcode)
        {
            if (string.IsNullOrEmpty(inspcode))
                return ResponseOutput.NotOk("请输入正确的参数");

            int result;
           
            result = await _inspectionService.doDelete(inspcode);
            if (result > 0)
            {
                await _fileservice.doDeleteByLinkid(inspcode);
            }
           
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 移动执法问题添加批示
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="limitdate">限时整改日期</param>
        /// <param name="deductpoint">整改扣分</param>
        /// <param name="remark">备注</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RemarkAdd([FromForm] string inspcode, [FromForm] int SITEID, [FromForm] DateTime? limitdate, [FromForm] int deductpoint =0, [FromForm] string remark="", [FromForm] string files="[]")
        {
            if (string.IsNullOrEmpty(inspcode) || string.IsNullOrEmpty(remark))
                return ResponseOutput.NotOk<int>("请填写处理信息");
            
            int result;
            if (string.IsNullOrEmpty(remark))
            {
                remark = "";
            }
            try
            {
                string imgs = "A" + DateTime.Now.ToString("yyMMddHHmmss");
                object param = new
                {
                    inspcode = inspcode,
                    USERID = _user.Id,
                    limitpoint = deductpoint,
                    limitdate = limitdate,
                    remark = remark,
                    imgs = imgs
                };
                result = await _inspectionService.doRemarkAdd(param);
                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    JArray mJObj = JArray.Parse(files);
                    foreach (string file in mJObj)
                    {
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = SITEID;
                        param2.linkid = inspcode;
                        param2.filetype = FileEntity.FileType.Inspection;
                        param2.exparam = imgs;
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
        /// 移动执法问题处理
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="solvedremark">处理日志</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Solve([FromForm] string inspcode, [FromForm] int SITEID, [FromForm] string solvedremark, [FromForm] string files)
        {
            if (string.IsNullOrEmpty(inspcode) ||  string.IsNullOrEmpty(solvedremark))
                return ResponseOutput.NotOk<int>("请填写处理信息");
            if (string.IsNullOrEmpty(solvedremark))
            {
                solvedremark = "";
            }
            int result = 0;
            List<string> list = new List<string>();
            try
            {
                

                //  string[] str = list.ToArray();
                string imgs = "B" + DateTime.Now.ToString("yyMMddHHmmss");
                object param = new
                {
                    inspcode = inspcode,
                    USERID = _user.Id,
                    imgs = imgs,
                    remark = solvedremark                    
                };
                result = await _inspectionService.doSolve(param);

                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    JArray mJObj = JArray.Parse(files);
                    foreach (string file in mJObj)
                    {
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = SITEID;
                        param2.linkid = inspcode;
                        param2.filetype = FileEntity.FileType.Inspection;
                        param2.exparam = imgs;
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
        /// 结束移动执法问题
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="deductpoint">整改扣分</param>
        /// <param name="remark">解决日志</param>
        /// <param name="files">新追加图片信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Finish([FromForm] string inspcode, [FromForm] int SITEID, [FromForm] int deductpoint = 0, [FromForm] string remark = "", [FromForm] string files ="[]")
        {
            if (string.IsNullOrEmpty(inspcode))
                return ResponseOutput.NotOk<int>("请填写处理信息");
            
            int result;
            if (string.IsNullOrEmpty(remark))
            {
                remark = "";
            }
            try
            {
                string imgs = "A" + DateTime.Now.ToString("yyMMddHHmmss");
                object param = new
                {
                    inspcode = inspcode,
                    USERID = _user.Id,
                    limitpoint = deductpoint,
                    imgs = imgs,
                    remark = remark
                };
                result = await _inspectionService.doFinish(param);
                if (result > 0)
                {
                    FileEntityParam param2 = new FileEntityParam();
                    JArray mJObj = JArray.Parse(files);
                    foreach (string file in mJObj)
                    {
                        param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = SITEID;
                        param2.linkid = inspcode;
                        param2.filetype = FileEntity.FileType.Inspection;
                        param2.exparam = imgs;
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
        /// 获取移动执法信息
        /// </summary>
        /// <param name="inspcode">移动执法单号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOne(string inspcode)
        {
            return ResponseOutput.Ok(await _inspectionService.getOne(inspcode));
        }

        /// <summary>
        /// 获取用户有权限移动执法工地别统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOrderListCount()
        { 
            return ResponseOutput.Ok(await _inspectionService.GetMobileListCount());
        }

        /// <summary>
        /// 移动执法，待办工单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOrderList(int siteid =0, long INSPID = 0, int datetype = 4, string insplevel = "",int status =0,int page=1,int size=10)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 1;
            }
            return ResponseOutput.Ok(await _inspectionService.GetOrderListUnSolve(siteid, INSPID,datetype, insplevel,status, page, size));
        }

        /// <summary>
        /// 获取站点下的移动执法单
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="INSPID"></param>
        /// <param name="datetype"></param>
        /// <param name="insplevel"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOrderSiteList(int siteid = 0, long INSPID = 0, int datetype = 4, string insplevel = "", int status = 0, int page = 1, int size = 10)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 1;
            }
            return ResponseOutput.Ok(await _inspectionService.GetOrderSiteList(siteid, INSPID, datetype, insplevel, status, page, size));
        }
    }
}