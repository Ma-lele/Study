using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.OperateLogS
{
    public interface IOperateLogService : IBaseServices<OperateLog>
    {
        /// <summary>
        /// 增加操作日志
        /// </summary>
        /// <param name="operateLog"></param>
        /// <returns></returns>
        Task<bool> AddAsync(MongoOperateLog operateLog);


        Task<bool> AddOpenApiLog(OpenApiOperateLog operateLog);

        Task<bool> AddCityOpenApiLog(CityOpenApiOperateLog operateLog);

        Task<bool> AddCityUploadApiLog(CityUploadOperateLog operateLog);

        Task<bool> AddSpecialEqpDataLog(SpecialEqpData Log);

        Task<bool> AddHighFormworkDataLog(HighFormworkData Log);

        Task<bool> AddUploadDataLog(UnloadInput Log);
        Task<bool> AddOzoneDataLog(OzoneRtdData Log);
        Task ClearMongoDBLog();
    }
}
