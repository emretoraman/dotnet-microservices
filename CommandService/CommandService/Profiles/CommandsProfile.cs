using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(p => p.ExternalId, o => o.MapFrom(ppd => ppd.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(p => p.ExternalId, o => o.MapFrom(gpm => gpm.PlatformId));

            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
        }
    }
}
