namespace KfAccountNumbers.Tests.Unit.Utility;

public record Foo(String Details);

public enum FooErrorType
{
   None = 0,
   Empty,
   Bar
}
