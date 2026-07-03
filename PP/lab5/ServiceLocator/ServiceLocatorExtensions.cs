using ServiceLocatorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLocatorLib
{
    public static class ServiceLocatorExtensions
    {
        public static void AddTransient<TService>(this ServiceLocator locator, Func<ServiceScope, TService> factory) where TService : class
        {
            locator.AddService<TService>(new TransientStorage<TService>(factory));
        }
        public static void AddScoped<TService>(this ServiceLocator locator, Func<ServiceScope, TService> factory) where TService : class
        {
            locator.AddService<TService>(new ScopedStorage<TService>(factory));
        }

        public static void AddSingleton<TService>(this ServiceLocator locator, Func<ServiceScope, TService> factory) where TService : class
        {
            locator.AddService<TService>(new SingletonStorage<TService>(factory));
        }
    }
}

