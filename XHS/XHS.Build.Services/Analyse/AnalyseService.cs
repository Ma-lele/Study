using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;

namespace XHS.Build.Services.Analyse
{
    public class AnalyseService : BaseServices<AYEventLevel>, IAnalyseService
    {
        private readonly IBaseRepository<AYEventLevel> _repository;
        public AnalyseService(IBaseRepository<AYEventLevel> repository)
        {
            _repository = repository;
            BaseDal = repository;
        }

        public async Task<List<AYEventLevel>> GetAllPLAlgorithm()
        {
            _repository.CurrentDb = "XHS_Analyse";
            return await _repository.Query();
        }

        public async Task<List<SeverityDto>> GetSeverityData()
        {
            _repository.CurrentDb = "XHS_Analyse";
            return await _repository.Db.Queryable<AYEventType, AYSeverity>((type, seve) => new JoinQueryInfos(
                 JoinType.Left, type.ETID == seve.ETID
                 )).Select<SeverityDto>().ToListAsync();
        }


        public async Task<List<AYFrequencyType>> GetFrequencyData()
        {
            _repository.CurrentDb = "XHS_Analyse";
            return await _repository.Db.Queryable<AYFrequencyType>().ToListAsync();
        }


        public async Task<bool> SetPLAlgorithm(List<AYEventLevel> inputs)
        {
            _repository.CurrentDb = "XHS_Analyse";
            return await _repository.Db.Updateable(inputs)
                 .WhereColumns(ii => new { ii.FTCODE, ii.srlevel })
                .UpdateColumns(ii => new { ii.eventlevel })
                .ExecuteCommandHasChangeAsync();
        }


        public async Task<bool> SetSeverity(List<AYSeverity> input,bool isDel)
        {
            _repository.CurrentDb = "XHS_Analyse";
            bool success = await _repository.Db.Deleteable<AYSeverity>()
                .Where(ii => ii.ETID == input.FirstOrDefault().ETID)
                .ExecuteCommandHasChangeAsync();

            if (!isDel)
            {
                int rows = await _repository.Db.Insertable(input).ExecuteCommandAsync();
                success = rows > 0 ? true : false;
            }
          
            return success ? true : false;
        }

        public async Task<bool> SetFrequency(List<AYFrequencyType> inputs)
        {
            _repository.CurrentDb = "XHS_Analyse";
            return await _repository.Db.Updateable(inputs)
             .WhereColumns(ii => new { ii.FTCODE })
            .UpdateColumns(ii => new { ii.ftfrom, ii.ftto })
            .ExecuteCommandHasChangeAsync();
        }
    }
}
