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
        private ConcurrentDictionary<ServiceInfo, ConcurrentBag<object>> Dependencies { get; } 
            = new ConcurrentDictionary<ServiceInfo, ConcurrentBag<object>>();

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
                var obj = MockService(serviceType);
                Dependencies[serviceInfo] = new ConcurrentBag<object>(new[] { obj });
            }
            var service = Dependencies[serviceInfo].First();
            if (!(service is Mock))
            {
                return service;
            }
            return GetMockObject(service, serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                var obj = MockService(serviceType);
                Dependencies[serviceInfo] = new ConcurrentBag<object>(new[] { obj });
            }
            return Dependencies[serviceInfo]
                .Select(m => GetMockObject(m, serviceType))
                .ToList()
                .AsEnumerable();
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                Dependencies[serviceInfo] = new ConcurrentBag<object>();
            }

            var obj = factory();
            Dependencies[serviceInfo].Add(obj);
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            throw new NotImplementedException();
        }
    }
}
