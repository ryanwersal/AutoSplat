using Splat;

namespace AutoSplat.Tests
{
    public class Example
    {
        public IExample Foo { get; }

        public Example(IExample foo = null)
        {
            Foo = foo ?? Locator.Current.GetService<IExample>();
        }
    }
}
