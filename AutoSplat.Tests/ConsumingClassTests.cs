using System;
using Moq;
using Splat;
using Xunit;

namespace AutoSplat.Tests
{
    public class ConsumingClassTests : IDisposable
    {
        private AutoMockContext _context;

        public ConsumingClassTests()
        {
            _context = new AutoMockContext();
        }

        public void Dispose()
        {
            _context.Dispose();
            _context = null;
        }

        [Fact]
        public void ShouldMockLocatorCallsInClass()
        {
            var example = new Example();
            Assert.NotNull(example.Foo);
        }

        [Fact]
        public void ShouldReturnCustomMockFromLocatorCallsInClass()
        {
            const int value = 7;

            Locator.CurrentMutable.Register(() =>
            {
                var mock = new Mock<IExample>();
                mock.Setup(m => m.DoFoo()).Returns(value);
                return mock;
            }, typeof(IExample));

            var example = new Example();
            Assert.Equal(value, example.Foo.DoFoo());
        }
    }
}
