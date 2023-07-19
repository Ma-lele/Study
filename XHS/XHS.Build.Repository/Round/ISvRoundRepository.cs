using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Round
{
    public interface ISvRoundRepository : IBaseRepository<BaseEntity>
    {
        /// <summary>
        /// 巡查图片列表
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        DataTable getList(ulong ROUNDID);

        /// <summary>
        /// 巡查图片插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns>PROOFID</returns>
        string doInsert(
           int GROUPID,
           int SITEID,
           string USERID,
           long ROUNDID,
           string filename,
           int filesize,
           string username);

        /// <summary>
        /// 删除巡查图片
        /// </summary>
        /// <param name="PROOFID">PROOFID</param>
        /// <returns></returns>
        int doDelete(string PROOFID);
    }
}
