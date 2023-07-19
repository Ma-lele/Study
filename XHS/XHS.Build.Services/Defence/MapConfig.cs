using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Defence
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<DefenceEntity, DefenceOutputDto>()
                .ForMember(d => d.OfflineDays, opt =>
                    opt.MapFrom(s => ((DateTime.Now - s.checkouttime).TotalDays < 9 ? 0 : (DateTime.Now - s.checkouttime).TotalDays - 9))
                );


        }
    }
}
