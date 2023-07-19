using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.NetModels;
using XHS.Build.Model.NetModels.Dtos;

namespace XHS.Build.Services.RealName
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<EmployeeInput, EmployeeEntity>();

            CreateMap<EmployeePassHisInsertInput, EmployeePassHisInsertEntity>();
        }
    }
}
