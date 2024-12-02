using Interface.Service.RouteQuery;
using Interface.Service.Token;
using Microsoft.Extensions.DependencyInjection;
using Service.RouteQuery;
using Service.Token;
using Utility.Helper;


namespace Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                 .AddSingleton<JwtHelper>()
                 .AddScoped<ITestManhowErpService, TestManhowErpService>()
                 .AddScoped<ITokenService, TokenService>()
                 //.AddScoped<JwtHelper>()
                ;
        }
    }
}
