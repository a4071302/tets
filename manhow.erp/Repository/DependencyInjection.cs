using Interface.Repository.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Repository.MsSql.ManhowErp;
using Interface.Repository.MsSql.ManhowErp;
using Repository.MsSql.ErpDb;
using Repository.MsSql.PointToPointDb;


namespace Repository
{
    public static class DependencyInjection
    {



        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddSqlServerContext()
                .AddOracleContext()                
                .AddScoped<ITestManHowRepository, TestManHowRepository>()                
                .AddScoped<ITokenRepository, TokenRepository>()                
                ;
        }

        private static IServiceCollection AddSqlServerContext(this IServiceCollection services)
        {
            services.AddScoped<IPointToPointDbContext>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection("ConnectionStrings")["PointToPointDb"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new ArgumentNullException($"config需要設定PointToPointDb連線字串");

                var options = provider.GetService<ISqlServerOptions>();

                return options is null
                    ? new PointToPointDbContext(connectionString)
                    : new PointToPointDbContext(connectionString, options);
            });

            services.AddScoped<IErpDbContext>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection("ConnectionStrings")["ErpDb"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new ArgumentNullException($"config需要設定ErpDb連線字串");

                var options = provider.GetService<ISqlServerOptions>();

                return options is null
                    ? new ErpDbContext(connectionString)
                    : new ErpDbContext(connectionString, options);
            });


            return services;
        }

        private static IServiceCollection AddOracleContext(this IServiceCollection services)
        {
            //services.AddScoped<ISunlitDbContext>(provider =>
            //{
            //    var configuration = provider.GetRequiredService<IConfiguration>();
            //    var connectionString = configuration.GetSection("ConnectionStrings")["SunlitOracleDb"];

            //    if (string.IsNullOrWhiteSpace(connectionString))
            //        throw new ArgumentNullException($"config需要設定 SunlitOracleDb連線字串");

            //    return new SunlitDbContext(connectionString);
            //});

            return services;
        }

    }
}
