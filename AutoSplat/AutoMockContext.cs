using System;
using Splat;

namespace AutoSplat
{
    public class AutoMockContext : IDisposable
    {
        private IDependencyResolver _currentResolver;

        public AutoMockContext()
        {
            _currentResolver = Locator.Current as IDependencyResolver;
            Locator.SetLocator(new MockDependencyResolver());
        }

        public void Dispose()
        {
            Locator.SetLocator(_currentResolver);
            _currentResolver = null;
        }
    }
}
