using common.libs;
using Microsoft.AspNetCore.Mvc;
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
        public ConsulController(IConsulService consulservice)
        {
            _consulservice = consulservice;
        }

        [HttpGet("getService")]
        public async Task<List<string>> Get(string name)
        {
            var services = await _consulservice.GetServicesAsync(name);

            return services.Select(x => x.Address + ":" + x.Port).ToList();
        }
    }
}
