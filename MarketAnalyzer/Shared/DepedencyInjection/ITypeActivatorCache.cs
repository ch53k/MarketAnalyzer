using System;

namespace MarketAnalyzer.Shared.DepedencyInjection
{
    public interface ITypeActivatorCache
    {
        TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type optionType);
    }
}