using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Splat;

namespace AutoSplat
{
    public class MockDependencyResolver : IDependencyResolver
    {
        private ModernDependencyResolver resolver = new ModernDependencyResolver();

        public void Dispose()
        {
            resolver.Dispose();
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
            if (!resolver.HasRegistration(serviceType, contract))
            {
                var obj = MockService(serviceType);
                resolver.Register(() => obj, serviceType, contract);
            }

            var result = resolver.GetService(serviceType, contract);
            if (!(result is Mock))
            {
                return result;
            }
            return GetMockObject(result, serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            if (!resolver.HasRegistration(serviceType, contract))
            {
                var obj = MockService(serviceType);
                resolver.Register(() => obj, serviceType, contract);
            }

            return resolver.GetServices(serviceType, contract)
                .Select(result =>
                {
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
            resolver.Register(factory, serviceType, contract);
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            return resolver.ServiceRegistrationCallback(serviceType, contract, callback);
        }

        public bool HasRegistration(Type serviceType, string contract = null)
        {
            return resolver.HasRegistration(serviceType, contract);
        }

        public void UnregisterCurrent(Type serviceType, string contract = null)
        {
            resolver.UnregisterCurrent(serviceType, contract);
        }

        public void UnregisterAll(Type serviceType, string contract = null)
        {
            resolver.UnregisterAll(serviceType, contract);
        }
    }
}
