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
        private ConcurrentDictionary<ServiceInfo, ConcurrentBag<Mock>> Dependencies { get; } 
            = new ConcurrentDictionary<ServiceInfo, ConcurrentBag<Mock>>();

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
                Dependencies[serviceInfo] = new ConcurrentBag<Mock>(new[] { obj });
            }
            var mock = Dependencies[serviceInfo].First();
            return GetMockObject(mock, serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var serviceInfo = new ServiceInfo(serviceType, contract);
            if (!Dependencies.ContainsKey(serviceInfo))
            {
                var obj = MockService(serviceType);
                Dependencies[serviceInfo] = new ConcurrentBag<Mock>(new[] { obj });
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
                Dependencies[serviceInfo] = new ConcurrentBag<Mock>();
            }

            var obj = factory();
            if (!(obj is Mock))
            {
                // Splat has some default services it adds during container change.
                if (obj is ILogManager || obj is ILogger)
                {
                    return;
                }

                // Otherwise require all registered services to be mocks.
                throw new MustBeMockException();
            }
            Dependencies[serviceInfo].Add((Mock)obj);
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            throw new NotImplementedException();
        }
    }
}
