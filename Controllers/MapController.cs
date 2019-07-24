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
    [Route("map")]

    public class MapController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly iShoppingRepo _repo;
        public readonly iMapService _map;
        public MapController(IMapper mapper, iShoppingRepo repo, iMapService map)
        {
            _repo = repo;
            _mapper = mapper;
            _map = map;

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddStore(StoreDto Store)
        {
            var store = _mapper.Map<Store>(Store);
            _repo.Add(store);
            if (await _repo.SaveAll())
            {
                return Ok(store.Id);
            }
            return BadRequest();
        }

        [HttpPost("deviceDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> DeviceDetails(DeviceDetailsDto device)
        {
            var DeviceDetails = _mapper.Map<DeviceToken>(device);
            _repo.Add(DeviceDetails);
            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateStore(StoreDto Store)
        {
            var storeInfo = _mapper.Map<Store>(Store);
            storeInfo.Id = Store.Id;
            try
            {
                await _map.UpdateStore(storeInfo);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllStoreOfUser(string UserId)
        {
            var stores = await _map.GetAllStoresOfSeller(UserId);
            var storesinfo = _mapper.Map<IEnumerable<Store>>(stores);
            return Ok(storesinfo);
        }

        [HttpGet("id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStore(int id)
        {
            var stores = await _map.GetStore(id);
            var storesinfo = _mapper.Map<Store>(stores);
            return Ok(storesinfo);
        }

        [HttpDelete("id/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteStore(int Id)
        {
            var info = await _map.GetStore(Id);

            _repo.Delete(info);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();
        }

        [HttpDelete("{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteStore(string UserId)
        {
            var info = await _map.GetAllStoresOfSeller(UserId);

            _repo.DeleteAll(info);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest();
        }


        [HttpPost("map/{DeviceId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Check(string DeviceId, GPSDto[] value)
        {
            var DeviceInfo = await _map.GetDeviceDetails(DeviceId);
            if(DeviceInfo==null){
                return Ok();
            }
            Position pos1 = new Position();
            pos1.Latitude = value[0].lat;
            pos1.Longitude = value[0].lng;
            var stores = await _map.GetAllStores();

            TimeSpan timeDiff = (DeviceInfo.LastNotifyTime - DateTime.UtcNow);
            var time = Convert.ToInt32(timeDiff.Hours);

            if (time > 1)
            {
                return Ok();
            }

            if (DeviceInfo.Last_Lng == value[0].lng && DeviceInfo.Last_Lat == value[0].lat)
            {
                return Ok();
            }

            DeviceInfo.Last_Lng = value[0].lng;
            DeviceInfo.Last_Lat = value[0].lat;

            foreach (var store in stores)
            {

                Position pos2 = new Position();
                pos2.Latitude = store.lat;
                pos2.Longitude = store.lng;
                Haversine hv = new Haversine();
                double result = hv.Distance(pos1, pos2, DistanceType.Kilometers);

                if (result <= 5 && result > 0)
                {
                    string title = "Location";
                    string body = "You are Welcome";
                    var data = new { action = "Play", userId = 5 };
                    var pushSent = await PushNotification.SendPushNotification(DeviceInfo.FirebaseToken, title, body, data);
                    if(pushSent)
                    DeviceInfo.LastNotifyTime = DateTime.UtcNow;
                    return Ok(result);
                }
            }


            return Ok();
        }
    }
}