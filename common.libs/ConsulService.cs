using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.libs
{
    public class ConsulService : IConsulService
    {
        public IConsulClient _consulClient;
        public ConsulService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task<List<AgentService>> GetServicesAsync(string serviceName)
        {
            var result = await _consulClient.Health.Service(serviceName, "", true);
            return result.Response.Select(x => x.Service).ToList();
        }
    }
}
