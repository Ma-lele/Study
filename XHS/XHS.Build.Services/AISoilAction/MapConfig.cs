using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Services.AISoilAction.Dtos;

namespace XHS.Build.Services.AISoilAction
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<AISoilInputDto, AISoilActionEntity>();
            CreateMap<AISoilInputDto, AISoilProcInputDto>();
        }
    }
}
