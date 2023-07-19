using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Services.AIAirTightAction.Dtos;

namespace XHS.Build.Services.AIAirTightAction
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<AirTightInputDto, AIAirTightActionEntity>();
            CreateMap<AirTightInputDto, AirTightProcInputDto>();
        }
    }
}
