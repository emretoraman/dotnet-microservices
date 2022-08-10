using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(p => p.ExternalId, o => o.MapFrom(ppd => ppd.Id));

            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
        }
    }
}
