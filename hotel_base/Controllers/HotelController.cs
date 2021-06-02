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
    public class HotelController : ControllerBase
    {
        private static List<HotelVM> _hotels = new List<HotelVM>() { 
            new HotelVM{ 
                Id="H8001",
                Name = "观前店",
                Phone = "50761111"
            },
            new HotelVM{
                Id="H8002",
                Name = "石路店",
                Phone = "50761112"
            },

        };

        private readonly ILogger<HotelController> _logger;

        public HotelController(ILogger<HotelController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<HotelVM> Get()
        {
            return _hotels;
        }

        [HttpGet("{id}")]
        public HotelVM Get(string id)
        {
            return _hotels.FirstOrDefault(x=>x.Id == id);
        }
    }
}
