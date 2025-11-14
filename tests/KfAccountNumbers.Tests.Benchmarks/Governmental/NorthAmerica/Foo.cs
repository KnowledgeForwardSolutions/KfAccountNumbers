namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

public record Foo
{
   public Foo(String name)
      => Name = name;

   public String Name { get; init; }
}
