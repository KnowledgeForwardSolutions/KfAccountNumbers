namespace KfAccountNumbers.Tests.Unit.Utility;

public class DateResultTests
{
   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(1800,    1,    1)]      // January 1, 1800
   [InlineData(1899,   12,   31)]      // December 31, 1899

   [InlineData(1900,    1,    1)]      // January 1, 1900
   [InlineData(1999,   12,   31)]      // December 31, 1999

   [InlineData(2000,    1,    1)]      // January 1, 2000
   [InlineData(2099,   12,   31)]      // December 31, 2099

   [InlineData(1901,    1,   31)]      // maximum days for January, any year
   [InlineData(1991,    2,   28)]      // maximum days for February, non leap year
   [InlineData(1996,    2,   29)]      // maximum days for February, leap year
   [InlineData(2000,    2,   29)]      // maximum days for February, leap year (2000 is leap-year)
   [InlineData(1904,    3,   31)]      // maximum days for March, any year
   [InlineData(1904,    4,   30)]      // maximum days for April, any year
   [InlineData(1904,    5,   31)]      // maximum days for May, any year
   [InlineData(2004,    6,   30)]      // maximum days for June, any year
   [InlineData(2004,    7,   31)]      // maximum days for July, any year
   [InlineData(2004,    8,   31)]      // maximum days for August, any year
   [InlineData(2004,    9,   30)]      // maximum days for September, any year
   [InlineData(2004,   10,   31)]      // maximum days for October, any year
   [InlineData(2004,   11,   30)]      // maximum days for November, any year
   [InlineData(2004,   12,   31)]      // maximum days for December, any year

   [InlineData(null, null, null)]      // Unknown date
   [InlineData(2001, null, null)]      // Partial date, year only
   [InlineData(2001,   12, null)]      // Partial date, year and month
   [InlineData(2001, null,    1)]      // Partial date, year and day
   [InlineData(null,    2, null)]      // Partial date, month only
   [InlineData(null,    1,    1)]      // Partial date, month and day
   [InlineData(null, null,   28)]      // Partial date, day only

   [InlineData(   1, null, null)]      // Minimum valid year
   [InlineData(9999, null, null)]      // Maximum valid year
   [InlineData(null,    1, null)]      // Minimum valid month
   [InlineData(null,   12, null)]      // Maximum valid month
   [InlineData(null, null,    1)]      // Minimum valid day
   [InlineData(null, null,   31)]      // Maximum valid day
   public void DateResult_Constructor_ShouldCreateInstance_WhenAllValuesAreValid(
      Int32? year,
      Int32? month,
      Int32? day)
   {
      // Act.
      var sut = new DateResult(year, month, day);

      // Assert.
      sut.Should().NotBeNull();
      sut.Year.Should().Be(year);
      sut.Month.Should().Be(month);
      sut.Day.Should().Be(day);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(10000)]
   public void DateResult_Constructor_ShouldThrowArgumentOutOfRangeException_WhenYearIsInvalid(Int32 year)
      => FluentActions
         .Invoking(() => _ = new DateResult(year))
         .Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(year))
         .WithMessage(Messages.YearOutOfRange + "*")
         .And.ActualValue.Should().Be(year);

   [Theory]
   [InlineData(0)]
   [InlineData(13)]
   public void DateResult_Constructor_ShouldThrowArgumentOutOfRangeException_WhenMonthIsInvalid(Int32 month)
      => FluentActions
         .Invoking(() => _ = new DateResult(month: month))
         .Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(month))
         .WithMessage(Messages.MonthOutOfRange + "*")
         .And.ActualValue.Should().Be(month);

   [Theory]
   [InlineData(0)]
   [InlineData(32)]
   public void DateResult_Constructor_ShouldThrowArgumentOutOfRangeException_WhenDayIsInvalid(Int32 day)
      => FluentActions
         .Invoking(() => _ = new DateResult(day: day))
         .Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(day))
         .WithMessage(Messages.DayOutOfRange + "*")
         .And.ActualValue.Should().Be(day);

   [Theory]
   [InlineData(1904,  1, 32)]       // Invalid day of month for January, any year
   [InlineData(1901,  2, 29)]       // Invalid day of for February, non-leap year
   [InlineData(1904,  2, 30)]       // Invalid day of for February, leap year
   [InlineData(1904,  2, 30)]       // Invalid day of for February, leap year (2000 is leap-year)
   [InlineData(1904,  3, 32)]       // Invalid day of for March, any year
   [InlineData(1904,  4, 31)]       // Invalid day of for April, any year
   [InlineData(1904,  5, 32)]       // Invalid day of for May, any year
   [InlineData(2004,  6, 31)]       // Invalid day of for June, any year
   [InlineData(2004,  7, 32)]       // Invalid day of for July, any year
   [InlineData(2004,  8, 32)]       // Invalid day of for August, any year
   [InlineData(2004,  9, 31)]       // Invalid day of for September, any year
   [InlineData(2004, 10, 32)]       // Invalid day of for October, any year
   [InlineData(2004, 11, 31)]       // Invalid day of for November, any year
   [InlineData(2004, 12, 32)]       // Invalid day of for December, any year
   public void DateResult_Constructor_ShouldThrowArgumentOutOfRangeException_WhenDayIsInvalidForYearAndMonth(
      Int32 year,
      Int32 month,
      Int32 day)
   {
      // Arrange.
      var expectedMessage = String.Format(Messages.DayOutOfRangeForYearAndMonth, day, year, month);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = new DateResult(year, month, day))
         .Should().ThrowExactly<ArgumentOutOfRangeException>()
         .WithParameterName(nameof(day))
         .WithMessage(expectedMessage + "*")
         .And.ActualValue.Should().Be(day);
   }

   #endregion

   #region AvailableElements Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(null, null, null, DateElements.None)]
   [InlineData(2001, null, null, DateElements.Year)]
   [InlineData(null,    1, null, DateElements.Month)]
   [InlineData(null, null,    1, DateElements.Day)]
   [InlineData(2000,    1, null, DateElements.Year | DateElements.Month)]
   [InlineData(2000, null,    1, DateElements.Year | DateElements.Day)]
   [InlineData(null,    1,    1, DateElements.Month | DateElements.Day)]
   [InlineData(2001,    1,    1, DateElements.CompleteDate)]
   public void DateResult_AvailableElements_ShouldReturnExpectedValue(
      Int32? year,
      Int32? month,
      Int32? day,
      DateElements expected)
   {
      // Arrange.
      var sut = new DateResult(year, month, day);

      // Act/assert.
      sut.AvailableElements.Should().Be(expected);
   }

   #endregion

   #region Day Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(null, null, null, null)]
   [InlineData(2001, null, null, null)]
   [InlineData(null,    1, null, null)]
   [InlineData(null, null,    1,    1)]
   [InlineData(2000,    1, null, null)]
   [InlineData(2000, null,    1,    1)]
   [InlineData(null,    1,    1,    1)]
   public void DateResult_Day_ShouldReturnExpectedValue(
      Int32? year,
      Int32? month,
      Int32? day,
      Int32? expected)
   {
      // Arrange.
      var sut = new DateResult(year, month, day);

      // Act/assert.
      sut.Day.Should().Be(expected);
   }

   #endregion

   #region Month Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(null, null, null, null)]
   [InlineData(2001, null, null, null)]
   [InlineData(null,    1, null,    1)]
   [InlineData(null, null,    1, null)]
   [InlineData(2000,    1, null,    1)]
   [InlineData(2000, null,    1, null)]
   [InlineData(null,    1,    1,    1)]
   public void DateResult_Month_ShouldReturnExpectedValue(
      Int32? year,
      Int32? month,
      Int32? day,
      Int32? expected)
   {
      // Arrange.
      var sut = new DateResult(year, month, day);

      // Act/assert.
      sut.Month.Should().Be(expected);
   }

   #endregion

   #region Year Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(null, null, null, null)]
   [InlineData(2001, null, null, 2001)]
   [InlineData(null,    1, null, null)]
   [InlineData(null, null,    1, null)]
   [InlineData(2000,    1, null, 2000)]
   [InlineData(2000, null,    1, 2000)]
   [InlineData(null,    1,    1, null)]
   public void DateResult_Year_ShouldReturnExpectedValue(
      Int32? year,
      Int32? month,
      Int32? day,
      Int32? expected)
   {
      // Arrange.
      var sut = new DateResult(year, month, day);

      // Act/assert.
      sut.Year.Should().Be(expected);
   }

   #endregion

   #region ToDateOnly Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DateResult_ToDateOnly_ShouldReturnExpectedResult_WhenCompleteDateProvided()
   {
      // Arrange.
      var year = 2001;
      var month = 5;
      var day = 5;
      var sut = new DateResult(year, month, day);
      var expected = new DateOnly(year, month, day);

      // Act/assert.
      sut.ToDateOnly().Should().Be(expected);
   }

   [Theory]
   [InlineData(null, null, null)]
   [InlineData(2001, null, null)]
   [InlineData(null,    1, null)]
   [InlineData(null, null,    1)]
   [InlineData(2000,    1, null)]
   [InlineData(2000, null,    1)]
   [InlineData(null,    1,    1)]
   public void DateResult_ToDateOnly_ShouldThrowInvalidOperationException_WhenPartialDateProvided(
      Int32? year,
      Int32? month,
      Int32? day)
   {
      // Arrange.
      var sut = new DateResult(year, month, day);
      var expectedMessage = Messages.DateResultPartialDateToDateOnly;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.ToDateOnly())
         .Should().ThrowExactly<InvalidOperationException>()
         .WithMessage(Messages.DateResultPartialDateToDateOnly + "*");
   }

   #endregion
}
