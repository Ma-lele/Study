using AutoMapper;
using iTextSharp.text;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.NetModels;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.RealName
{
    public class RAttendService : BaseServices<BaseEntity>, IRAttendService
    {
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        private readonly IUserKey _userKey;
        private readonly IMapper _mapper;
        public RAttendService(IBaseRepository<BaseEntity> baseRepository, IUserKey userKey, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _userKey = userKey;
            _mapper = mapper;
        }

        public async Task<IResponseOutput> EmployeePassHisInsert(EmployeePassHisInsertInput input)
        {
            var entity = _mapper.Map<EmployeePassHisInsertEntity>(input);
            entity.@operator = _userKey.Name;
            List<SugarParameter> listParam = new List<SugarParameter>();
            foreach (PropertyInfo p in entity.GetType().GetProperties())
            {
                listParam.Add(new SugarParameter("@" + p.Name, p.GetValue(entity)));
            }
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            listParam.Add(returnvalue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spEmployeePassHisInsert", listParam);
            var retValue = Convert.ToInt32(returnvalue.Value);
            if (retValue == -27)
            {
                return ResponseOutput.NotOk("未找到相关项目", retValue);
            }
            if (retValue == -40)
            {
                return ResponseOutput.NotOk("数据不匹配", retValue);
            }
            if (retValue == -41)
            {
                return ResponseOutput.NotOk("人员基础数据不存在", retValue);
            }
            return retValue > 0 ? ResponseOutput.Ok(retValue) : ResponseOutput.NotOk("请求发生错误");
        }

        public async Task<IResponseOutput> EmployeeRegist(EmployeeInput input)
        {
            var entity = _mapper.Map<EmployeeEntity>(input);
            entity.GROUPID = _userKey.GroupId;
            entity.@operator = _userKey.Name;
            entity.jsonall = JsonConvert.SerializeObject(entity);

            SgParams sp = new SgParams();
            sp.SetParams(entity, true);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spEmployeeRegist", sp.Params);
            var retValue = sp.ReturnValue;

            if (retValue == -27)
            {
                return ResponseOutput.NotOk("未找到相关项目", retValue);
            }
            return retValue > 0 ? ResponseOutput.Ok(retValue) : ResponseOutput.NotOk("请求发生错误");
        }
    }
}
