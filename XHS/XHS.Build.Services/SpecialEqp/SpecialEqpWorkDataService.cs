using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public class SpecialEqpWorkDataService : BaseServices<GCSpecialEqpWorkDataEntity>, ISpecialEqpWorkDataService
    {
        private readonly IBaseRepository<GCSpecialEqpWorkDataEntity> _baseRepository;
        public SpecialEqpWorkDataService(IBaseRepository<GCSpecialEqpWorkDataEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
    }
}
