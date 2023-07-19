using AutoMapper;
using XHS.Build.Model.Models;
using XHS.Build.Services.AIIllegalCarAction.Dtos;

namespace XHS.Build.Services.AIIllegalCarAction
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<IllegalCarInputDto, AIIllegalCarActionEntity>();
            CreateMap<IllegalCarInputDto, IllegalCarProcInputDto>();
        }
    }
}
