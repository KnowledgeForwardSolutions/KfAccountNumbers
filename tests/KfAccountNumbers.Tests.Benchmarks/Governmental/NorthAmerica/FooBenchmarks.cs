#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class FooBenchmarks
{
   [Benchmark()]
   [Arguments("F'd up beyond all recognition")]
   [Arguments("FooBar")]
   public void FooConstructor(String name)
   {
      var fb = new Foo(name);
   }
}
