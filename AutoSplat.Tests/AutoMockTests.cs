using System;
using System.Collections.Generic;
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
        public void ShouldReturnNonNullReference()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.NotNull(obj);
        }

        [Fact]
        public void ShouldReturnMockOfInterface()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.IsAssignableFrom<IExample>(obj);
        }

        [Fact]
        public void ShouldReturnEnumerableOfMocks()
        {
            var objects = Locator.Current.GetServices<IExample>();
            Assert.IsAssignableFrom<IEnumerable<IExample>>(objects);
        }

        [Fact]
        public void ShouldReturnSameInstance()
        {
            var obj1 = Locator.Current.GetService<IExample>();
            var obj2 = Locator.Current.GetService<IExample>();
            Assert.Equal(obj1, obj2);
        }

        [Fact]
        public void ShouldReturnDifferentInstancesForDifferentContracts()
        {
            var obj1 = Locator.Current.GetService<IExample>();
            var obj2 = Locator.Current.GetService<IExample>(nameof(IExample));
            Assert.NotEqual(obj1, obj2);
        }
    }
}
