using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Round
{
    public class SvRoundRepository : BaseRepository<BaseEntity>, ISvRoundRepository
    {
        public SvRoundRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// 巡查图片列表
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        public DataTable getList(ulong ROUNDID)
        {
            return Db.Ado.UseStoredProcedure().GetDataTable("spRoundProofList", new { ROUNDID = ROUNDID });
        }

        /// <summary>
        /// 巡查图片插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns>PROOFID</returns>
        public string doInsert(int GROUPID,
           int SITEID,
           string USERID,
           long ROUNDID,
           string filename,
           int filesize,
           string username)
        {
            var nameP1 = new SugarParameter("@GROUPID", GROUPID);
            var nameP2 = new SugarParameter("@SITEID", SITEID);
            var nameP3 = new SugarParameter("@USERID", USERID);
            var nameP4 = new SugarParameter("@ROUNDID", ROUNDID);
            var nameP6 = new SugarParameter("@filename", filename);
            var nameP7 = new SugarParameter("@filesize", filesize);
            var nameP8 = new SugarParameter("@operator", username);
            var nameP9 = new SugarParameter("@output", null, true);//isOutput=true
            Db.Ado.UseStoredProcedure().GetDataTable("spRoundProofInsert",nameP9, nameP1, nameP2, nameP3, nameP4, nameP6, nameP7, nameP8);

            return nameP9.Value.ToString();
        }

        /// <summary>
        /// 删除巡查图片
        /// </summary>
        /// <param name="PROOFID">PROOFID</param>
        /// <returns></returns>
        public int doDelete(string PROOFID)
        {
            return Db.Ado.UseStoredProcedure().GetInt("spRoundProofDelete", new { PROOFID = PROOFID });
        }
    }
}
