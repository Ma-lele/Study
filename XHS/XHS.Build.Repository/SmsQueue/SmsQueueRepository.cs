using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.SmsQueue
{
    public class SmsQueueRepository: BaseRepository<SmsQueueEntity>, ISmsQueueRepository
    {
        public SmsQueueRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// 检索该监测点下的提醒
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>提醒信息数据集</returns>
        public async Task<DataTable> getList(int SITEID)
        {
            var nameP = new SugarParameter("@SITEID", SITEID);
            DataTable result =await Db.Ado.UseStoredProcedure().GetDataTableAsync("spSmsQueueList", nameP);
            return result;
        }

        /// <summary>
        /// 检索待发送提醒
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> getSendList()
        { 
            DataTable result = await Db.Ado.UseStoredProcedure().GetDataTableAsync("spSmsQueueSendList");
    
            return result;
        }

        /// <summary>
        /// 更新提醒状态
        /// </summary>
        /// <param name="SQID">提醒ID</param>
        /// <param name="resultmessage">提醒结果</param>
        /// <returns></returns>
        public async Task<int> doUpdate(long SQID, string resultmessage)
        {
            var param1 = new SugarParameter("@SQID", SQID);
            var param2 = new SugarParameter("@resultmessage", resultmessage);
            var output= new SugarParameter("@return", null, true);//isOutput=true
            await Db.Ado.UseStoredProcedure().GetDataTableAsync("spSmsQueueUpdate", param1, param2,output);
            
            return  output.Value.ObjToInt();
        }

        /// <summary>
        /// 插入提醒
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doInsert(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue); 
            var list = param.ToList();
            list.Add(output);
            await Db.Ado.UseStoredProcedure().GetIntAsync("spSmsQueueInsert", list);
            return output.Value.ObjToInt();
        }
    }
}
