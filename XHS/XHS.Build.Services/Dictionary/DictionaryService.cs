using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Dictionary
{
    public class DictionaryService : BaseServices<CCDataDictionaryEntity>, IDictionaryService
    {
        private readonly IBaseRepository<CCDataDictionaryEntity> _baseRepository;

        public DictionaryService(IBaseRepository<CCDataDictionaryEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
    }
}
