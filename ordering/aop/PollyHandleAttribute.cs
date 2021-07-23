using AspectCore.DynamicProxy;
using ordering.models;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ordering.aop
{
    public class PollyHandleAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryTimes { get; set; } 

        /// <summary>
        /// 是否熔断
        /// </summary>
        public bool IsCircuitBreaker { get; set; }

        /// <summary>
        /// 熔断前的异常次数
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; }

        /// <summary>
        /// 熔断时间
        /// </summary>
        public int SecondsOfBreak { get; set; }

        /// <summary>
        /// 降级方法
        /// </summary>
        public string FallbackMethod { get; set; }

        /// <summary>
        /// 一些方法级别统一计数的策略，比如熔断
        /// </summary>
        static ConcurrentDictionary<string, AsyncCircuitBreakerPolicy> policyCaches = new ConcurrentDictionary<string, AsyncCircuitBreakerPolicy>();

        public PollyHandleAttribute()
        {

        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Context pollyCtx = new Context();
            pollyCtx["aspectContext"] = context;

            Polly.Wrap.AsyncPolicyWrap policyWarp = null;

            var retry = Policy.Handle<HttpRequestException>().RetryAsync(RetryTimes);
            var fallback = Policy.Handle<Exception>().FallbackAsync(async (fallbackContent, token) =>
            {
                AspectContext aspectContext = (AspectContext)fallbackContent["aspectContext"];
                var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(this.FallbackMethod);
                var fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                aspectContext.ReturnValue = fallBackResult;
            }, async (ex, t) => { });
            AsyncCircuitBreakerPolicy circuitBreaker = null;
            if (IsCircuitBreaker)
            {
                var cacheKey = $"{context.ServiceMethod.DeclaringType.ToString()}_{context.ServiceMethod.Name}";
                if (policyCaches.TryGetValue(cacheKey, out circuitBreaker))
                {
                    //从缓存内获取该方法的全局熔断策略
                }
                else
                {
                    circuitBreaker = Policy.Handle<Exception>().CircuitBreakerAsync(
                      exceptionsAllowedBeforeBreaking: this.ExceptionsAllowedBeforeBreaking,
                      durationOfBreak: TimeSpan.FromSeconds(this.SecondsOfBreak));

                    policyCaches.TryAdd(cacheKey, circuitBreaker);
                }
            }

            if (circuitBreaker == null)
            {
                policyWarp = fallback.WrapAsync(retry);
            }
            else
            {
                policyWarp = fallback.WrapAsync(circuitBreaker.WrapAsync(retry));
            }


            await policyWarp.ExecuteAsync(ctx => next(context), pollyCtx);
        }
    }
}
