namespace KfAccountNumbers.Tests.Unit.Results;

public class KfOptionTests
{
   public readonly record struct Something(String Text);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_Constructor_ShouldCreateInstance_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");

      // Act.
      var sut = new KfOption<Something>(some);

      // Assert.
      sut.IsSome.Should().BeTrue();
      sut.IsNone.Should().BeFalse();
      sut.Value.Should().Be(some);
   }

   [Fact]
   public void KfOption_Constructor_ShouldCreateInstance_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);

      // Act.
      var sut = new KfOption<Something>(none);

      // Assert.
      sut.IsNone.Should().BeTrue();
      sut.IsSome.Should().BeFalse();
      sut.Value.Should().Be(none);
   }

   #endregion

   #region HasValue Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_HasValue_ShouldReturnTrue_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);

      // Act/assert.
      sut.HasValue.Should().BeTrue();
   }

   [Fact]
   public void KfOption_HasValue_ShouldReturnTrue_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);

      // Act/assert.
      sut.HasValue.Should().BeTrue();
   }

   #endregion

   #region IsNOne Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_IsNone_ShouldReturnFalse_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);

      // Act/assert.
      sut.IsNone.Should().BeFalse();
   }

   [Fact]
   public void KfOption_IsNone_ShouldReturnTrue_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);

      // Act/assert.
      sut.IsNone.Should().BeTrue();
   }

   #endregion

   #region IsSome Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_IsSome_ShouldReturnTrue_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);

      // Act/assert.
      sut.IsSome.Should().BeTrue();
   }

   [Fact]
   public void KfOption_IsSome_ShouldReturnFalse_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);

      // Act/assert.
      sut.IsSome.Should().BeFalse();
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_Value_ShouldReturnExpectedResult_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);

      // Act/assert.
      sut.Value.Should().Be(some);
   }

   [Fact]
   public void KfOption_Value_ShouldReturnExpectedResult_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);

      // Act/assert.
      sut.Value.Should().Be(none);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_ImplicitSomeConversion_ShouldReturnExpectedResult()
   {
      // Arrange.
      var some = new Something("asdf");

      // Act.
      KfOption<Something> sut = some;

      // Assert.
      sut.IsSome.Should().BeTrue();
      sut.Value.Should().Be(some);
   }

   [Fact]
   public void KfOption_ImplicitNoneConversion_ShouldReturnExpectedResult()
   {
      // Arrange.
      var none = default(None);

      // Act.
      KfOption<Something> sut = none;

      // Assert.
      sut.IsNone.Should().BeTrue();
      sut.Value.Should().Be(none);
   }

   #endregion

   #region TryGetValue Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void KfOption_TryGetValueSomeOverload_ShouldReturnTrue_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);

      // Act.
      var result = sut.TryGetValue(out Something value);

      // Assert.
      result.Should().BeTrue();
      value.Should().Be(some);
   }

   [Fact]
   public void KfOption_TryGetValueSomeOverload_ShouldReturnFalse_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);
      Something expected = default;

      // Act.
      var result = sut.TryGetValue(out Something value);

      // Assert.
      result.Should().BeFalse();
      value.Should().Be(expected);
   }

   [Fact]
   public void KfOption_TryGetValueNoneOverload_ShouldReturnTrue_WhenValueIsNone()
   {
      // Arrange.
      var none = default(None);
      var sut = new KfOption<Something>(none);

      // Act.
      var result = sut.TryGetValue(out None value);

      // Assert.
      result.Should().BeTrue();
      value.Should().Be(none);
   }

   [Fact]
   public void KfOption_TryGetValueNoneOverload_ShouldReturnFalse_WhenValueIsPresent()
   {
      // Arrange.
      var some = new Something("asdf");
      var sut = new KfOption<Something>(some);
      None expected = default;

      // Act.
      var result = sut.TryGetValue(out None value);

      // Assert.
      result.Should().BeFalse();
      value.Should().Be(expected);
   }

   #endregion
}
