// Ignore Spelling: Kennitala

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class IsKennitalaTests
{
   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void IsKennitala_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => IsKennitala.MinimumValidYearOfBirth.Should().Be(1900);

   [Fact]
   public void IsKennitala_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => IsKennitala.MaximumValidYearOfBirth.Should().Be(2099);

   [Fact]
   public void IsKennitala_FyrirtaekiDayOffset_ShouldHaveExpectedValue()
      => IsKennitala.FyrirtaekiDayOffset.Should().Be(40);

   #endregion
}
