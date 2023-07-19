using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Photo;
using XHS.Build.Services.UavData;
using XHS.Build.Services.Video;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlyerController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IVideoService _videoService;
        private readonly IUavDataService _uavDataService;
        public FlyerController(IPhotoService photoService, IVideoService videoService, IUavDataService uavDataService)
        {
            _photoService = photoService;
            _videoService = videoService;
            _uavDataService = uavDataService;
        }

        /// <summary>
        /// 获取照片列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="createddate">拍摄日</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getphotolist")]
        public async Task<IResponseOutput> GetPhotoList(int SITEID, DateTime createddate)
        {
            List<BnSitePhoto> result = new List<BnSitePhoto>();
            DataTable dt = await _photoService.getOne(SITEID, createddate);
            if (dt == null || dt.Rows.Count.Equals(0))
                return ResponseOutput.Ok(result);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BnSitePhoto bs = new BnSitePhoto();
                bs.PHOTOID = Convert.ToInt32(dt.Rows[i]["PHOTOID"]);
                bs.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                bs.latitude = Convert.ToSingle(dt.Rows[i]["latitude"]);
                bs.longitude = Convert.ToSingle(dt.Rows[i]["longitude"]);
                bs.altitude = Convert.ToSingle(dt.Rows[i]["altitude"]);
                bs.path = Convert.ToString(dt.Rows[i]["path"]);
                bs.filename = Convert.ToString(dt.Rows[i]["filename"]);
                bs.filesize = Convert.ToInt32(dt.Rows[i]["filesize"]);
                bs.username = Convert.ToString(dt.Rows[i]["operator"]);
                bs.createddate = Convert.ToDateTime(dt.Rows[i]["createddate"]);
                bs.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                result.Add(bs);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取视频列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getVideoList")]
        public async Task<IResponseOutput> GetVideoList()
        {
            List<BnSiteVideo> result = new List<BnSiteVideo>();

            try
            {
                DataSet ds = await _videoService.getList();
                if (ds == null || ds.Tables.Count.Equals(0) || ds.Tables[0].Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSiteVideo bs = new BnSiteVideo();
                    bs.VIDEOID = Convert.ToInt32(dt.Rows[i]["VIDEOID"]);
                    bs.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                    bs.path = Convert.ToString(dt.Rows[i]["path"]);
                    bs.filename = Convert.ToString(dt.Rows[i]["filename"]);
                    bs.filesize = Convert.ToInt32(dt.Rows[i]["filesize"]);
                    bs.remark = Convert.ToString(dt.Rows[i]["remark"]);
                    bs.username = Convert.ToString(dt.Rows[i]["operator"]);
                    bs.createddate = Convert.ToDateTime(dt.Rows[i]["createddate"]);
                    bs.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                    result.Add(bs);
                }
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取飞行数据
        /// <param name="SITEID">监测点ID</param>
        /// <param name="createddate">拍摄日</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getUavDataList")]
        public async Task<IResponseOutput> GetUavDataList(int SITEID, DateTime createddate)
        {
            if (SITEID < 1 || createddate == null)
                return ResponseOutput.NotOk();

            List<BnSiteUavData> result = new List<BnSiteUavData>();

            try
            {
                DataTable dt = await _uavDataService.getList(SITEID, createddate);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSiteUavData bs = new BnSiteUavData();
                    bs.UAVDATAID = Convert.ToInt64(dt.Rows[i]["UAVDATAID"]);
                    bs.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                    bs.latitude = Convert.ToSingle(dt.Rows[i]["latitude"]);
                    bs.longitude = Convert.ToSingle(dt.Rows[i]["longitude"]);
                    bs.altitude = Convert.ToSingle(dt.Rows[i]["altitude"]);
                    bs.pm25 = Convert.ToDecimal(dt.Rows[i]["pm25"]);
                    bs.pm10 = Convert.ToDecimal(dt.Rows[i]["pm10"]);
                    bs.username = Convert.ToString(dt.Rows[i]["operator"]);
                    bs.createddate = Convert.ToDateTime(dt.Rows[i]["createddate"]);
                    bs.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                    result.Add(bs);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return ResponseOutput.Ok(result);
        }
    }
}