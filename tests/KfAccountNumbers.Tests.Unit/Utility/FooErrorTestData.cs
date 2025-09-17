namespace KfAccountNumbers.Tests.Unit.Utility;

public record Foo(String Details);

public enum FooErrorType
{
   None = 0,
   Empty,
   Bar
}

public static class FooErrors
{
   public static CreateError<FooErrorType> Empty => new(FooErrorType.Empty, "Empty foo");

   public static CreateError<FooErrorType> BarError => new(FooErrorType.Bar, "Bar is not a valid foo type");
}
