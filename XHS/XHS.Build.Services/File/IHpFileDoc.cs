using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Model.Models;
using static XHS.Build.Model.Models.FileEntity;

namespace XHS.Build.Services.File
{
    public interface IHpFileDoc
    {
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="fep">文件参数</param>
        /// <param name="creater">创建者</param>
        /// <returns></returns>
        Task<string> doRegist(IFormFile file, FileEntity.FileEntityParam fep, string creater);

        Task<FileOutput> AddBase64Img(string base64string, FileEntityParam fileEntityParam = null);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="GROUPID">组编号</param>
        /// <param name="SITEID">监测点编号</param>
        /// <param name="linkid">链接ID</param>
        /// <param name="filetype">功能类型</param>
        /// <returns></returns>
        Task<int> doDelete(int GROUPID, int SITEID, string linkid, string filetype);


        /// <summary>
        /// 更新文档（将文档拷贝到正式目录）
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="fep">文件参数</param>
        /// <param name="creater">创建者</param>
        Task<bool> doUpdate(string file, FileEntity.FileEntityParam fep, string creater);

        /// <summary>
        /// 上传并压缩
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<FileOutput> UploadImgWithTmp(IFormFile file, string creater = "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileid"></param>
        /// <param name="fep"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        Task<bool> UpdateUploadImgTmp(string fileid, FileEntity.FileEntityParam fep, string creater="");

        /// <summary>
        /// 上传覆盖在地图上的工地平面图
        /// </summary>
        /// <param name="file">平面图图片文件</param>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">工地ID</param>
        /// <returns></returns>
        Task<bool> SaveSiteMapPlane(IFormFile file, int GROUPID, int SITEID);
        /// <summary>
        /// 获取工地平面图
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        Task<string> GetSiteMapPlane(int GROUPID, int SITEID, bool isForce = false);
        /// <summary>
        /// 删除工地平面图
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<bool> DeleteSiteMapPlane(int GROUPID, int SITEID);

  
    }
}
