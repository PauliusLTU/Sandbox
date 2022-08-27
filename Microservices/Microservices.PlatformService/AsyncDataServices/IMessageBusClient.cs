using Microservices.PlatformService.Dtos;

namespace Microservices.PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishPlatform(PlatformPublishedDto platformPublishedDto);
    }
}
