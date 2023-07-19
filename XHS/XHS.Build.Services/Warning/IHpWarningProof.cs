using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Services.Warning
{
    public interface IHpWarningProof
    {
        /// <summary>
        /// 上传任务相关文件
        /// </summary>
        /// <param name="SITEID">SITEID</param>
        /// <param name="WPID">WPID</param>
        /// <param name="filename">文件名</param>
        /// <param name="fileString">文件源字符串</param>
        /// <param name="filesize">文件大小</param>
        /// <returns></returns>
        int doUpload(int SITEID, long WPID, int bsolved, string filename, string fileString, long filesize);

        /// <summary>
        /// 删除巡检目录下所有文件
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="WPID">WPID</param>
        /// <returns></returns>
        int doDeleteAll(int GROUPID, int SITEID, long WPID);

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="PROOFID">照片ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="WPID">WPID</param>
        /// <param name="PROOFID">PROOFID</param>
        /// <param name="createddate">拍摄日期</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        int doDelete(int GROUPID, int SITEID, long WPID, string PROOFID, string filename);
    }
}
