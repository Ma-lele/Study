using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Truck
{
    public interface ITruckService : IBaseServices<GCSiteTruckEntity>
    {
        Task<PageOutput<GCTruckPageListOutput>> GetList(string keyword, int page, int size);



    }
}
