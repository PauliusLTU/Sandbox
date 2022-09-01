using AutoMapper;
using Grpc.Net.Client;
using Microservices.CommandsService.Models;
using Microservices.PlatformService;

namespace Microservices.CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        IEnumerable<Platform> IPlatformDataClient.ReturnAllPlatforms()
        {
            Console.WriteLine($"->> Calling Grpc Service {_configuration["GrpcPlatform"]}");

            GrpcChannel channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            GrpcPlatform.GrpcPlatformClient client = new GrpcPlatform.GrpcPlatformClient(channel);
            GetAllRequest request = new GetAllRequest();

            try
            {
                PlatformResponse response = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(response.Platform);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not call GRPC Server {e.Message}");
                return null;
            }
        }
    }
}
