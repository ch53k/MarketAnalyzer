using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MarketAnalyzer.Shared.DepedencyInjection
{
    public class AnalyzerHttpControllerActivator : IHttpControllerActivator
    {
        private readonly ITypeActivatorCache _typeActivatorCache;

        public AnalyzerHttpControllerActivator(ITypeActivatorCache typeActivatorCache)
        {
            _typeActivatorCache = typeActivatorCache;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scope = (DependencyScope)request.GetDependencyScope();

            //var disposable = controller as IDisposable;
            //if (disposable != null)
            //{
            //    request.RegisterForDispose(disposable);
            //}

            return (IHttpController)_typeActivatorCache.CreateInstance<object>(scope.GetServiceProvider(), controllerType);
        }
    }
}