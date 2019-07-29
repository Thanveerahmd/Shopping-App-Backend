using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GeoLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pro.backend.Dtos;
using pro.backend.Entities;
using pro.backend.Helpers;
using pro.backend.iServices;
using Project.Helpers;

namespace pro.backend.Controllers
{
    [ApiController]
    [Route("chat")]

    public class ChatController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        public readonly iMapService _map;
        public ChatController(IMapper mapper)
        {
            
            _mapper = mapper;
            

        }
    }
}