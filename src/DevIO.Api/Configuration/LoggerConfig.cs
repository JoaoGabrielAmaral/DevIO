using DevIO.Api.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var helmaIo = new
            {
                apiKey = "bcb8814b1c264f48b1633af3d5f8c40e",
                logId = new Guid("9828f062-3d67-43d9-80e6-b43fedc60495")
            };

            services.AddElmahIo(
                o =>
                {
                    o.ApiKey = helmaIo.apiKey;
                    o.LogId = helmaIo.logId;
                }
            );

            #region Recupera os logs adicionados manualmente
            //services.AddLogging(
            //    b =>
            //    {
            //        b.AddElmahIo(
            //            o =>
            //            {
            //                o.ApiKey = "bcb8814b1c264f48b1633af3d5f8c40e";
            //                o.LogId = new Guid("9828f062-3d67-43d9-80e6-b43fedc60495");
            //            }
            //        );

            //        b.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //    }
            //);
            #endregion

            services.AddHealthChecks()
                .AddElmahIoPublisher(o =>
                {
                    o.ApiKey = helmaIo.apiKey;
                    o.LogId = helmaIo.logId;
                    o.HeartbeatId = "009a6403832e427abc051ee28c2438fd";
                })
                .AddCheck("Produtos", new SqlServerHealthCheck(connectionString))
                .AddSqlServer(connectionString, name: "BancoSQL");

            services.AddHealthChecksUI()
                .AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseLoggingConfig(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            app.UseHealthChecks("/api/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(
                o =>
                {
                    o.UIPath = "/api/hc-ui";
                    o.ApiPath = "/api/hc-api";
                    o.UseRelativeApiPath = false;
                    o.UseRelativeResourcesPath = false;
                }
            );

            return app;
        }
    }
}
