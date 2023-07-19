using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.City.Dtos;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.DeepPit;
using XHS.Build.Services.Defence;
using XHS.Build.Services.HighFormwork;
using XHS.Build.Services.Inspection;
using XHS.Build.Services.Warning;

namespace XHS.Build.City.Controllers
{
    /// <summary>
    /// 《智慧工地对接智慧监管平台标准V1.0》
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class SmartDataUploadController : ControllerBase
    {
        private readonly IUserAqtKeyToken _userToken;
        private readonly ConfigHelper _configHelper;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IDefenceService _defenceService;
        private readonly IHighFormworkService _highFormworkService;
        private readonly IDeepPitService _deepPitService;
        private readonly IInspectionService _inspectionService;
        private readonly IWarningService _warningService;
        private readonly IUserKey _userKey;
        public SmartDataUploadController(IUserAqtKeyToken userToken, IUserKey userKey, IWarningService warningService, IDeepPitService deepPitService, IHighFormworkService highFormworkService, IInspectionService inspectionService, IDefenceService defenceService, IAqtUploadService aqtUploadService)
        {
            _userToken = userToken;
            _configHelper = new ConfigHelper();
            _aqtUploadService = aqtUploadService;
            _defenceService = defenceService;
            _inspectionService = inspectionService;
            _highFormworkService = highFormworkService;
            _deepPitService = deepPitService;
            _warningService = warningService;
            _userKey = userKey;
        }

        /// <summary>
        /// 3.1 鉴权接口
        /// </summary>
        /// <param name="appkey">平台分配的AppKey</param>
        /// <param name="appsecret">平台分配的AppSecret</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Security/getToken")]
        public async Task<string> getToken(string appkey, string appsecret)
        {
            string jwtStr = string.Empty;
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(appkey) || string.IsNullOrEmpty(appsecret))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string dAppId = appkey;//UEncrypter.DecryptByRSA16(key, MyConfig.Webconfig.PrivateKey);
            string dSecret = appsecret;//UEncrypter.DecryptByRSA16(secret, MyConfig.Webconfig.PrivateKey);

            var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
            if (keyList == null)
            {
                mJObj.Add("code", "10003");
                mJObj.Add("message", "appKey不存在。");
                return mJObj.ToString();
            }
            var keySecret = keyList.Items.FirstOrDefault(k => k.Key == dAppId && k.Secret == dSecret);
            if (keySecret == null || string.IsNullOrEmpty(keySecret.Key) || string.IsNullOrEmpty(keySecret.Secret))
            {
                mJObj.Add("code", "10004");
                mJObj.Add("message", "appkey和appSecret不匹配。");
                return mJObj.ToString();
            }


            var token = _userToken.Create(new[]
            {
                new Claim(KeyClaimAttributes.Key, keySecret.Key),
                new Claim(KeyClaimAttributes.GroupId, keySecret.GroupId==null?"":keySecret.GroupId),
                new Claim(KeyClaimAttributes.Name, keySecret.Name)
            });
            mJObj.Add("code", "200");
            mJObj.Add("message", "操作成功！");
            JObject data = new JObject();
            data.Add("accessToken", token);
            data.Add("expireTime", (DateTime.Now.AddMinutes(20).ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            mJObj.Add("data", data);
            return mJObj.ToString();
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
        [Route("Check/InspectContentInfo")]
        public async Task<string> InspectContentInfo(InspectContentInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.checkPerson) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            if (dto.checkLists == null || dto.checkLists.Count <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "检查单内容必须。");
                return mJObj.ToString();
            }
            if (dto.isRectify == 1 && dto.rectifyDate == null)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "需要整改时计划整改完成时间必须。");
                return mJObj.ToString();
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
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }

            return mJObj.ToString();
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
        [Route("Check/RectifyContentInfo")]
        public async Task<string> RectifyContentInfo(RectifyContentInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            if (dto.rectifyContents == null || dto.rectifyContents.Count <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "整改内容必须。");
                return mJObj.ToString();
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
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（单号不存在或单子已完成）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.2.3检查单数据看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="checkNumber">检查单编号</param>
        /// <param name="stereotacticBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Check/InspectBoard")]
        public async Task<string> InspectBoard(InspectBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.stereotacticBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = "",
                recordNumber = dto.recordNumber,
                boardtype = "checkorder",
                boardurl = dto.stereotacticBoardUrl,
                linkid = dto.checkNumber
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }

            return mJObj.ToString();
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
        [Route("Check/InspectionPoint")]
        public async Task<string> InspectionPoint(InspectionPointDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.inspectionId) || string.IsNullOrEmpty(dto.site))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
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
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else if (result == -4)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（巡检点不存在）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
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
        [Route("Check/InspectionPointContent")]
        public async Task<string> InspectionPointContent(InspectionPointContentDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.inspectionId) || string.IsNullOrEmpty(dto.inspectionContentId)
                || string.IsNullOrEmpty(dto.checkPerson) || string.IsNullOrEmpty(dto.checkPersonId) || string.IsNullOrEmpty(dto.checkContent) || dto.urls == null || dto.urls.Length <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
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
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
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
        [Route("Check/FreeToShoot")]
        public async Task<string> FreeToShoot(FreeToShootDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.shootTime.ToString())
                || string.IsNullOrEmpty(dto.shootPerson) || string.IsNullOrEmpty(dto.phoneNumber) || string.IsNullOrEmpty(dto.CheckContent) || dto.urls == null || dto.urls.Length <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
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
            sp.SetParams(dto);
            sp.Add("urls", picUrls);
            sp.NeetReturnValue();
            int result = await _aqtUploadService.doDownFourFreeToShoot(sp);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
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
        [Route("Check/FreeToShootRectify")]
        public async Task<string> FreeToShootRectify(FreeToShootRectifyDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.rectifyTime.ToString())
                || string.IsNullOrEmpty(dto.rectifyPerson) || string.IsNullOrEmpty(dto.rectifyRemark) || dto.urls == null || dto.urls.Length <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
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
            sp.SetParams(dto);
            sp.Add("urls", picUrls);
            sp.NeetReturnValue();
            int result = await _aqtUploadService.doDownFourFreeToShootRectify(sp);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -1)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（项目未加白名单）");
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。（随手拍编号不存在或单子已完成）");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.3.1 上传立体定位看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="stereotacticBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("StereotacticBoard")]
        public async Task<string> StereotacticBoard(StereotacticBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.stereotacticBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = "",
                recordNumber = dto.recordNumber,
                boardtype = "stereotactic",
                boardurl = dto.stereotacticBoardUrl,
                linkid = ""
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }



        /// <summary>
        /// 3.4.1 视频信息数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="videoId">视频地址的唯一id，在集成服务商系统中唯一，方便重复上传时做更新操作</param>
        /// <param name="type">视频类型（0：扬尘监控，1：塔吊监控，2：其它）</param>
        /// <param name="site">安装位置</param>
        /// <param name="url">视频跳转url</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DustVideo/UploadVideo")]
        public async Task<string> UploadVideo(UploadVideoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.videoId) || string.IsNullOrEmpty(dto.site) || string.IsNullOrEmpty(dto.url))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            GCSiteEntity data = await _aqtUploadService.GetVideoInfo(dto.recordNumber);
            if (data == null)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败，安监备案号未添加白名单。");
                return mJObj.ToString();
            }
            SgParams sp = new SgParams();
            sp.Add("operator", "第三方");
            sp.Add("cameraparam", dto.url);
            sp.Add("cameraname", dto.site);
            sp.Add("cameracode", dto.videoId);
            sp.Add("cameratype", 21);
            sp.Add("GROUPID", data.GROUPID);
            sp.Add("SITEID", data.SITEID);
            sp.Add("channel", dto.type == 0 || dto.type == 1 ? dto.type : 2);
            sp.NeetReturnValue();
            if (await _aqtUploadService.GetExists(dto.videoId, dto.type == 0 || dto.type == 1 ? dto.type : 2, 21, data.SITEID))
            {
                int result = await _aqtUploadService.doAqtCameraUpdate(sp);
                if (result > 0)
                {
                    mJObj.Add("code", "0");
                    mJObj.Add("message", "上传成功。");
                }
                else
                {
                    mJObj.Add("code", "10005");
                    mJObj.Add("message", "数据异常。");
                }
            }
            else
            {
                int result = await _aqtUploadService.doAqtCameraInsert(sp);
                if (result > 0)
                {
                    mJObj.Add("code", "0");
                    mJObj.Add("message", "上传成功。");
                }
                else
                {
                    mJObj.Add("code", "10005");
                    mJObj.Add("message", "数据异常。");
                }
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.5.1缺失记录上传（可选） 
        /// 3.5.2恢复记录上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="warnNumber">设备监测编号 (即deviceId)</param>
        /// <param name="defectPosition">缺失位置</param>
        /// <param name="defectWarnNumber">缺失预警模块编号（备用）</param>
        /// <param name="defectDate">发生时间（2020-01-10 10:00:00）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Fenceinterface/FenceAlarmInfo")]
        public async Task<string> FenceAlarmInfo(FenceAlarmInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.warnNumber))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            int result = 0;
            if (string.IsNullOrEmpty(dto.defectPosition))
            {
                if (dto.recoveryDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    mJObj.Add("code", "10001");
                    mJObj.Add("message", "恢复记录上传时recoveryDate必须。");
                    return mJObj.ToString();
                }
                result = await _defenceService.doFourRecover(dto.warnNumber, dto.recoveryDate);
            }
            else
            {
                if (dto.defectDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    mJObj.Add("code", "10001");
                    mJObj.Add("message", "缺失记录上传时defectDate必须。");
                    return mJObj.ToString();
                }
                result = await _defenceService.doFourDisconnect(dto.warnNumber, "4", dto.defectPosition, dto.defectDate);
            }
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.6.1 高支模实时数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="collectionTime">收集时间</param>
        /// <param name="Power">电量(%)</param>
        /// <param name="temperature">温度（℃）</param>
        /// <param name="load">立杆轴力(KN)</param>
        /// <param name="horizontalAngle">水平倾角（°）</param>
        /// <param name="coordinate">立杆倾角（°）</param>
        /// <param name="translation">水平位移（mm）</param>
        /// <param name="settlement">模板沉降（mm）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("HighFormwork/HighFormworkHistory")]
        public async Task<string> HighFormworkHistory(HighFormworkData dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            int result = await _highFormworkService.doRtdInsert(dto);

            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.6.2 高支模预警数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="warnExplain">报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)</param>
        /// <param name="warnContent">预警内容</param>
        /// <param name="happenTime">发生时间</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("HighFormwork/HighFormworkAlarmInfo")]
        public async Task<string> HighFormworkAlarmInfo(HighFormworkAlarmInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.warnExplain) || string.IsNullOrEmpty(dto.warnContent))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            int result = await _warningService.doHighFormwork(dto);

            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。(设备编号未加白名单)");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.6.3 上传高支模设备信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="uploadBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/HighFormworkBoard")]
        public async Task<string> HighFormworkBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.uploadBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = "",
                recordNumber = dto.recordNumber,
                boardtype = "highformwork",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.6.4 深基坑实时数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="collectionTime">采集时间</param>
        /// <param name="monitorType">监测项</param>
        /// <param name="warnValue">预警阀值</param>
        /// <param name="alarmValue">报警阀值</param>
        /// <param name="data">数值</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeepPit/DeepPitHistory")]
        public async Task<string> DeepPitHistory(DeepPitRtdDto dto)
        {
            JObject mJObj = new JObject();
            if (dto.monitorType <= 0 || dto.monitorType > 13 || (string.IsNullOrEmpty(dto.recordNumber) && string.IsNullOrEmpty(dto.dpCode)))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            MonitorType type = (MonitorType)dto.monitorType;

            string watchPoints = "";
            string watchPointValues = "";
            string watchPointExValues = "";
            if (dto.data.Count > 0)
            {
                foreach (DeepPitData d in dto.data)
                {
                    watchPoints = watchPoints + "," + d.watchPoint;
                    watchPointValues = watchPointValues + "," + d.watchPointValue;
                    watchPointExValues = watchPointExValues + "," + d.watchPointExValue;
                }
                watchPoints = watchPoints.Substring(watchPoints.IndexOf(",") + 1);
                watchPointValues = watchPointValues.Substring(watchPointValues.IndexOf(",") + 1);
                watchPointExValues = watchPointExValues.Substring(watchPointExValues.IndexOf(",") + 1);
            }
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Add("deviceId", dto.deviceId.Replace(":", ""));
            sp.Add("monitorType", type.ToString());
            sp.Add("data", JsonConvert.SerializeObject(dto.data));
            sp.Add("operator", _userKey.Name);
            sp.Add("watchPoints", watchPoints);
            sp.Add("watchPointValues", watchPointValues);
            sp.Add("watchPointExValues", watchPointExValues);
            sp.NeetReturnValue();
        

            int result = await _deepPitService.RtdInsert(sp);

            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();



        }

        /// <summary>
        /// 3.6.5 上传深基坑设备信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="uploadBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/DeppPitBoard")]
        public async Task<string> DeppPitBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.uploadBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = "",
                recordNumber = dto.recordNumber,
                boardtype = "depppit",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.7.1上传智慧监管整体看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="uploadBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/SmartSupervisionBoard")]
        public async Task<string> SmartSupervisionBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.uploadBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = "",
                recordNumber = dto.recordNumber,
                boardtype = "site",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return mJObj.ToString();
        }
    }

}
