using hotel_base.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotel_base.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private static List<RoomVM> _rooms = new List<RoomVM>() {
            new RoomVM{
                Floor =1,
                Id= "11",
                No = "1001",
                HotelId = "H8001"
            },
            new RoomVM{
             Floor =1,
                Id= "12",
                No = "1002",
                HotelId = "H8001"
            },
            new RoomVM{
             Floor =2,
                Id= "21",
                No = "2001",
                HotelId = "H8001"
            },
            new RoomVM{
             Floor =2,
                Id= "22",
                No = "2002",
                HotelId = "H8002"
            },
              new RoomVM{
             Floor =3,
                Id= "31",
                No = "3001",
                HotelId = "H8002"
            },
            new RoomVM{
             Floor =3,
                Id= "32",
                No = "3002",
                HotelId = "H8002"
            },
        };

        private readonly ILogger<RoomController> _logger;

        public RoomController(ILogger<RoomController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<RoomVM> Get()
        {
            return _rooms;
        }

        [HttpGet("{no}")]
        public RoomVM Get(string no)
        {
            return _rooms.FirstOrDefault(x=>x.No == no);
        }

        [HttpGet("hotel_rooms/{hotelId}")]
        public List<RoomVM> HotelRooms(string hotelId)
        {
            return _rooms.Where(x => x.HotelId == hotelId).ToList();
        }
    }
}
