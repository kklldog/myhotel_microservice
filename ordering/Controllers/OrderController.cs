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
using Polly.CircuitBreaker;
using ordering.services;

namespace ordering.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private IConsulService _consulservice;
        private IMemberService _memberService;
        public OrderController(ILogger<OrderController> logger, IConsulService consulService, IMemberService memberService)
        {
            _consulservice = consulService;
            _logger = logger;
            _memberService = memberService;
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

        static AsyncCircuitBreakerPolicy circuitBreaker =  Policy.Handle<HttpRequestException>().CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 10,
            durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (ex, ts) =>
        {
            Console.WriteLine("circuitBreaker onBreak .");
        },
        onReset: () =>
        {
            Console.WriteLine("circuitBreaker onReset ");
        },
        onHalfOpen: () =>
        {
            Console.WriteLine("circuitBreaker onHalfOpen");
        }
        );

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
                var member = await _memberService.GetMemberInfo(order.MemberId);
                if (member != null)
                {
                    vm.Member = member;
                }
            }

            return vm;
        }

        [HttpGet("get_orders")]
        public async Task<IEnumerable<OrderVM>> Query(string day)
        {
            var orders = await FreeSQL.Instance.Select<Order>().Where(x => x.CreateDay == day).ToListAsync();

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
