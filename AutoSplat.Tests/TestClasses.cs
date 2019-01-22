using Splat;

namespace AutoSplat.Tests
{
    public interface ICommon
    {
        int Number { get; }
    }

    public class Common : ICommon
    {
        public int Number { get; }

        public Common(int number)
        {
            Number = number;
        }
    }

    public interface IExample
    {
        int DoFoo();
    }

    public class Example
    {
        public IExample Foo { get; }

        public Example(IExample foo = null)
        {
            Foo = foo ?? Locator.Current.GetService<IExample>();
        }
    }
}
