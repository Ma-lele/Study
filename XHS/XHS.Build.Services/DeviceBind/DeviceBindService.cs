using AutoMapper;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeviceBind
{
    public class DeviceBindService : BaseServices<DeviceBindEntity>, IDeviceBindService
    {
        private readonly ICache _cache;
        private readonly IBaseRepository<DeviceBindEntity> _baseRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeviceBindService> _logger;
        public DeviceBindService(ICache cache, IBaseRepository<DeviceBindEntity> baseRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<DeviceBindService> logger)
        {
            _cache = cache;
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IResponseOutput> AddOrEditDeviceBind(List<DeviceBindInputDto> dtolist)
        {
            try
            {
                // _unitOfWork.BeginTran();
                if (dtolist.Any())
                {
                    //删除域名domain下已经存在的type
                    await _baseRepository.Db.Deleteable<DeviceBindEntity>().Where(a => dtolist.Select(d => d.DeviceType).Contains(a.DeviceType) && dtolist.Select(d => d.Domain).Contains(a.Domain)).ExecuteCommandAsync();
                    //取得过滤重复后的数据
                    var distinctList = dtolist.Where(d => !string.IsNullOrEmpty(d.DeviceCode) && !string.IsNullOrEmpty(d.Domain) && !string.IsNullOrEmpty(d.DeviceType)).Distinct(new DeviceBindCompare()).ToList();
                    var entitys = _mapper.Map<List<DeviceBindEntity>>(distinctList);
                    await _baseRepository.Db.Insertable(entitys).ExecuteCommandAsync();
                }
                //_unitOfWork.CommitTran();
                var DbList =await _baseRepository.Db.Queryable<DeviceBindEntity, ServerEntity>((d, s) => new JoinQueryInfos(JoinType.Inner, d.Domain == s.domain)).Select<DeviceBindOutput>().ToListAsync(); // await _baseRepository.Query(); //
                string key = XHS.Build.Common.Cache.CacheKey.DeviceBindCenterKey;
                _cache.Del(key);
                _cache.Set(key, DbList, TimeSpan.FromMinutes(10));
                return ResponseOutput.Ok();
            }
            catch (Exception e)
            {
                //_unitOfWork.RollbackTran();
                _logger.LogInformation(e.ToString());
                return ResponseOutput.NotOk("发生错误，请稍后再试");
            }

        }

        public async Task<List<DeviceBindOutput>> GetDeviceBindByCode(string devicecode)
        {
            var DBList = await GetDeviceBindList();
            return DBList.data.Where(a => a.DeviceCode == devicecode).ToList();
        }

        public async Task<List<DeviceBindOutput>> GetDeviceBindByCodeType(string devicecode, string devicetype)
        {
            var DBList = await GetDeviceBindList();
            return DBList.data.Where(a => a.DeviceCode == devicecode && a.DeviceType == devicetype).ToList();
        }

        public async Task<IResponseOutput<List<DeviceBindOutput>>> GetDeviceBindList()
        {
            var res = new ResponseOutput<List<DeviceBindOutput>>();
            string key = XHS.Build.Common.Cache.CacheKey.DeviceBindCenterKey;
            var cacheList = _cache.Get<List<DeviceBindOutput>>(key);
            if (cacheList == null)
            {
                var DbList = await _baseRepository.Db.Queryable<DeviceBindEntity, ServerEntity>((d, s) => new JoinQueryInfos(JoinType.Inner, d.Domain == s.domain)).Select<DeviceBindOutput>().ToListAsync();
                _cache.Set(key, DbList, TimeSpan.FromMinutes(10));
                return res.Ok(DbList);
            }
            else
            {
                return res.Ok(cacheList);
            }

        }

        public async Task<IResponseOutput> GetWaterEqpApiBindList(DateTime updatedate)
        {
            return ResponseOutput.Ok(await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWaterEqpApiBindList", new { updatedate = updatedate }));
        }

        public async Task<IResponseOutput> GetElectricEqpApiBindList(DateTime updatedate)
        {
            return ResponseOutput.Ok(await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spElectricEqpApiBindList", new { updatedate = updatedate }));
        }

        public async Task<IResponseOutput> GetSpecialEqpApiBindList(DateTime updatedate)
        {
            return ResponseOutput.Ok(await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSpecialEqpApiBindList", new { updatedate = updatedate }));
        }

        public async Task<IResponseOutput> GetSiteApiBindList(DateTime updatedate)
        {
            return ResponseOutput.Ok(await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteApiBindList", new { updatedate = updatedate }));
        }

        public async Task<DateTime> GetBindDatetime(string domain, string type)
        {
            var Special = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => a.DeviceType.ToLower() == type && a.Domain==domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();//.Query(a => a.DeviceType.ToLower() == "special" && a.Domain == domain, " updatedate desc ");//
            if (Special == null)
            {
                return new DateTime(2000, 1, 1);
            }
            else
            {
                return Special.UpdateDate;
            }
        }

        public async Task<DateTime> GetInvadeDatetime(string domain)
        {
            var Special = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => a.DeviceType.ToLower() == "invade" && a.Domain == domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();
            if (Special == null)
            {
                return new DateTime(2000, 1, 1);
            }
            else
            {
                return Special.UpdateDate;
            }
        }


        public async Task<DateTime> GetWaterDatetime(string domain)
        {
            var Special = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => a.DeviceType.ToLower() == "water" && a.Domain == domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();
            if (Special == null)
            {
                return new DateTime(2000, 1, 1);
            }
            else
            {
                return Special.UpdateDate;
            }
        }


        public async Task<DateTime> GetElectricDatetime(string domain)
        {
            var Special = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => a.DeviceType.ToLower() == "electric" && a.Domain == domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();
            if (Special == null)
            {
                return new DateTime(2000, 1, 1);
            }
            else
            {
                return Special.UpdateDate;
            }
        }
        public async Task<DateTime> GetSiteDatetime(string domain)
        {
            string[] types = { "stranger", "trespasser", "fire", "liftover", "helmet" };
           // var Site = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => (a.DeviceType.ToLower() == "stranger" || a.DeviceType.ToLower() == "trespasser" || a.DeviceType.ToLower() == "fire" || a.DeviceType.ToLower() == "liftover" || a.DeviceType.ToLower() == "helmet") && a.Domain == domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();//.Query(a => a.DeviceType.ToLower() != "special" && a.Domain == domain, " updatedate desc ");//
            var Site = await _baseRepository.Db.Queryable<DeviceBindEntity>().Where(a => types.Contains(a.DeviceType.ToLower()) && a.Domain==domain).OrderBy(a => a.UpdateDate, SqlSugar.OrderByType.Desc).FirstAsync();//.Query(a => a.DeviceType.ToLower() != "special" && a.Domain == domain, " updatedate desc ");//
            if (Site == null)
            {
                return new DateTime(2000, 1, 1);
            }
            else
            {
                return Site.UpdateDate;
            }
        }
    }

    public class DeviceBindCompare : IEqualityComparer<DeviceBindInputDto>
    {
        public bool Equals(DeviceBindInputDto x, DeviceBindInputDto y)
        {
            if (x == null || y == null)
                return false;
            if (x.DeviceType == y.DeviceType && x.Domain == y.Domain && x.DeviceCode == y.DeviceCode)
                return true;
            else
                return false;
        }

        public int GetHashCode(DeviceBindInputDto obj)
        {
            if (obj == null)
                return 0;
            else
                return obj.DeviceType.GetHashCode() ^ obj.Domain.GetHashCode() ^ obj.DeviceCode.GetHashCode();
        }
    }
}
