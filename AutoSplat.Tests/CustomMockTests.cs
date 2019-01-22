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
            var expected = new object();
            Locator.CurrentMutable.Register(() => expected, typeof(object));
            var result = Locator.Current.GetService<object>();
            Assert.Same(expected, result);
        }

        [Fact]
        public void ShouldReturnNonMocks()
        {
            Locator.CurrentMutable.Register(() => new object(), typeof(object), "foo");
            Locator.CurrentMutable.Register(() => new object(), typeof(object), "bar");
            var results = Locator.Current.GetServices<object>("bar");
            Assert.NotNull(results);
            Assert.Single(results);
        }

        [Fact]
        public void ShouldReturnLastIfCollectionOfServicesRegistered()
        {
            var first = new Common(1);
            Locator.CurrentMutable.RegisterConstant(first, typeof(ICommon));
            var last = new Common(2);
            Locator.CurrentMutable.RegisterConstant(last, typeof(ICommon));

            Assert.Same(last, Locator.Current.GetService<ICommon>());
        }

        [Fact]
        public void RealFactoryMethodsShouldBeInvokedAtGetTimeNotRegisterTime()
        {
            const int expectedValue = 10;

            var index = 0;
            Locator.CurrentMutable.Register(() => new Common(index++), typeof(ICommon));

            for (var i = 0; i < expectedValue; ++i)
            {
                Locator.Current.GetService<ICommon>();
            }

            Assert.Equal(expectedValue, Locator.Current.GetService<ICommon>().Number);
        }
    }
}
