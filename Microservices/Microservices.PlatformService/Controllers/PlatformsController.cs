using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microservices.PlatformService.Data;
using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;
using Microservices.PlatformService.SyncDataServices.Http;
using Microservices.PlatformService.AsyncDataServices;

namespace Microservices.PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            IEnumerable<Platform> platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        [HttpGet("{id}", Name = nameof(GetPlatformById))]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Platform platformItem = _repository.GetPlatformById(id);
            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Platform platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            PlatformReadDto platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Send sync message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not send syncronously: {e.Message}");
            }

            // Send async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishPlatform(platformPublishedDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not send asyncronously: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
