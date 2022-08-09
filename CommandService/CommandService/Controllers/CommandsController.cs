using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Getting commands for platformId: {platformId}...");

            var platformExists = await _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound();
            }

            var commands = await _commandRepository.GetCommandsForPlatform(platformId);

            var commandReadDtos = _mapper.Map<List<PlatformReadDto>>(commands);

            return Ok(commandReadDtos);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting command for platformId: {platformId} and commandId: {commandId}...");

            var platformExists = await _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound();
            }

            var command = await _commandRepository.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }

            var commandReadDto = _mapper.Map<PlatformReadDto>(command);

            return Ok(commandReadDto);
        }

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Creating command for platformId: {platformId}...");

            var platformExists = await _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            _commandRepository.CreateCommand(platformId, command);
            await _commandRepository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(
                nameof(GetCommandForPlatform),
                new { platformId, commandId = commandReadDto.Id },
                commandReadDto
            );
        }
    }
}
