using AutoMapper;
using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
        }
    }
}
