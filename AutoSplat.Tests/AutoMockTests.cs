using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Splat;
using Xunit;

namespace AutoSplat.Tests
{
    public class AutoMockTests : IDisposable
    {
        private AutoMockContext _context;

        public AutoMockTests()
        {
            _context = new AutoMockContext();
        }

        public void Dispose()
        {
            _context.Dispose();
            _context = null;
        }

        [Fact]
        public void GetServiceReturnsNonNullReference()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.NotNull(obj);
        }

        [Fact]
        public void GetServicesReturnsNonNullEnumerable()
        {
            var objects = Locator.Current.GetServices<IExample>();
            Assert.NotNull(objects);
        }

        [Fact]
        public void GetServiceReturnsMockOfInterface()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.IsAssignableFrom<IExample>(obj);
        }

        [Fact]
        public void GetServicesReturnsProperlyTypedEnumerable()
        {
            var objects = Locator.Current.GetServices<IExample>();
            Assert.IsAssignableFrom<IEnumerable<IExample>>(objects);
        }

        [Fact]
        public void GetServicesReturnsEnumerableOfMocksOfInterface()
        {
            var objects = Locator.Current.GetServices<IExample>();
            Assert.Collection(objects, o => Assert.IsAssignableFrom<IExample>(o));
        }

        [Fact]
        public void GetServicesReturnsNonEmptyEnumerable()
        {
            var objects = Locator.Current.GetServices<IExample>();
            Assert.NotEmpty(objects);
        }

        [Fact]
        public void GetServiceShouldReturnSameMockAcrossCalls()
        {
            var obj1 = Locator.Current.GetService<IExample>();
            var obj2 = Locator.Current.GetService<IExample>();
            Assert.Same(obj1, obj2);
        }

        [Fact]
        public void GetServicesShouldReturnSameMocksAcrossCalls()
        {
            var result1 = Locator.Current.GetServices<IExample>();
            var result2 = Locator.Current.GetServices<IExample>();
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetServiceReturnsDifferentMocksForDifferentContracts()
        {
            var obj1 = Locator.Current.GetService<IExample>();
            var obj2 = Locator.Current.GetService<IExample>(nameof(IExample));
            Assert.NotSame(obj1, obj2);
        }

        [Fact]
        public void GetServicesReturnsDifferentMocksForDifferentContracts()
        {
            var result1 = Locator.Current.GetServices<IExample>();
            var result2 = Locator.Current.GetServices<IExample>(nameof(IExample));
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void GetServicesReturnsOnlyCustomMockIfProvided()
        {
            var mock = new Mock<IExample>();
            Locator.CurrentMutable.RegisterConstant(mock.Object, typeof(IExample));

            var result = Locator.Current.GetServices<IExample>();
            Assert.Single(result);
            Assert.Same(mock.Object, result.First());
        }
    }
}
