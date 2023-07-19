using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Elevator
{
    public class ElevatorService : BaseServices<ElevatorEntity>, IElevatorService
    {
        private readonly IBaseRepository<ElevatorEntity> _elevatorRepository;
        private readonly IUserKey _userKey;
        public ElevatorService(IBaseRepository<ElevatorEntity> elevatorRepository, IUserKey userKey)
        {
            base.BaseDal = elevatorRepository;
            _elevatorRepository = elevatorRepository;
            _userKey = userKey;
        }


        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        public async Task<int> doCheckin(string secode)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpCheckin", new SugarParameter("@secode", secode), output);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 发生报警后转移保存实时数据
        /// </summary>
        /// <param name="secode">特种设备编号</param>
        /// <param name="setype">特种设备种类</param>
        /// <returns></returns>
        public async Task<int> doPartInsert(string secode, int setype)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpPartInsert", new SugarParameter("@secode", secode), new SugarParameter("@setype", setype), output);
            return output.Value.ObjToInt();

        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        public async Task<int> doCheckout(string secode)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpCheckout", new SugarParameter("@secode", secode), output);
            return output.Value.ObjToInt();

        }

        /// <summary>
        /// 插入实时数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doRtdInsert(SgParams sp)
        {
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpRtdInsert", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 更新设备参数数据
        /// </summary>
        /// <param name="secode">特种设备编号</param>
        /// <param name="setype">特种设备种类</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxRange">最大幅度</param>
        /// <returns></returns>
        public async Task<int> doParamUpdate(SgParams sp)
        {
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpParamUpdate", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 发生防倾翻报警
        /// </summary>
        /// <param name="secode">特种设备编号</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <param name="tipOverData">防倾翻报警数据</param>
        /// <returns></returns>
        public async Task<int> doTipOverWarn(string secode, string starttime, string endtime, string tipOverData)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSpecialEqpTipWarnInsert", new SugarParameter("@secode", secode),
            new SugarParameter("@starttime", starttime),
            new SugarParameter("@endtime", endtime),
            new SugarParameter("@tipOverData", tipOverData), output);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 获取所有特种设备
        /// </summary>
        /// <returns></returns>
        public DataTable getAll()
        {
            return  _elevatorRepository.Db.Ado.GetDataTable("spSpecialEqpAll");
        }

        /// <summary>
        /// 插入塔吊数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doDyCraneRtdInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyCraneRtdInsert", ps);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 插入升降机数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doDyLiftRtdInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyLiftRtdInsert", ps);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 插入卸料平台数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doDyUnloadRtdInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _elevatorRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyUnloadRtdInsert", ps);
            return output.Value.ObjToInt();
        }
    }
}
