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
        public void ShouldThrowIfTypeOtherThanMockProvided()
        {
            Assert.Throws<MustBeMockException>(
                () => Locator.CurrentMutable.Register(() => new object(), typeof(object)));
        }
    }
}
