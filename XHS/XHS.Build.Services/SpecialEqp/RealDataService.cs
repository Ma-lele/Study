using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public class RealDataService : IRealDataService
    {
        private readonly IMongoDBRepository<SpecialEqpData> _dataRepository;
        public RealDataService(IMongoDBRepository<SpecialEqpData> dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<PageOutput<SpecialEqpData>> GetPage(string secode, string starttime, string endtime, int pageindex = 1, int pagesize = 10)
        {
            //var filter = Builders<SpecialEqpData>.Filter.Where(m => m.SeCode ==secode);
            Expression<Func<SpecialEqpData, bool>> expression = expression => true;
            if (!string.IsNullOrEmpty(secode))
            {
                expression = expression.And(a => a.SeCode == secode);
            }
            if (!string.IsNullOrEmpty(starttime))
            {
                expression = expression.And(a => a.UpdateTime >= Convert.ToDateTime(starttime));
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                expression = expression.And(a => a.UpdateTime <= Convert.ToDateTime(endtime));
            }
            FindOptions<SpecialEqpData, SpecialEqpData> findOpt = new FindOptions<SpecialEqpData, SpecialEqpData>();
            findOpt.Limit = pagesize;
            findOpt.Skip = (pageindex - 1) * pagesize;
            findOpt.Sort = Builders<SpecialEqpData>.Sort.Descending(m => m.Id);
            var result = (await _dataRepository.Collection.FindAsync(expression, findOpt)).ToList();

            var dataCount = await _dataRepository.Collection.CountDocumentsAsync(expression);

            var data = new PageOutput<SpecialEqpData>()
            {
                data = result,
                dataCount = dataCount
            };
            return data;
        }
    }
}
