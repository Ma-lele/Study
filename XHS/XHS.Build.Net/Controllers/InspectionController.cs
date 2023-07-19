using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.Inspection;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 《智慧工地对接智慧监管平台标准V1.0》
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Permission]

    public class InspectionController : ControllerBase
    {

        private readonly IAqtUploadService _aqtUploadService;
        private readonly IInspectionService _inspectionService;
        public InspectionController(IInspectionService inspectionService, IAqtUploadService aqtUploadService)
        {

            _aqtUploadService = aqtUploadService;
            _inspectionService = inspectionService;
        }



        /// <summary>
        /// 3.2.1检查单数据上传接口
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="checkNumber">检查单编号</param>
        /// <param name="checkDate">检查时间，例如：2019-4-07 11:32:12</param>
        /// <param name="checkPerson">检查人姓名，多人用;隔开</param>
        /// <param name="idCard">检查人身份证号</param>
        /// <param name="checkNumType">检查单类型：1：检查记录单 2：一般隐患单 3：严重隐患单</param>
        /// <param name="checkLists">检查单内容列表</param>
        /// <param name="IsProvinStand">是否符合省标准，0:否 1:是</param>
        /// <param name="itemId">检查项唯一id</param>
        /// <param name="checkContent">检查内容</param>
        /// <param name="rectifyPerson">整改负责人</param>
        /// <param name="isRectify">是否需要整改：(1:是、0:否)</param>
        /// <param name="rectifyDate">计划整改完成时间， 需要整改时必传</param>
        /// <param name="remark">检查单备注</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("InspectContentInfo")]
        public async Task<IResponseOutput> InspectContentInfo(InspectContentInfoDto dto)
        {
            if (string.IsNullOrEmpty(dto.checkPerson) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber))
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            if (dto.checkLists == null || dto.checkLists.Count <= 0)
            {
                return ResponseOutput.NotOk("检查单内容必须。");
            }
            if (dto.isRectify == 1 && dto.rectifyDate == null)
            {
                return ResponseOutput.NotOk("需要整改时计划整改完成时间必须。");
            }
            string checkContentsAnalyse = "";
            string rectifyPersons = "";
            int itemnum = dto.checkLists.Count;
            string strcheckContents = "";
            string urls = "";
            ArrayList checkContentsArr = new ArrayList();
            for (int i = 0; i < dto.checkLists.Count; i++)
            {
                CheckListData data = dto.checkLists[i];
                if (i == 0)
                {
                    checkContentsAnalyse = data.checkContent;
                    rectifyPersons = data.rectifyPerson;
                    strcheckContents = "[\"\",\"\",\"" + data.checkContent + "\"]";
                    if (data.urls != null)
                    {
                        urls = string.Join(",", data.urls.ToArray());
                    }
                }
                else
                {
                    checkContentsAnalyse = checkContentsAnalyse + ":" + data.checkContent;
                    rectifyPersons = rectifyPersons + "," + data.checkContent;
                    strcheckContents = strcheckContents + "," + "[\"\",\"\",\"" + data.checkContent + "\"]";
                    if (data.urls != null)
                    {
                        urls = urls + "," + string.Join(",", data.urls.ToArray());
                    }
                }
                checkContentsArr.Add(strcheckContents);
            }

            string checkContents = "[" + strcheckContents + "]";
            SgParams sp = new SgParams();
            foreach (PropertyInfo p in dto.GetType().GetProperties())
            {
                if (p.Name.Equals("checkLists"))
                {
                    continue;
                }
                sp.Add(p.Name, p.GetValue(dto));
            }
            sp.Add("checkContents", checkContents);
            sp.Add("rectifyPersons", rectifyPersons);
            sp.Add("checkContentsAnalyse", checkContentsAnalyse);
            sp.Add("itemnum", itemnum);
            sp.Add("urls", urls);
            sp.NeetReturnValue();
            int result = await _inspectionService.doFourInsert(sp);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }


        /// <summary>
        /// 3.2.2检查单整改完成数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="checkNumber">检查单编号</param>
        /// <param name="rectifyContents">整改内容</param>
        /// <param name="itemId">检查项唯一id</param>
        /// <param name="finalRectifyDate">整改完成时间</param>
        /// <param name="rectifyApprover">整改审批人</param>
        /// <param name="rectifyRemark">整改备注</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("RectifyContentInfo")]
        public async Task<IResponseOutput> RectifyContentInfo(RectifyContentInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber))
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            if (dto.rectifyContents == null || dto.rectifyContents.Count <= 0)
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            string rectifyRemarks = "";
            string urls = "";
            for (int i = 0; i < dto.rectifyContents.Count; i++)
            {
                rectifyContentsData data = dto.rectifyContents[i];
                if (i == 0)
                {
                    rectifyRemarks = data.rectifyRemark;
                    if (data.urls != null)
                    {
                        urls = string.Join(",", data.urls.ToArray());
                    }
                }
                else
                {
                    rectifyRemarks = rectifyRemarks + "," + data.rectifyRemark;
                    if (data.urls != null)
                    {
                        urls = urls + "," + string.Join(",", data.urls.ToArray());
                    }
                }
            }
            SgParams sp = new SgParams();
            foreach (PropertyInfo p in dto.GetType().GetProperties())
            {
                if (p.Name.Equals("rectifyContents"))
                {
                    continue;
                }
                sp.Add(p.Name, p.GetValue(dto));
            }
            sp.Add("rectifyRemarks", rectifyRemarks);
            sp.Add("urls", urls);
            sp.NeetReturnValue();
            int result = await _inspectionService.doFourFinish(sp);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else if (result == -2)
            {
                return ResponseOutput.NotOk("操作失败。（单号不存在或单子已完成）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }


        /// <summary>
        /// 3.2.4巡检点数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="inspectionId">巡检点id（唯一标识）</param>
        /// <param name="site">巡检地点描述</param>
        /// <param name="building">楼栋号</param>
        /// <param name="floor">楼层号</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("InspectionPoint")]
        public async Task<IResponseOutput> InspectionPoint(InspectionPointDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.inspectionId) || string.IsNullOrEmpty(dto.site))
            {
                return ResponseOutput.NotOk("参数不正确。");
            }

            object dbparam = new
            {
                recordNumber = dto.recordNumber,
                checkPointId = dto.inspectionId,
                summary = dto.site,
                building = dto.building,
                floor = dto.floor
            };
            int result = await _aqtUploadService.doUpdateCheckPoints(dbparam);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else if (result == -4)
            {
                return ResponseOutput.NotOk("操作失败。（巡检点不存在）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }

        /// <summary>
        /// 3.2.5巡检内容数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="inspectionId">巡检点id（唯一标识）</param>
        /// <param name="inspectionContentId">巡检记录id</param>
        /// <param name="checkPerson">检查人姓名</param>
        /// <param name="checkPersonId">检查人身份证id</param>
        /// <param name="checkContent">巡检描述</param>
        /// <param name="urls">巡检照片</param>
        /// <param name="inspectionTime">巡检时间（2019-07-07 12:24:34）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("InspectionPointContent")]
        public async Task<IResponseOutput> InspectionPointContent(InspectionPointContentDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.inspectionId) || string.IsNullOrEmpty(dto.inspectionContentId)
                || string.IsNullOrEmpty(dto.checkPerson) || string.IsNullOrEmpty(dto.checkPersonId) || string.IsNullOrEmpty(dto.checkContent) || dto.urls == null || dto.urls.Length <= 0)
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            string picUrls = "";
            for (int i = 0; i < dto.urls.Length; i++)
            {
                if (i == 0)
                {
                    picUrls = dto.urls[i];
                }
                else
                {
                    picUrls = picUrls + "," + dto.urls[i];
                }
            }
            object dbparam = new
            {
                recordNumber = dto.recordNumber,
                checkPointId = dto.inspectionId,
                checkPeople = dto.checkPerson,
                checkContent = dto.checkContent,
                picUrls = picUrls,
                checkDate = dto.inspectionTime
            };
            int result = await _aqtUploadService.doUpdateMobileCheck(dbparam);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }

        /// <summary>
        /// 3.2.6 随手拍数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="checkNumber">随手拍唯一编号</param>
        /// <param name="shootPerson">拍摄人</param>
        /// <param name="shootTime">拍摄时间</param>
        /// <param name="phoneNumber">手机号</param>
        /// <param name="CheckContent">隐患描述内容</param>
        /// <param name="url">照片全路径，必须可访问</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("FreeToShoot")]
        public async Task<IResponseOutput> FreeToShoot(FreeToShootDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.shootTime.ToString())
                || string.IsNullOrEmpty(dto.shootPerson) || string.IsNullOrEmpty(dto.phoneNumber) || string.IsNullOrEmpty(dto.CheckContent) || dto.urls == null || dto.urls.Length <= 0)
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            string picUrls = "";
            for (int i = 0; i < dto.urls.Length; i++)
            {
                if (i == 0)
                {
                    picUrls = dto.urls[i];
                }
                else
                {
                    picUrls = picUrls + "," + dto.urls[i];
                }
            }
            SgParams sp = new SgParams();
            sp.Add("urls", picUrls);
            sp.SetParams(dto,true);
            int result = await _aqtUploadService.doDownFourFreeToShoot(sp);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }

        /// <summary>
        /// 3.2.7 随手拍完成数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="checkNumber">随手拍唯一编号</param>
        /// <param name="rectifyTime">完成时间</param>
        /// <param name="rectifyPerson">整改负责人</param>
        /// <param name="rectifyRemark">备注</param>
        /// <param name="url">照片全路径，必须可访问</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("FreeToShootRectify")]
        public async Task<IResponseOutput> FreeToShootRectify(FreeToShootRectifyDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.rectifyTime.ToString())
                || string.IsNullOrEmpty(dto.rectifyPerson) || string.IsNullOrEmpty(dto.rectifyRemark) || dto.urls == null || dto.urls.Length <= 0)
            {
                return ResponseOutput.NotOk("参数不正确。");
            }
            string picUrls = "";
            for (int i = 0; i < dto.urls.Length; i++)
            {
                if (i == 0)
                {
                    picUrls = dto.urls[i];
                }
                else
                {
                    picUrls = picUrls + "," + dto.urls[i];
                }
            }
            
            SgParams sp = new SgParams();
            sp.Add("urls", picUrls);
            sp.SetParams(dto,true);
            int result = await _aqtUploadService.doDownFourFreeToShootRectify(sp);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。（项目未加白名单）");
            }
            else if (result == -2)
            {
                return ResponseOutput.NotOk("操作失败。（随手拍编号不存在或单子已完成）");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败。");
            }
        }


    }

}
