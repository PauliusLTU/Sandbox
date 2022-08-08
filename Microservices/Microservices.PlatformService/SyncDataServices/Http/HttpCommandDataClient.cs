using Microservices.PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace Microservices.PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        async Task ICommandDataClient.SendPlatformToCommand(PlatformReadDto platform)
        {
            StringContent httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/commands/platforms", httpContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync posto to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync posto to CommandService was NOT OK!");
            }
        }
    }
}
