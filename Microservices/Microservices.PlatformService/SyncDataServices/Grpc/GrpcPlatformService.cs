using AutoMapper;
using Grpc.Core;
using Microservices.PlatformService.Data;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService: GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            PlatformResponse response = new PlatformResponse();

            IEnumerable<Platform> platforms = _repository.GetAllPlatforms();
            foreach (Platform platform in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

            return Task.FromResult(response);
        }
    }
}
