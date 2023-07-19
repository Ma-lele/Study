using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.AssetInspection
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<InspectionTemplateDto, CACheckModel>();
            CreateMap<InspectionItems, CACheckList>()
              .ForMember(g => g.name, opt => opt.MapFrom(t => t.value));
        }
    }
}
