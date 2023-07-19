using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Systems
{
    public class SystemsService : BaseServices<SystemsEntity>, ISystemsService
    {
        private readonly IBaseRepository<SystemsEntity> _baseRepository;
        public SystemsService(IBaseRepository<SystemsEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            base.BaseDal = baseRepository;
        }
    }
}
