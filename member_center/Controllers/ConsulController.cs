using common.libs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace member_center.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsulController : ControllerBase
    {
        IConsulService _consulservice;
        IConfiguration _configuration;
        public ConsulController(IConsulService consulservice, IConfiguration configuration)
        {
            _consulservice = consulservice;
            _configuration = configuration;
        }

        [HttpGet("getService")]
        public async Task<List<string>> Get(string name)
        {
            var services = await _consulservice.GetServicesAsync(name);

            return services.Select(x => x.Address + ":" + x.Port).ToList();
        }

        [HttpGet("getConfig")]
        public string GetConfig(string key)
        {
            return _configuration[key];
        }
    }
}
