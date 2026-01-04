using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy;

namespace api_gateway_microsservice.Extensions
{
    public static class ReverseProxyExtensions
    {
        public static IServiceCollection AddReverseProxyConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"));

            return services;
        }
    }
}
