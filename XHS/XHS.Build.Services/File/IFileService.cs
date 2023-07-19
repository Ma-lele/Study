using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.File
{
    public interface IFileService:IBaseServices<FileEntity>
    {

        /// <summary>
        /// 插入文件
        /// </summary>
        /// <param name="param">文件信息</param>
        /// <returns>设置结果</returns>
        string doInsert(DBParam param);

        /// <summary>
        /// 更新文件文件
        /// </summary>
        /// <param name="param">文件信息</param>
        /// <returns>设置结果</returns>
        int doUpdate(DBParam param);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FILEID">文件ID</param>
        /// <returns>结果</returns>
        Task<int> doDelete(string FILEID);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="linkid">链接ID</param>
        /// <returns>结果</returns>
        Task<int> doDeleteByLinkid(string linkid);

        Task<List<FileEntity>> GetFileListByLindId(string linkid);

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="fileEntity"></param>
        /// <param name="tmb">是否缩略</param>
        /// <returns></returns>
        string GetImageUrl(FileEntity fileEntity,bool tmb=false);

        /// <summary>
        /// 获取临时文件路径
        /// </summary>
        /// <param name="fileEntity"></param>
        /// <returns></returns>
        string GetImageTempUrl(FileEntity fileEntity,bool tmb=false);

        /// <summary>
        /// 定时删除一天前被删除的文件
        /// </summary>
        Task DeleteFile();
    }
}
