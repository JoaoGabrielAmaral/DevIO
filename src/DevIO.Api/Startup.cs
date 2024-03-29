using AutoMapper;
using DevIO.Api.Configuration;
using DevIO.Api.Extensions;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevIO.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DevIODbContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            services.AddIdentityConfiguration(Configuration);
            services.AddAutoMapper(typeof(Startup));
            services.WebApiConfig();
            services.AddSwaggerConfig();
            services.ResolveDependencies();
            services.AddLoggingConfig(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("Development");
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvcConfiguration();
            app.UseSwaggerConfig(provider);
            app.UseLoggingConfig();
        }
    }
}
