using System;
using System.Collections.Generic;

namespace ChatApp.Core
{
    /// <summary>
    /// Lightweight dependency injection container to keep wiring out of views.
    /// </summary>
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _singletons = new();

        public void RegisterSingleton<TService>(TService instance)
        {
            _singletons[typeof(TService)] = instance;
        }

        public TService Resolve<TService>()
        {
            if (_singletons.TryGetValue(typeof(TService), out var instance))
            {
                return (TService)instance;
            }

            throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
        }
    }
}