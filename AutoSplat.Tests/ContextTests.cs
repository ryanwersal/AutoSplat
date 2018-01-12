using System;
using Splat;
using Xunit;

namespace AutoSplat.Tests
{
    public class ContextTests
    {
        [Fact]
        public void ReplacesDependencyResolver()
        {
            var currentResolver = Locator.Current;
            using (new AutoMockContext())
            {
                Assert.NotEqual(currentResolver, Locator.Current);
            }
        }

        [Fact]
        public void PutOriginalDependencyResolverBack()
        {
            var currentResolver = Locator.Current;
            using (new AutoMockContext()) { }
            Assert.Equal(currentResolver, Locator.Current);
        }
    }
}
