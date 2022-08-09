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

            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
        }
    }
}
