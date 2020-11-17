using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services)
        {
            services.AddElmahIo(
                o =>
                {
                    o.ApiKey = "bcb8814b1c264f48b1633af3d5f8c40e";
                    o.LogId = new Guid("9828f062-3d67-43d9-80e6-b43fedc60495");
                }
            );

            //Recupera os logs adicionados manualmente
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

            return services;
        }

        public static IApplicationBuilder UseLoggingConfig(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            return app;
        }
    }
}
