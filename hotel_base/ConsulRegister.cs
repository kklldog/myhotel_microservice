using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hotel_base
{
    public class ServiceInfo
    {
        public string Name { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string SHealthCheckAddress { get; set; }
    }

    public class ConsulRegister : IHostedService
    {
        string _serviceID = "";
        ServiceInfo _serviceInfo;
        public ConsulRegister(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;
            _serviceID = $"{_serviceInfo.Name}={_serviceInfo.IP}:{_serviceInfo.Port}_" + Guid.NewGuid().ToString();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri("");
            });
            await consulClient.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = _serviceID,
                Name = _serviceInfo.Name,// 服务名
                Address = _serviceInfo.IP, // 服务绑定IP
                Port = _serviceInfo.Port, // 服务绑定端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(1),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(30),//健康检查时间间隔
                    HTTP = _serviceInfo.SHealthCheckAddress,//健康检查地址
                    Timeout = TimeSpan.FromSeconds(10)
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri("");
            });
            await consulClient.Agent.ServiceDeregister(_serviceID);
        }
    }
}
