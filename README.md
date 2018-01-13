# AutoSplat

[![AppVeyor](https://img.shields.io/appveyor/ci/ryanwersal/AutoSplat.svg?style=for-the-badge)](https://ci.appveyor.com/project/ryanwersal/autosplat)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/ryanwersal/AutoSplat.svg?style=for-the-badge)](https://ci.appveyor.com/project/ryanwersal/autosplat/build/tests)
[![NuGet](https://img.shields.io/nuget/v/AutoSplat.svg?style=for-the-badge)](https://www.nuget.org/packages/AutoSplat)

The objective is to do for Splat what projects like [Autofac.Extras.Moq provide](https://github.com/autofac/Autofac.Extras.Moq) for Autofac. In short, provide primitive mocks for types encountered using Splat's Locator to simplify mocking scenarios where you are
not verifying all dependencies but still require them to, at least, be non-null.

# Usage

## What It Does

AutoSplat provides the `AutoMockContext` class to handle swapping the Locator dependency resolver for you. It will put the original
resolver back in place once the context disposes. When inside the context, accessing services will return a mocked instance using Moq.

## Creating Mocks

If verifying mocked methods and such is required you will need to create the Mock yourself (within the context with the Locator dependency resolver swapped) and invoke the resolver's `Register` method with the mock:

```csharp
var example = new Mock<IExample>();
example.Setup(m => m.DoFoo()).Returns("foo");
Locator.CurrentMutable.Register(example.Object, typeof(IExample));
```