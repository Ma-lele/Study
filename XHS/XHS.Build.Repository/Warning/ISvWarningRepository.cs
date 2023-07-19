using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Warning
{
    public interface ISvWarningRepository : IBaseRepository<BaseEntity>
    {
        /// <summary>
        /// 预警图片列表
        /// </summary>
        /// <param name="WARNID">巡查ID</param>
        /// <returns></returns>
        DataTable getList(ulong WARNID, int kind);

        /// <summary>
        /// 预警图片插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns>WPROOFID</returns>
         string doInsert(int GROUPID, int SITEID, string USERID, long WPID, int bsolved, string filename , long filesize, string username);

        /// <summary>
        /// 删除预警图片
        /// </summary>
        /// <param name="WPROOFID">WPROOFID</param>
        /// <returns></returns>
        int doDelete(string WPROOFID);
    }
}
