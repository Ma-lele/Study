using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Analyse
{
    public interface IAnalyseService : IBaseServices<AYEventLevel>
    {
        Task<bool> SetPLAlgorithm(List<AYEventLevel> inputs);
        Task<List<AYEventLevel>> GetAllPLAlgorithm();
        Task<List<SeverityDto>> GetSeverityData();
        Task<bool> SetSeverity(List<AYSeverity> input,bool isDel);
        Task<List<AYFrequencyType>> GetFrequencyData();
        Task<bool> SetFrequency(List<AYFrequencyType> inputs);

    }
}
