using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ordering.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ordering.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private static readonly List<OrderVM> _orders = new List<OrderVM>() { 
            new OrderVM { 
                Id = "OD001",
                StartDay = "2021-05-01",
                EndDay = "2021-05-02",
                RoomNo = "1001",
                MemberId = "M001",
                HotelId = "H8001",
                CreateDay = "2021-05-01"
            },
            new OrderVM {
                Id = "OD002",
                StartDay = "2021-05-03",
                EndDay = "2021-05-04",
                RoomNo = "1002",
                MemberId = "M002",
                HotelId = "H8001",
                CreateDay = "2021-05-03"
            },
        };

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<OrderVM> Get()
        {
            return _orders;
        }

        [HttpGet("{id}")]
        public OrderVM Get(string id)
        {
            return _orders.FirstOrDefault(x=>x.Id == id);
        }

        [HttpGet("get_orders")]
        public List<OrderVM> Query(string day)
        {
            return _orders.Where(x => x.CreateDay == day).ToList();
        }
    }
}
