using DevIO.Business.Interfaces;
using DevIO.Business.Notifications;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<DevIODbContext>();
            services.AddScoped<INotifier, Notifier>();

            RegisterRepositories(services);
            RegisterServices(services);

            return services;
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IFornecedorService, FornecedorService>();
        }
    }
}
