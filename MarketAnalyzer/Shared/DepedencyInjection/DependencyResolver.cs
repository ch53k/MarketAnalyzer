using System;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Shared.DepedencyInjection
{
    public class DependencyResolver : DependencyScope, IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope()
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();

            return new DependencyScope(scopeFactory.CreateScope().ServiceProvider);
        }
    }
}