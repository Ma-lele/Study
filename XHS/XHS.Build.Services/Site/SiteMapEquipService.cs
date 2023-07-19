using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Site
{
    public class SiteMapEquipService : BaseServices<GCSiteMapEquipEntity>, ISiteMapEquipService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSiteMapEquipEntity> _siteRepository;
        public SiteMapEquipService(IUser user, IBaseRepository<GCSiteMapEquipEntity> siteRepository)
        {
            _user = user;
            _siteRepository = siteRepository;
            BaseDal = siteRepository;
        }

        /// <summary>
        /// 获取监测点下设备地图点位
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        public async Task<DataTable> getListBySite(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteMapEquipList", new { SITEID = SITEID });
        }
    }
}
