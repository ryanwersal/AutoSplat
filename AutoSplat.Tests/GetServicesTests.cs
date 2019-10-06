using System;
using Moq;
using Splat;
using Xunit;

namespace AutoSplat.Tests
{
    public class GetServicesTests : IDisposable
    {
        private AutoMockContext _context;

        public GetServicesTests()
        {
            _context = new AutoMockContext();
        }

        public void Dispose()
        {
            _context.Dispose();
            _context = null;
        }

        [Fact]
        public void GetServicesInvokedWithNoContractReturnsServicesWithNoContract()
        {
            var expected = Mock.Of<ICommon>();

            Locator.CurrentMutable.Register(() => Mock.Of<ICommon>(), typeof(ICommon), "foo");
            Locator.CurrentMutable.Register(() => Mock.Of<ICommon>(), typeof(ICommon), "bar");
            Locator.CurrentMutable.Register(() => expected, typeof(ICommon));

            var results = Locator.Current.GetServices<ICommon>();
            Assert.Collection(results, o => Assert.Same(expected, o));
        }

        [Fact]
        public void GetServicesInvokedWithContractReturnsServicesWithThatContract()
        {
            const string contract = "bar";
            var expected = Mock.Of<ICommon>();

            Locator.CurrentMutable.Register(() => Mock.Of<ICommon>(), typeof(ICommon), "foo");
            Locator.CurrentMutable.Register(() => expected, typeof(ICommon), contract);
            Locator.CurrentMutable.Register(() => Mock.Of<ICommon>(), typeof(ICommon));

            var results = Locator.Current.GetServices<ICommon>(contract);
            Assert.Collection(results, o => Assert.Same(expected, o));
        }

        [Fact]
        public void GetServicesReturnsMoreThanOneService()
        {
            const string contract = "foobar";

            var firstExpected = Mock.Of<ICommon>();
            var secondExpected = Mock.Of<ICommon>();
            Locator.CurrentMutable.Register(() => firstExpected, typeof(ICommon), contract);
            Locator.CurrentMutable.Register(() => secondExpected, typeof(ICommon), contract);

            var results = Locator.Current.GetServices<ICommon>(contract);
            Assert.Contains(firstExpected, results);
            Assert.Contains(secondExpected, results);
        }

        [Fact]
        public void HasRegistrationReturnsTrueForExplicitRegistrations()
        {
            Type type = typeof(ICommon);
            const string contract = "foobar";

            Locator.CurrentMutable.Register(() => Mock.Of<ICommon>(), type, contract);

            var resolver = Locator.Current as IDependencyResolver;
            Assert.True(resolver.HasRegistration(type, contract));
        }

        [Fact]
        public void HasRegistrationReturnsFalseIfNoExplicitRegistration()
        {
            Type type = typeof(ICommon);
            const string contract = "foobar";

            var resolver = Locator.Current as IDependencyResolver;
            Assert.False(resolver.HasRegistration(type, contract));
        }
    }
}
