namespace Microservices.CommandsService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}
