using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace MarketAnalyzer.Shared.DepedencyInjection
{
    public class DependencyScope : IDependencyScope
    {
        private IServiceProvider _serviceProvider;

        public DependencyScope(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
            try
            {
                if (_serviceProvider != null)
                {
                    (_serviceProvider as IDisposable)?.Dispose();
                    _serviceProvider = null;
                }
            }
            catch (Exception e)
            {
                HttpContext.Current.AddError(e);
                throw;
            }
        }
    }
}