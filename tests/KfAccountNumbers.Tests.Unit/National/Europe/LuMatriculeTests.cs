namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class LuMatriculeTests
{
   [Fact]
   public void CheckDigitTest()
   {
      // Arrange.
      var value = "198501150013";

      // Act.
      var result = Algorithms.Verhoeff.Validate(value);

      // Assert.
      result.Should().BeTrue();
   }
}
