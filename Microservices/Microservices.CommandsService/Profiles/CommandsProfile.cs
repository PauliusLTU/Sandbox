﻿using AutoMapper;
using Microservices.CommandsService.Dtos;
using Microservices.CommandsService.Models;

namespace Microservices.CommandsService.Profiles
{
    public class CommandsProfile: Profile
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
        }
    }
}