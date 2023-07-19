using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;

namespace XHS.Build.Services.OperateLogService
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<OperateLogDto, OperateLog>();
            CreateMap<ElevatorRealDataInput, SpecialEqpData>();
            CreateMap<TowerCraneRealDataInput, SpecialEqpData>();
            CreateMap<OzoneRtdDataInput, OzoneRtdData>();
        }
    }
}
