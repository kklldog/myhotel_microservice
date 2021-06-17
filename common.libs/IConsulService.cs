using Consul;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace common.libs
{
    public interface IConsulService
    {
        Task<List<AgentService>> GetServicesAsync(string serviceName);
    }
}