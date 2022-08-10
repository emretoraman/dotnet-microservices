using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    await AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private static EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event");

            var genericEventDto = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            var eventType = genericEventDto!.Event switch
            {
                "Platform_Published" => EventType.PlatformPublished,
                _ => EventType.Undetermined
            };

            Console.WriteLine(
                eventType == EventType.Undetermined
                    ? "--> Could not determine the event type"
                    : $"--> {eventType} event detected"
            );

            return eventType;
        }

        private async Task AddPlatform(string platformPublishedMessage)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
            var platform = _mapper.Map<Platform>(platformPublishedDto);

            try
            {
                var exists = await commandRepository.ExternalPlatformExists(platform.ExternalId);
                if (!exists)
                {
                    commandRepository.CreatePlatform(platform);
                    await commandRepository.SaveChanges();
                    Console.WriteLine("--> Platform added");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add Platform to DB: ${ex.Message}");
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
