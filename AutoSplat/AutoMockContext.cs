using System;
using Splat;

namespace AutoSplat
{
    public class AutoMockContext : IDisposable
    {
        private IDependencyResolver _currentResolver;

        public AutoMockContext()
        {
            _currentResolver = Locator.Current;
            Locator.Current = new MockDependencyResolver();
        }

        public void Dispose()
        {
            Locator.Current = _currentResolver;
            _currentResolver = null;
        }
    }
}
