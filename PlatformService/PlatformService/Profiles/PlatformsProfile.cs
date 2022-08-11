using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(gpm => gpm.PlatformId, o => o.MapFrom(g => g.Id));

            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
        }
    }
}
