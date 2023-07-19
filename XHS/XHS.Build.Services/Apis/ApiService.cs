using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Apis
{
    public class ApiService : BaseServices<SysApisEntity>, IApiService
    {
        private readonly IBaseRepository<SysApisEntity> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ApiService(IBaseRepository<SysApisEntity> baseRepository, IUnitOfWork unitOfWork)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResponseOutput> AsyncApi(List<ApisInputDto> dtos)
        {
            var DBList = await _baseRepository.Query();
            try
            {
                _unitOfWork.BeginTran();
                foreach (var dto in dtos)
                {
                    if (DBList.Exists(a => a.ApiUrl.ToLower() == dto.ApiUrl))
                    {
                        var ExistApi = DBList.Find(a => a.ApiUrl.ToLower() == dto.ApiUrl);
                        ExistApi.Name = dto.Name;
                        await _baseRepository.Update(ExistApi);
                    }
                    else
                    {
                        SysApisEntity entity = new SysApisEntity() { Name = dto.Name, ApiUrl = dto.ApiUrl };
                        await _baseRepository.Add(entity);
                    }
                }
                _unitOfWork.CommitTran();
                return ResponseOutput.Ok();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                return ResponseOutput.NotOk();
            }
            
        }

        /// <summary>
        /// 同步swagger到sysapi表
        /// </summary>
        /// <param name="apis"></param>
        /// <returns></returns>
        public async Task<IResponseOutput> AsyncApiJson(JArray apis)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoSysApiInsert",
                new SugarParameter("@apis", apis.ToString()),
                returnvalue);
            if (returnvalue.Value.ObjToInt() > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk();
            }
        }
    }
}
