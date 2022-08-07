using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper,
            ICommandDataClient commandDataClient
        )
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
        {
            Console.WriteLine("--> Getting platforms...");

            var platforms = await _platformRepository.GetAllPlatforms();
            var platformReadDtos = _mapper.Map<List<PlatformReadDto>>(platforms);

            return Ok(platformReadDtos);
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting platform by Id...");

            var platform = await _platformRepository.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            return Ok(platformReadDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Creating platform...");

            var platform = _mapper.Map<Platform>(platformCreateDto);
            _platformRepository.CreatePlatform(platform);
            await _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(
                nameof(GetPlatformById),
                new { id = platformReadDto.Id },
                platformReadDto
            );
        }
    }
}
