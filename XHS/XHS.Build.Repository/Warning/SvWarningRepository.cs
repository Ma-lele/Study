using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Warning
{
    public class SvWarningRepository : BaseRepository<BaseEntity>, ISvWarningRepository
    {
        public SvWarningRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// 预警图片列表
        /// </summary>
        /// <param name="WARNID">巡查ID</param>
        /// <returns></returns>
        public DataTable getList(ulong WARNID, int kind)
        {
            return Db.Ado.UseStoredProcedure().GetDataTable("spWarnProofList", new { WARNID = WARNID, kind = kind });

        }

        /// <summary>
        /// 预警图片插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns>WPROOFID</returns>
        public string doInsert(int GROUPID, int SITEID, string USERID, long WPID, int bsolved, string filename, long filesize, string username)
        {
            var nameP1 = new SugarParameter("@GROUPID", GROUPID);
            var nameP2 = new SugarParameter("@SITEID", SITEID);
            var nameP3 = new SugarParameter("@USERID", USERID);
            var nameP4 = new SugarParameter("@WPID", WPID);
            var nameP5 = new SugarParameter("@bsolved", bsolved);
            var nameP6 = new SugarParameter("@filename", filename);
            var nameP7 = new SugarParameter("@filesize", filesize);
            var nameP8 = new SugarParameter("@operator", username);
            var nameP9 = new SugarParameter("@output",null, true);//isOutput=true
            var dt= Db.Ado.UseStoredProcedure().GetString("spWarnProofInsert", nameP9, nameP1, nameP2, nameP3, nameP4, nameP5, nameP6, nameP7, nameP8);
            return nameP9.Value.ToString();
        }

        /// <summary>
        /// 删除预警图片
        /// </summary>
        /// <param name="WPROOFID">WPROOFID</param>
        /// <returns></returns>
        public int doDelete(string WPROOFID)
        {
            return Db.Ado.UseStoredProcedure().GetInt("spWarnProofDelete", new { WPROOFID = WPROOFID });

        }
    }
}
