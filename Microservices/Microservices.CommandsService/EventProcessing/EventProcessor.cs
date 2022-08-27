using AutoMapper;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.Dtos;
using Microservices.CommandsService.Models;
using System.Text.Json;

namespace Microservices.CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        void IEventProcessor.ProcessEvent(string message)
        {
            EventType eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;

                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event");

            GenericEventDto eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;

                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                ICommandRepo repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                PlatformPublishedDto platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    Platform platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExits(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> could not add platform to db {e.Message}");
                }
            }
        }
    }

    public enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
