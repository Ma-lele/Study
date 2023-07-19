using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Dictionary
{
    public class GCRegionService:BaseServices<GCRegionEntity>,IGCRegionService
    {
        private readonly IBaseRepository<GCRegionEntity> _baseRepository;

        public GCRegionService(IBaseRepository<GCRegionEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
    }
}
