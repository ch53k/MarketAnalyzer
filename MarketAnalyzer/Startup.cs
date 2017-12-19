using System;
using System.IO;
using System.Web.Http.Dispatcher;
using MarketAnalyzer.Model;
using MarketAnalyzer.Modules.App.Repository;
using MarketAnalyzer.Shared.DepedencyInjection;
using MarketAnalyzer.Shared.Stocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer
{
    public static class Startup
    {
        public static IConfiguration ConfigureOptions()
        {
            var executionDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Path.Combine(executionDirectoryInfo?.FullName ?? "", "Config"));
            builder.AddJsonFile("settings.debug.json", true);
            return builder.Build();
        }

        public static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddSingleton(provider => configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>());
            services.AddSingleton<IStockLoader, StockLoaderAlphaVantage>();

            services.AddSingleton<ITypeActivatorCache, AnalyzerTypeActivatorCache>();
            services.AddTransient<IHttpControllerActivator, AnalyzerHttpControllerActivator>();

            services.AddScoped<AnalyzerDbContext>();
            services.AddScoped<IStockProcessor, StockProcessor>();
            services.AddScoped<StockRepository>();
            return services.BuildServiceProvider();
        }
    }
}