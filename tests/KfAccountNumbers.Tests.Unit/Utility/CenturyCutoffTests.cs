namespace KfAccountNumbers.Tests.Unit.Utility;

public class CenturyCutoffTests
{
   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(CenturyCutoff.CutoffMinValue)]
   [InlineData(CenturyCutoff.DefaultCutoff)]
   [InlineData(CenturyCutoff.CutoffMaxValue)]
   public void CenturyCutoff_Constructor_ShouldCreateInstance_WhenValueIsValid(Int32 cutoff)
   {
      // Act.
      var sut = new CenturyCutoff(cutoff);

      // Assert.
      sut.Should().NotBeNull();
      sut.Cutoff.Should().Be(cutoff);
   }

   [Fact]
   public void CenturyCutoff_Constructor_ShouldCreateInstanceWithDefaultCutoff_WhenCutoffIsNotSupplied()
   {
      // Act.
      var sut = new CenturyCutoff();

      // Assert.
      sut.Should().NotBeNull();
      sut.Cutoff.Should().Be(CenturyCutoff.DefaultCutoff);
   }

   [Theory]
   [InlineData(CenturyCutoff.CutoffMinValue - 1)]
   [InlineData(CenturyCutoff.CutoffMaxValue + 1)]
   public void CenturyCutoff_Constructor_ShouldThrowArgumentOutOfRangeException_WhenValueIsInvalid(Int32 cutoff)
      => FluentActions
      .Invoking(() => new CenturyCutoff(cutoff))
      .Should()
      .ThrowExactly<ArgumentOutOfRangeException>()
      .WithParameterName(nameof(cutoff))
      .WithMessage(Messages.InvalidCenturyCutoff + "*")
      .And.ActualValue.Should().Be(cutoff);

   #endregion

   #region Cutoff Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(CenturyCutoff.CutoffMinValue)]
   [InlineData(CenturyCutoff.DefaultCutoff)]
   [InlineData(CenturyCutoff.CutoffMaxValue)]
   public void CenturyCutoff_Cutoff_ShouldReturnExpectedValue(Int32 cutoff)
   {
      // Arrange.
      var sut = new CenturyCutoff(cutoff);

      // Act/assert.
      sut.Cutoff.Should().Be(cutoff);
   }

   #endregion

   #region DefaultInstance Property Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void CenturyCutoff_DefaultInstance_ShouldReturnInstanceWithExpectedCutoff()
   {
      // Act.
      var sut = CenturyCutoff.DefaultInstance;

      // Assert.
      sut.Should().NotBeNull();
      sut.Cutoff.Should().Be(CenturyCutoff.DefaultCutoff);
   }

   [Fact]
   public void CenturyCutoff_DefaultInstance_ShouldReturnSameObject_WhenMultipleCallsMade()
   {
      // Act.
      var sut1 = CenturyCutoff.DefaultInstance;
      var sut2 = CenturyCutoff.DefaultInstance;

      // Assert.
      sut1.Should().BeSameAs(sut2);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(CenturyCutoff.CutoffMinValue)]
   [InlineData(CenturyCutoff.DefaultCutoff)]
   [InlineData(CenturyCutoff.CutoffMaxValue)]
   public void CenturyCutoff_ExplicitConversionToCenturyCutoff_ShouldCreateInstance_WhenValueIsValid(Int32 cutoff)
   {
      // Act.
      var sut = (CenturyCutoff)cutoff;

      // Assert.
      sut.Should().NotBeNull();
      sut.Cutoff.Should().Be(cutoff);
   }

   [Theory]
   [InlineData(CenturyCutoff.CutoffMinValue - 1)]
   [InlineData(CenturyCutoff.CutoffMaxValue + 1)]
   public void CenturyCutoff_ExplicitConversionToCenturyCutoff_ShouldThrowArgumentOutOfRangeException_WhenValueIsInvalid(Int32 cutoff)
      => FluentActions
         .Invoking(() => (CenturyCutoff)cutoff)
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(cutoff))
         .WithMessage(Messages.InvalidCenturyCutoff + "*")
         .And.ActualValue.Should().Be(cutoff);

   #endregion

   #region ToFourDigitYear Method Tests
   // ==========================================================================
   // ==========================================================================

#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
   [Theory]
   [InlineData( 0,   1)]
   [InlineData(49,  50)]
   [InlineData(99, 100)]
   public void CenturyCutoff_ToFourDigitYear_ShouldReturnCurrentCentury_WhenYearIsLessThanCutoff(
      Int32 year,
      Int32 cutoff)
   {
      // Arrange.
      var sut = new CenturyCutoff(cutoff);
      var expected = 2000 + year;

      // Act/assert.
      sut.ToFourDigitYear(year).Should().Be(expected);
   }

   [Theory]
   [InlineData( 1,  1)]
   [InlineData(50, 50)]
   [InlineData(99, 99)]
   public void CenturyCutoff_ToFourDigitYear_ShouldReturnPreviousCentury_WhenYearIsEqualToCutoff(
      Int32 year,
      Int32 cutoff)
   {
      // Arrange.
      var sut = new CenturyCutoff(cutoff);
      var expected = 1900 + year;

      // Act/assert.
      sut.ToFourDigitYear(year).Should().Be(expected);
   }

   [Theory]
   [InlineData( 2,  1)]
   [InlineData(51, 50)]
   [InlineData(99, 98)]
   public void CenturyCutoff_ToFourDigitYear_ShouldReturnPreviousCentury_WhenYearIsGreaterThanCutoff(
      Int32 year,
      Int32 cutoff)
   {
      // Arrange.
      var sut = new CenturyCutoff(cutoff);
      var expected = 1900 + year;

      // Act/assert.
      sut.ToFourDigitYear(year).Should().Be(expected);
   }

   [Theory]
   [InlineData(1000)]
   [InlineData(1900)]
   [InlineData(1999)]
   [InlineData(2000)]
   [InlineData(2099)]
   [InlineData(9999)]
   public void CenturyCutoff_ToFourDigitYear_ShouldReturnOriginalYear_WhenYearIsBetween1000And9999(Int32 year)
   {
      // Arrange.
      var sut = new CenturyCutoff(CenturyCutoff.CutoffMaxValue);

      // Act/assert.
      sut.ToFourDigitYear(year).Should().Be(year);
   }

   [Theory]
#pragma warning disable format
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1021 // Negative signs should be spaced correctly
   [InlineData(   -1,   1)]
   [InlineData(  100, 100)]
   [InlineData(  999, 100)]
   [InlineData(10000, 100)]
   public void CenturyCutoff_ToFourDigitYear_ShouldThrowArgumentOutOfRangeException_WhenYearIsInvalid(
      Int32 year,
      Int32 cutoff)
   {
      // Arrange.
      var sut = new CenturyCutoff(cutoff);

      // Act/assert.
      FluentActions
         .Invoking(() => sut.ToFourDigitYear(year))
         .Should()
         .ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(year))
         .WithMessage(Messages.InvalidYearForYyToYyyyConversion + "*")
         .And.ActualValue.Should().Be(year);
   }

#pragma warning restore SA1021 // Negative signs should be spaced correctly
   #pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
   #pragma warning restore format
   #pragma warning restore SA1008 // Opening parenthesis should be spaced correctly

   #endregion
}
