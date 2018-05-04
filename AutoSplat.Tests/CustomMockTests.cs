using System;
using Moq;
using Splat;
using Xunit;

namespace AutoSplat.Tests
{
    public class CustomMockTests : IDisposable
    {
        private AutoMockContext _context;

        public CustomMockTests()
        {
            _context = new AutoMockContext();
            Locator.CurrentMutable.Register(() =>
            {
                var mock = new Mock<IExample>();
                mock.Setup(m => m.DoFoo()).Returns(17);
                return mock;
            }, typeof(IExample));
        }

        public void Dispose()
        {
            _context.Dispose();
            _context = null;
        }

        [Fact]
        public void ShouldAllowAddingCustomMocks()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.NotNull(obj);
        }

        [Fact]
        public void ShouldReturnCustomMock()
        {
            var obj = Locator.Current.GetService<IExample>();
            Assert.Equal(17, obj.DoFoo());
        }

        [Fact]
        public void ShouldAllowNonMockRegistrations()
        {
            Locator.CurrentMutable.Register(() => new object(), typeof(object));
        }

        [Fact]
        public void ShouldReturnNonMock()
        {
            Locator.CurrentMutable.Register(() => new object(), typeof(object));
            var result = Locator.Current.GetService<object>();
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnNonMocks()
        {
            Locator.CurrentMutable.Register(() => new object(), typeof(object), "foo");
            Locator.CurrentMutable.Register(() => new object(), typeof(object), "bar");
            var results = Locator.Current.GetServices<object>();
            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }
    }
}
