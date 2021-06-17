using common.libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ordering.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private IConsulService _consulservice;
        public OrderController(ILogger<OrderController> logger, IConsulService consulService)
        {
            _consulservice = consulService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<OrderVM> Get()
        {
            return _orders;
        }

        [HttpGet("{id}")]
        public async Task<OrderVM> Get(string id)
        {
            var order = _orders.FirstOrDefault(x=>x.Id == id);
            if (!string.IsNullOrEmpty(order.MemberId))
            {
                var memberServiceAddresses = await _consulservice.GetServicesAsync("member_center");
                var memberServiceAddress = memberServiceAddresses.FirstOrDefault();
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri($"http://{memberServiceAddress.Address}:{memberServiceAddress.Port}");
                    var memberResult = await httpClient.GetAsync("/member/" + order.MemberId);
                    var json = await memberResult.Content.ReadAsStringAsync();
                    var member = JsonConvert.DeserializeObject<MemberVM>(json);
                    order.Member = member;
                }
            }

            return order;
        }

        [HttpGet("get_orders")]
        public List<OrderVM> Query(string day)
        {
            return _orders.Where(x => x.CreateDay == day).ToList();
        }
    }
}
