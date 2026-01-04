using Microsoft.Extensions.DependencyInjection;

namespace api_gateway_microsservice.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}
