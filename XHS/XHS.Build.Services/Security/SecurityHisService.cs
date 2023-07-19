using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Security
{
    public class SecurityHisService : BaseServices<GCSecureHisEntity>, ISecurityHisService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSecureHisEntity> _baseRepository;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SecurityHisService(IUser user, IBaseRepository<GCSecureHisEntity> baseRepository, IHpSystemSetting hpSystemSetting)
        {
            _user = user;
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _hpSystemSetting = hpSystemSetting;
        }


        public async Task<PageOutput<GCSecureHisEntity>> GetHisPageList(int siteid, int securityid, DateTime date, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSecureHisEntity>()
                //.Mapper(it => it.Files, it => it.Files.First().linkid)
                .Mapper((h, cache) =>
                {
                    //cache.get 内的方法永远只执行一次
                    List<FileEntity> allItems = cache.Get(ol => //ol的值等于db.queryable<order>().tolist()
                    {
                        //查询出所有的OrderId集合
                        var allOrderIds = ol.Select(x => x.SCHISID.ToString()).ToList();
                        //查询出N个OrderId所对应的OrderItem
                        return _baseRepository.Db.Queryable<FileEntity>().In(it => it.linkid, allOrderIds).Where(f => f.bdel == 0).OrderBy(" createdate asc ").ToList();
                    });
                    //allOrderItems已经查询出N个OrderId所需要的Iitem，我们在从allOrderItems取出我要的 
                    h.Files = allItems.Where(it => it.linkid == h.SCHISID.ToString() && it.filetype.ToLower()== "security").ToList();
                    h.scname = _baseRepository.Db.Queryable<GCSecurityEntity>().First(a => a.SECURITYID == h.SECURITYID).scname;
                })
                .WhereIF(securityid > 0, h => h.SECURITYID == securityid)
                .Where(h => h.SITEID == siteid && SqlFunc.DateIsSame(h.createddate, date))
                .OrderBy(h => h.SCHISID, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            if (list.Any())
            {
                string S034 = _hpSystemSetting.getSettingValue("S034");
                list.ForEach(item =>
                {
                    if (item.Files != null && item.Files.Any())
                    {
                        item.Files.ForEach(f =>
                        {
                            if (!string.IsNullOrEmpty(f.filename) || f.filename.Split('.').Length == 2)
                            {
                                f.path = "http://" + S034 + "/resourse/" + f.GROUPID + "/" + f.filetype + "/" + f.SITEID + "/" + f.linkid + "/" + f.FILEID + "." + f.filename.Split('.')[1];
                                f.tmbpath = "http://" + S034 + "/resourse/" + f.GROUPID + "/" + f.filetype + "/" + f.SITEID + "/" + f.linkid + "/tmb_" + f.FILEID + "." + f.filename.Split('.')[1];
                            }
                        });
                    }
                });
            }

            var data = new PageOutput<GCSecureHisEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<dynamic> GetMonthCount(int siteid, DateTime date)
        {
            var start = date.Year + "-" + date.Month + "-01 00:00:00";
            var end = date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
            var list= await _baseRepository.Db.Queryable<GCSecureHisEntity>().Where(h => h.SITEID == siteid && SqlFunc.Between(h.createddate, start, end)).Select(it => new
            {
                createddate = it.createddate.ToString("yyyy-MM-dd")//只取日期
            })
            .MergeTable()//将查询结果转成一个表
            .GroupBy(it => it.createddate)
            .Select(it => new MonthCount() { createddate = it.createddate, count = SqlFunc.AggregateCount(it.createddate) })
            .ToListAsync();
            if (list.Any())
            {
                list.ForEach(d => d.createddate = Convert.ToDateTime(d.createddate).ToString("yyyy-MM-dd"));
            }
            return list;
        }

        public async Task<Dictionary<string, List<SecureHisListOutput>>> GetHisALlList(int siteid, int securityid, DateTime date)
        {
            var DBList = await _baseRepository.Db.Queryable<GCSecureHisEntity, GCSecurityEntity>((h, s) => new JoinQueryInfos(JoinType.Inner, h.SECURITYID == s.SECURITYID))
                .Where((h, s) => h.SITEID == siteid && s.bdel==0 && SqlFunc.DateIsSame(h.createddate, date))
                .WhereIF(securityid > 0, h => h.SECURITYID == securityid)
                .OrderBy((h, s) => h.SCHISID, OrderByType.Desc)
                .Select<SecureHisListOutput>().ToListAsync();
            var keys = DBList.Distinct(new Compare()).Select(d => d.USERID).ToList();
            Dictionary<string, List<SecureHisListOutput>> dic = new Dictionary<string, List<SecureHisListOutput>>();
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    if (!string.IsNullOrEmpty(key) && !dic.ContainsKey(key))
                    {
                        dic[key] = DBList.Where(a => a.USERID == key).ToList();
                    }
                }
            }

            return dic;
        }
    }

    public class Compare : IEqualityComparer<SecureHisListOutput>
    {
        public bool Equals(SecureHisListOutput x, SecureHisListOutput y)
        {
            if (x == null || y == null)
                return false;
            if (x.USERID == y.USERID)
                return true;
            else
                return false;
        }

        public int GetHashCode(SecureHisListOutput obj)
        {
            if (obj == null)
                return 0;
            else
                return obj.USERID.GetHashCode();
        }
    }
}
