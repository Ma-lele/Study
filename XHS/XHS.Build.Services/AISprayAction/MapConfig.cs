using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Services.AISprayAction.Dtos;

namespace XHS.Build.Services.AISprayAction
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<AISparyInputDto, AISprayActionEntity>();
        }
    }
}
