using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Site
{
    public class MapShareService : BaseServices<GCSiteMapShareEntity>, IMapShareService
    {
        private readonly IBaseRepository<GCSiteMapShareEntity> _baseRepository;
        public MapShareService(IBaseRepository<GCSiteMapShareEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
    }
}
