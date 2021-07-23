using common.libs;
using Newtonsoft.Json;
using ordering.aop;
using ordering.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ordering.services
{
    public class MemberService : IMemberService
    {
        private IConsulService _consulservice;

        public MemberService(IConsulService consulService)
        {
            _consulservice = consulService;
        }

        [PollyHandle(IsCircuitBreaker = true, FallbackMethod = "GetMemberInfoFallback", ExceptionsAllowedBeforeBreaking = 5, SecondsOfBreak = 30, RetryTimes = 3)]
        public async Task<MemberVM> GetMemberInfo(string id)
        {
            var memberServiceAddresses = await _consulservice.GetServicesAsync("member_center");
            var memberServiceAddress = memberServiceAddresses.FirstOrDefault();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress =
                    new Uri($"http://{memberServiceAddress.Address}:{memberServiceAddress.Port}");
                var result = await httpClient.GetAsync("/member/" + id);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<MemberVM>(json);
                }
            }

            return null;
        }

        public MemberVM GetMemberInfoFallback(string id)
        {
            return null;
        }
    }
}
