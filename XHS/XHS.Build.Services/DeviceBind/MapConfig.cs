using AutoMapper;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.DeviceBind
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<DeviceBindInputDto, DeviceBindEntity>();
            CreateMap<GCHighFormwork, DeviceBindInputDto>()
                .ForMember(dto => dto.DeviceCode, entity => entity.MapFrom(a => a.hfwcode))
                .ForMember(dto => dto.UpdateDate, entity => entity.MapFrom(a => a.operatedate));
            CreateMap<GCElecMeterEntity, DeviceBindInputDto>()
              .ForMember(dto => dto.DeviceCode, entity => entity.MapFrom(a => a.emetercode))
              .ForMember(dto => dto.UpdateDate, entity => entity.MapFrom(a => a.operatedate));
            CreateMap<GCDeepPit, DeviceBindInputDto>()
                .ForMember(dto => dto.DeviceCode, entity => entity.MapFrom(a => a.dpcode))
                .ForMember(dto => dto.UpdateDate, entity => entity.MapFrom(a => a.operatedate));
            CreateMap<GCUnloadEntity, DeviceBindInputDto>()
                .ForMember(dto => dto.DeviceCode, entity => entity.MapFrom(a => a.unloadid))
                .ForMember(dto => dto.UpdateDate, entity => entity.MapFrom(a => a.operatedate));
            CreateMap<SpecialEntity, DeviceBindInputDto>()
                .ForMember(dto => dto.DeviceCode, entity => entity.MapFrom(a => a.secode))
                .ForMember(dto => dto.UpdateDate, entity => entity.MapFrom(a => a.operatedate));
            CreateMap<SiteDeviceEntity, SiteDeviceTypeDto>()
                .ForMember(dto => dto.attend, entity => entity.MapFrom(a => a.attendprojid))
                .ForMember(dto => dto.carwash, entity => entity.MapFrom(a => a.parkkey))
                .ForMember(dto => dto.helmet, entity => entity.MapFrom(a => a.helmetprojid))
                .ForMember(dto => dto.trespasser, entity => entity.MapFrom(a => a.trespasserprojid))
                .ForMember(dto => dto.stranger, entity => entity.MapFrom(a => a.strangerprojid))
                .ForMember(dto => dto.fire, entity => entity.MapFrom(a => a.fireprojid))
                .ForMember(dto => dto.liftover, entity => entity.MapFrom(a => a.liftoverprojid))
                .ForMember(dto => dto.amdisclose, entity => entity.MapFrom(a => a.amdiscloseprojid))
                .ForMember(dto => dto.airtight, entity => entity.MapFrom(a => a.airtightprojid))
                .ForMember(dto => dto.spray, entity => entity.MapFrom(a => a.sprayprojid))
                .ForMember(dto => dto.soil, entity => entity.MapFrom(a => a.soilprojid))
                .ForMember(dto => dto.vest, entity => entity.MapFrom(a => a.vestprojid));
           
        }
    }
}
