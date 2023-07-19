using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Model.MongoModels;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.OperateLogS
{
    public class OperateLogService : BaseServices<OperateLog>, IOperateLogService
    {
        private readonly IUser _user;
        private readonly IUserKey _userKey;
        private readonly IHttpContextAccessor _context;
        IBaseRepository<OperateLog> _dal;
        private readonly IMongoDBRepository<MongoOperateLog> _mongoDBRepository;
        private readonly IMongoDBRepository<OpenApiOperateLog> _openApiOperateLog;
        private readonly IMongoDBRepository<SpecialEqpData> _eqpRepository;
        private readonly IMongoDBRepository<AppExceptionLog> _appRepository;
        private readonly IMongoDBRepository<CityOpenApiOperateLog> _cityopenApiOperateLog;
        private readonly IMongoDBRepository<CityUploadOperateLog> _cityUploadOperateLog;
        private readonly IMongoDBRepository<UnloadInput> _unloadRepository;
        private readonly IMongoDBRepository<OzoneRtdData> _ozoneRepository;
        private readonly IMongoDBRepository<HighFormworkData> _highFormworkRepository;
        private static string[] KEYS = new string[] { "projid", "cwcode", "secode", "fc", "unload_id", "invadecode", "dpcode", "deviceid", "emetercode", "devicecode" };

        public OperateLogService(IUser user, IUserKey userKey, IMongoDBRepository<UnloadInput> unloadRepository, IHttpContextAccessor context, IBaseRepository<OperateLog> dal,
           IMongoDBRepository<CityOpenApiOperateLog> cityopenApiOperateLog, IMongoDBRepository<OzoneRtdData> ozoneRepository, IMongoDBRepository<HighFormworkData> highFormworkRepository, IMongoDBRepository<CityUploadOperateLog> cityUploadOperateLog, IMongoDBRepository<OpenApiOperateLog> openApiOperateLog, IMongoDBRepository<MongoOperateLog> mongoDBRepository, IMongoDBRepository<SpecialEqpData> eqpRepository, IMongoDBRepository<AppExceptionLog> appRepository)
        {
            _user = user;
            _context = context;
            _dal = dal;
            base.BaseDal = dal;
            _mongoDBRepository = mongoDBRepository;
            _openApiOperateLog = openApiOperateLog;
            _cityopenApiOperateLog = cityopenApiOperateLog;
            _cityUploadOperateLog = cityUploadOperateLog;
            _eqpRepository = eqpRepository;
            _appRepository = appRepository;
            _userKey = userKey;
            _unloadRepository = unloadRepository;
            _ozoneRepository = ozoneRepository;
            _highFormworkRepository = highFormworkRepository;
        }

        public async Task<bool> AddAsync(MongoOperateLog operateLog)
        {
            operateLog.UserId = _user.Id;
            operateLog.GroupId = _user.GroupId.ToString();
            operateLog.IP = IPHelper.GetIP(_context?.HttpContext?.Request);
            await _mongoDBRepository.InsertAsync(operateLog);
            return true;
        }

        public async Task<bool> AddOpenApiLog(OpenApiOperateLog operateLog)
        {
            operateLog.Name = _userKey.Name;
            operateLog.Key = _userKey.Key;
            operateLog.IP = IPHelper.GetIP(_context?.HttpContext?.Request);

            //对日志统一添加keycode字段
            if (operateLog.ApiMethod.Equals("post", StringComparison.OrdinalIgnoreCase) 
                && operateLog.ApiPath.IndexOf("api/access/",StringComparison.OrdinalIgnoreCase) < 0
                && operateLog.Body.StartsWith("{") && operateLog.Body.EndsWith("}"))
            {
                JObject jbody = JObject.Parse(operateLog.Body);
                operateLog.KeyCode = GetKeyCode(jbody);
            }

            await _openApiOperateLog.InsertAsync(operateLog);
            return true;
        }

        public async Task<bool> AddCityOpenApiLog(CityOpenApiOperateLog operateLog)
        {
            operateLog.Name = _userKey.Name;
            operateLog.Key = _userKey.Key;
            operateLog.IP = IPHelper.GetIP(_context?.HttpContext?.Request);
            await _cityopenApiOperateLog.InsertAsync(operateLog);
            return true;
        }

        public async Task<bool> AddCityUploadApiLog(CityUploadOperateLog operateLog)
        {
            await _cityUploadOperateLog.InsertAsync(operateLog);
            return true;
        }


        public async Task<bool> AddSpecialEqpDataLog(SpecialEqpData log)
        {
            await _eqpRepository.InsertAsync(log);
            return true;
        }

        public async Task<bool> AddHighFormworkDataLog(HighFormworkData log)
        {
            await _highFormworkRepository.InsertAsync(log);
            return true;
        }

        public async Task<bool> AddUploadDataLog(UnloadInput log)
        {
            await _unloadRepository.InsertAsync(log);
            return true;
        }


        public async Task<bool> AddOzoneDataLog(OzoneRtdData log)
        {
            await _ozoneRepository.InsertAsync(log);
            return true;
        }

        public async Task ClearMongoDBLog()
        {
            await _mongoDBRepository.DeleteManyAsync(a => a.CreateTime < DateTime.Now.AddMonths(-3));
            await _openApiOperateLog.DeleteManyAsync(a => a.CreateTime < DateTime.Now.AddMonths(-3));
            await _eqpRepository.DeleteManyAsync(a => a.CreateTime < DateTime.Now.AddMonths(-3));
            var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Lte("reportDate", DateTime.Now.AddMonths(-3));
            await _appRepository.DeleteManyAsync(filter);
        }

        /// <summary>
        /// 获取KeyCode
        /// </summary>
        /// <param name="jo">body</param>
        /// <returns></returns>
        public string GetKeyCode(JObject jo)
        {
            string result;
            foreach (var key in KEYS)
            {
                result = jo.GetValue(key, StringComparison.OrdinalIgnoreCase)?.ToString();
                if (!string.IsNullOrEmpty(result))
                    return result;
            }
            return null;
        }
    }
}
