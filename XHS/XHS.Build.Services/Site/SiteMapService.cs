using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Site
{
    public class SiteMapService : BaseServices<GCSiteMapEntity>, ISiteMapService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSiteMapEntity> _siteRepository;
        public SiteMapService(IUser user, IBaseRepository<GCSiteMapEntity> siteRepository)
        {
            _user = user;
            _siteRepository = siteRepository;
            BaseDal = siteRepository;
        }

    }
}
