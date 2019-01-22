using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Splat;

namespace AutoSplat
{
    public class MockDependencyResolver : IMutableDependencyResolver
    {
        private ConcurrentDictionary<ServiceInfo, ConcurrentBag<Func<object>>> Dependencies { get; } 
            = new ConcurrentDictionary<ServiceInfo, ConcurrentBag<Func<object>>>();

        public void Dispose()
        {
            Dependencies.Clear();
        }

        private static Mock MockService(Type serviceType)
        {
            var foo = typeof(Mock<>).MakeGenericType(serviceType);
            var mock = (Mock)Activator.CreateInstance(foo);
            return mock;
        }

        private static object GetMockObject(object mock, Type serviceType)
        {
            var foo = typeof(Mock<>).MakeGenericType(serviceType);
            var objProperty = foo.GetProperty(nameof(Object), foo);
            var obj = objProperty.GetValue(mock);
            return obj;
        }

        public object GetService(Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                Dependencies[serviceInfo] = new ConcurrentBag<Func<object>>();

                var obj = MockService(serviceType);
                Dependencies[serviceInfo].Add(() => obj);
            }

            var factory = Dependencies[serviceInfo].First();
            var result = factory();
            if (!(result is Mock))
            {
                return result;
            }
            return GetMockObject(result, serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                Dependencies[serviceInfo] = new ConcurrentBag<Func<object>>();

                var obj = MockService(serviceType);
                Dependencies[serviceInfo].Add(() => obj);
            }

            return Dependencies[serviceInfo]
                .Select(factory =>
                {
                    var result = factory();
                    if (!(result is Mock))
                    {
                        return result;
                    }
                    return GetMockObject(result, serviceType);
                })
                .ToList()
                .AsEnumerable();
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                Dependencies[serviceInfo] = new ConcurrentBag<Func<object>>();
            }

            Dependencies[serviceInfo].Add(factory);
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            throw new NotImplementedException();
        }
    }
}
