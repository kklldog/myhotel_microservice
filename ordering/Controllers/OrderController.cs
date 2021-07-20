using common.libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ordering.data;
using ordering.entities;
using ordering.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace ordering.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private IConsulService _consulservice;
        public OrderController(ILogger<OrderController> logger, IConsulService consulService)
        {
            _consulservice = consulService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderVM>> Get()
        {
            var orders = await FreeSQL.Instance.Select<Order>().ToListAsync();
            var vms = orders.Select(o => new OrderVM
            {
                Id = o.Id,
                StartDay = o.StartDay,
                EndDay = o.EndDay,
                RoomNo = o.RoomNo,
                MemberId = o.MemberId,
                HotelId = o.HotelId,
                CreateDay = o.CreateDay
            });

            return vms;
        }

        [HttpGet("{id}")]
        public async Task<OrderVM> Get(string id)
        {
            var order = await FreeSQL.Instance.Select<Order>().Where(x => x.Id == id).FirstAsync();
            var vm = new OrderVM
            {
                Id = order.Id,
                StartDay = order.StartDay,
                EndDay = order.EndDay,
                RoomNo = order.RoomNo,
                MemberId = order.MemberId,
                HotelId = order.HotelId,
                CreateDay = order.CreateDay
            };
            if (!string.IsNullOrEmpty(order.MemberId))
            {
                var memberServiceAddresses = await _consulservice.GetServicesAsync("member_center");
                var memberServiceAddress = memberServiceAddresses.FirstOrDefault();
                var member = await Policy.Handle<HttpRequestException>().RetryAsync(3).ExecuteAsync(async () =>
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri($"http://{memberServiceAddress.Address}:{memberServiceAddress.Port}");
                        var memberResult = await httpClient.GetAsync("/member/" + order.MemberId);
                        memberResult.EnsureSuccessStatusCode();
                        var json = await memberResult.Content.ReadAsStringAsync();
                        var member = JsonConvert.DeserializeObject<MemberVM>(json);
                        return member;
                    }
                });
                vm.Member = member;
            }

            return vm;
        }

        [HttpGet("get_orders")]
        public async Task<IEnumerable<OrderVM>> Query(string day)
        {
            var orders =  await FreeSQL.Instance.Select<Order>().Where(x=>x.CreateDay == day).ToListAsync();

            return orders.Select(o => new OrderVM
            {
                Id = o.Id,
                StartDay = o.StartDay,
                EndDay = o.EndDay,
                RoomNo = o.RoomNo,
                MemberId = o.MemberId,
                HotelId = o.HotelId,
                CreateDay = o.CreateDay
            });
        }
    }
}
