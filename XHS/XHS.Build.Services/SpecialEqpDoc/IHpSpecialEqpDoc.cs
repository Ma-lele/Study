using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common;

namespace XHS.Build.Services.SpecialEqpDoc
{
    public interface IHpSpecialEqpDoc
    {
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="file">POST提交的文件</param>
        /// <param name="param">其他信息</param>
        /// <param name="limitsize">压缩后最大size（k）</param>
        /// <returns></returns>
        int doRegist(IFormFile file, DBParam param, int limitsize);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="SEDOCID">文档ID</param>
        /// <returns></returns>
        int doDelete(string id);

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true);
    }
}
