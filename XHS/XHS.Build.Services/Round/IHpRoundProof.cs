using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XHS.Build.Services.Round
{
    public interface IHpRoundProof
    {
        /// <summary>
        /// 上传任务相关文件
        /// </summary>
        /// <param name="GROUPID">GROUPID</param>
        /// <param name="SITEID">SITEID</param> 
        /// <param name="ROUNDID">ROUNDID</param> 
        /// <param name="filename">文件名</param>
        /// <param name="fileString">文件源字符串</param>
        /// <param name="filesize">文件大小</param>
        /// <returns></returns>
        int doUpload(int GROUPID, int SITEID,   long ROUNDID,   string filename, string fileString, int filesize);


        /// <summary>
        /// 删除巡检目录下所有文件
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="ROUNDID">巡检ID</param>
        /// <returns></returns>
        int doDeleteAll(int GROUPID, int SITEID, long ROUNDID);

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="PROOFID">照片ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="createddate">拍摄日期</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        int doDelete(int GROUPID, int SITEID, long ROUNDID, string PROOFID, string filename);


        /// <summary>
        /// 巡查图片列表
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        DataTable getList(ulong ROUNDID);
    }
}
