using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Build.Services.Center
{
    public interface ICenterFile
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<object> upoadFile(IFormFile file);

        /// <summary>
        /// 网络地址下载本地
        /// </summary>
        /// <param name="downloadlink">网络地址</param>
        /// <returns></returns>
        Task<object> downloadFile(string downloadlink);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FileId"></param>
        /// <returns></returns>
        Task<bool> deleteFile(string FileId);
    }
}
